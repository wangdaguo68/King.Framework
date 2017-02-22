namespace King.Framework.DAL
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading;

    public class Table
    {
        private readonly Dictionary<string, Column> _columnDict;
        private readonly ReadOnlyCollection<Column> _columns;
        private readonly Column _identityColumn;
        private readonly ReadOnlyCollection<Column> _keyColumns;
        private readonly ReadOnlyCollection<MemberInfo> _keyMembers;
        private readonly ReadOnlyCollection<MemberInfo> _mappedMembers;
        private readonly ReadOnlyCollection<Column> _nonIdentityColumns;
        private readonly ReadOnlyCollection<Column> _nonKeyColumns;
        private readonly Table _parentTable;
        private readonly Dictionary<PropertyInfo, Column> _propertyDict;
        private readonly Dictionary<string, Column> _propertyNameDict;
        private readonly string _schemaName;
        private readonly string _tableName;
        private readonly Column _tableVersionColumn;
        internal string Delete_Oracle;
        internal string Delete_SqlServer;
        internal string Fill_ForUpdate_Oracle;
        internal string Fill_Oracle;
        internal string Fill_SqlServer;
        internal string Insert_Oracle;
        internal string Insert_SqlServer;
        internal string Select_Oracle;
        internal string Select_SqlServer;
        internal string Update_Oracle;
        internal string Update_SqlServer;

        private Table(Type type)
        {
            Column current;
            this._identityColumn = null;
            this._tableVersionColumn = null;
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (!type.IsClass)
            {
                throw new ApplicationException("type is not a class");
            }
            object[] customAttributes = type.GetCustomAttributes(typeof(KingTableAttribute), false);
            if (customAttributes.Length > 0)
            {
                KingTableAttribute attribute = customAttributes[0] as KingTableAttribute;
                if (string.IsNullOrWhiteSpace(attribute.Name))
                {
                    this._tableName = type.Name;
                }
                else
                {
                    this._tableName = attribute.Name;
                }
                if (string.IsNullOrWhiteSpace(attribute.Schema))
                {
                    this._schemaName = "dbo";
                }
                else
                {
                    this._schemaName = attribute.Schema;
                }
                if (attribute.IsInherited)
                {
                    if (type.BaseType == typeof(object))
                    {
                        throw new ApplicationException(string.Format("标记为继承的实体类型{0}不能从object继承", type));
                    }
                    this._parentTable = TableCache.GetTableOrCreate(type.BaseType);
                }
            }
            else
            {
                this._tableName = type.Name;
                this._schemaName = "dbo";
            }
            List<Column> source = new List<Column>();
            int num = 0;
            int num2 = 0;
            if (this.ParentTable != null)
            {
                using (IEnumerator<Column> enumerator = this.ParentTable.KeyColumns.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        current = enumerator.Current;
                        Column item = GetColumn(current.PropertyInfo);
                        source.Add(item);
                    }
                }
                num2 += this.ParentTable.KeyColumns.Count;
                num += this.ParentTable.KeyColumns.Count;
            }
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (PropertyInfo info in properties)
            {
                current = GetColumn(info);
                if (current != null)
                {
                    source.Add(current);
                    num++;
                    if (current.IsPrimaryKey)
                    {
                        num2++;
                    }
                }
            }
            if (num == 0)
            {
                throw new ApplicationException(string.Format("{0}未指定任何字段", type.Name));
            }
            if (num2 == 0)
            {
                string message = string.Format("{0}未指定义键", type.Name);
                Debug.WriteLine(message);
                Trace.WriteLine(message);
                throw new ApplicationException(message);
            }
            foreach (Column column in source)
            {
                column.TypeHelper = SharpTypeHelper.GetShartTypeHelper(column.DataType);
                column.GetFunction = SharpTypeHelper.CreateGetFunction(type, column.PropertyInfo.Name);
                column.SetAction = SharpTypeHelper.CreateSetAction(type, column.PropertyInfo.Name);
            }
            this._columns = source.AsReadOnly();
            this._keyColumns = (from p in source
                                where p.IsPrimaryKey
                                orderby p.KeyOrder
                                select p).ToList<Column>().AsReadOnly();
            this._nonKeyColumns = (from p in source
                                   where !p.IsPrimaryKey
                                   select p).ToList<Column>().AsReadOnly();
            this._identityColumn = source.FirstOrDefault<Column>(p => p.IsIdentity);
            this._nonIdentityColumns = (from p in source
                                        where !p.IsIdentity
                                        select p).ToList<Column>().AsReadOnly();
            this._propertyDict = source.ToDictionary<Column, PropertyInfo>(p => p.PropertyInfo);
            this._propertyNameDict = source.ToDictionary<Column, string>(p => p.PropertyInfo.Name);
            this._columnDict = source.ToDictionary<Column, string>(p => p.ColumnName.ToLower());
            this._keyMembers = (from p in this._keyColumns select p.PropertyInfo).Cast<MemberInfo>().ToList<MemberInfo>().AsReadOnly();
            this._mappedMembers = (from p in this._columns select p.PropertyInfo).Cast<MemberInfo>().ToList<MemberInfo>().AsReadOnly();
            this.InitSqlCache();
        }

        public static Table FromType(Type type)
        {
            return new Table(type);
        }

        private static Column GetColumn(PropertyInfo pi)
        {
            KingColumnAttribute attribute = Attribute.GetCustomAttribute(pi, typeof(KingColumnAttribute), false) as KingColumnAttribute;
            if (attribute != null)
            {
                string name;
                Type propertyType;
                if (string.IsNullOrWhiteSpace(attribute.ColumnName))
                {
                    name = pi.Name;
                }
                else
                {
                    name = attribute.ColumnName;
                }
                if (attribute.DataType == null)
                {
                    propertyType = pi.PropertyType;
                }
                else
                {
                    propertyType = attribute.DataType;
                }
                bool isTableVersion = false;
                if (Attribute.IsDefined(pi, typeof(KingTableVersionAttribute), false))
                {
                    isTableVersion = true;
                }
                return new Column(pi, name, propertyType, new int?(attribute.MaxLength), attribute.IsPrimaryKey, attribute.KeyOrder, attribute.IsIdentity, isTableVersion);
            }
            return null;
        }

        public Column GetColumnByColumnName(string columnName)
        {
            Column column;
            if (this._columnDict.TryGetValue(columnName.ToLower(), out column))
            {
                return column;
            }
            return null;
        }

        public Column GetColumnByProperty(PropertyInfo propertyInfo)
        {
            return this._propertyDict[propertyInfo];
        }

        public Column GetColumnByProperty(string propertyName)
        {
            return this._propertyNameDict[propertyName];
        }

        private string GetDelete_Oracle()
        {
            StringBuilder builder = new StringBuilder(0x200);
            builder.AppendFormat("delete from {0} where ", this.TableName);
            int num = 0;
            foreach (Column column in this.KeyColumns)
            {
                if (num > 0)
                {
                    builder.Append(" and ");
                }
                string str = ":" + column.ColumnName;
                builder.AppendFormat("{0} = {1}", column.ColumnName, str);
                num++;
            }
            return builder.ToString();
        }

        private string GetDelete_SqlServer()
        {
            StringBuilder builder = new StringBuilder(0x200);
            builder.AppendFormat("delete from [{0}].[{1}] where ", this.SchemaName, this.TableName);
            int num = 0;
            foreach (Column column in this.KeyColumns)
            {
                if (num > 0)
                {
                    builder.Append(" and ");
                }
                string str = "@" + column.ColumnName;
                builder.AppendFormat("[{0}]={1}", column.ColumnName, str);
                num++;
            }
            return builder.ToString();
        }

        private string GetFill_ForUpdate_Oracle()
        {
            return string.Format("{0} for update", this.GetFill_Oracle());
        }

        private string GetFill_Oracle()
        {
            StringBuilder builder = new StringBuilder(0x400);
            builder.AppendFormat("select ", new object[0]);
            string str = this.GetSelectedField_Oracle();
            builder.Append(str);
            builder.AppendFormat(" from {0} where ", this.TableName);
            int num = 0;
            foreach (Column column in this.KeyColumns)
            {
                if (num > 0)
                {
                    builder.Append(" and ");
                }
                string str2 = ":" + column.ColumnName;
                builder.AppendFormat("{0} = {1}", column.ColumnName, str2);
                num++;
            }
            return builder.ToString();
        }

        private string GetFill_SqlServer()
        {
            StringBuilder builder = new StringBuilder(0x400);
            builder.AppendFormat("select ", new object[0]);
            string str = this.GetSelectField_SqlServer();
            builder.Append(str);
            builder.AppendFormat(" from [{0}].[{1}] where ", this.SchemaName, this.TableName);
            int num = 0;
            foreach (Column column in this.KeyColumns)
            {
                if (num > 0)
                {
                    builder.Append(" and ");
                }
                string str2 = "@" + column.ColumnName;
                builder.AppendFormat("[{0}]={1}", column.ColumnName, str2);
                num++;
            }
            return builder.ToString();
        }

        public object GetFirstKeyValue(object obj)
        {
            return this.KeyColumns[0].GetFunction(obj);
        }

        public override int GetHashCode()
        {
            return (base.GetHashCode() ^ this.TableName.GetHashCode());
        }

        public string GetHashKey()
        {
            return string.Format("{0}_{1}", this.TableName, this.GetHashCode());
        }

        private string GetInsert_Oracle()
        {
            StringBuilder builder = new StringBuilder(0x400);
            builder.AppendFormat("insert into {0} (", this.TableName);
            int num = 0;
            foreach (Column column in this.NonIdentityColumns)
            {
                if (num > 0)
                {
                    builder.Append(",");
                }
                builder.AppendFormat("{0}", column.ColumnName);
                num++;
            }
            builder.Append(") values (");
            num = 0;
            foreach (Column column in this.NonIdentityColumns)
            {
                if (num > 0)
                {
                    builder.Append(",");
                }
                string str = ":" + column.ColumnName;
                builder.Append(str);
                num++;
            }
            builder.Append(")");
            if (this.IdentityColumn != null)
            {
                builder.Append(" SELECT Id as id");
            }
            return builder.ToString();
        }

        private string GetInsert_SqlServer()
        {
            StringBuilder builder = new StringBuilder(0x400);
            builder.AppendFormat("insert into [{0}].[{1}] (", this.SchemaName, this.TableName);
            int num = 0;
            foreach (Column column in this.NonIdentityColumns)
            {
                if (num > 0)
                {
                    builder.Append(",");
                }
                builder.AppendFormat("[{0}]", column.ColumnName);
                num++;
            }
            builder.Append(")values(");
            num = 0;
            foreach (Column column in this.NonIdentityColumns)
            {
                if (num > 0)
                {
                    builder.Append(",");
                }
                string str = "@" + column.ColumnName;
                builder.Append(str);
                num++;
            }
            builder.Append(")");
            if (this.IdentityColumn != null)
            {
                builder.Append(" SELECT SCOPE_IDENTITY() as id");
            }
            return builder.ToString();
        }

        public IEnumerable<Column> GetMappedColumns()
        {
            return this.Columns;
        }

        public IEnumerable<MemberInfo> GetMappedMembers()
        {
            return this._mappedMembers;
        }

        private string GetSelect_Oracle()
        {
            StringBuilder builder = new StringBuilder(0x400);
            builder.Append(" select ");
            string str = this.GetSelectedField_Oracle();
            builder.Append(str);
            builder.Append(" from ");
            builder.Append(this.TableName);
            builder.Append(" ");
            return builder.ToString();
        }

        private string GetSelect_SqlServer()
        {
            StringBuilder builder = new StringBuilder(0x400);
            builder.AppendFormat("select ", new object[0]);
            string str = this.GetSelectField_SqlServer();
            builder.Append(str);
            builder.Append(" from [");
            builder.Append(this.SchemaName);
            builder.Append("].[");
            builder.Append(this.TableName);
            builder.Append("] ");
            return builder.ToString();
        }

        private string GetSelectedField_Oracle()
        {
            StringBuilder builder = new StringBuilder(200);
            int num = -1;
            foreach (Column column in this.Columns)
            {
                num++;
                if (num > 0)
                {
                    builder.Append(",");
                }
                builder.Append(column.ColumnName);
            }
            return builder.ToString();
        }

        private string GetSelectField_SqlServer()
        {
            StringBuilder builder = new StringBuilder(200);
            int num = -1;
            foreach (Column column in this.Columns)
            {
                num++;
                if (num > 0)
                {
                    builder.Append(",");
                }
                builder.Append("[");
                builder.Append(column.ColumnName);
                builder.Append("]");
            }
            return builder.ToString();
        }

        private string GetUpdate_Oracle()
        {
            string str;
            StringBuilder builder = new StringBuilder(0x400);
            builder.AppendFormat("update {0} set ", this.TableName);
            int num = 0;
            foreach (Column column in this.NonKeyColumns)
            {
                if (column != this.IdentityColumn)
                {
                    if (num > 0)
                    {
                        builder.Append(",");
                    }
                    str = ":" + column.ColumnName;
                    builder.AppendFormat("{0} = {1}", column.ColumnName, str);
                    num++;
                }
            }
            builder.Append(" where ");
            num = 0;
            foreach (Column column in this.KeyColumns)
            {
                if (num > 0)
                {
                    builder.Append(" and ");
                }
                str = ":" + column.ColumnName;
                builder.AppendFormat("{0} = {1}", column.ColumnName, str);
                num++;
            }
            return builder.ToString();
        }

        private string GetUpdate_SqlServer()
        {
            string str;
            StringBuilder builder = new StringBuilder(0x400);
            builder.AppendFormat("update [{0}].[{1}] set ", this.SchemaName, this.TableName);
            int num = 0;
            foreach (Column column in this.NonKeyColumns)
            {
                if (column != this.IdentityColumn)
                {
                    if (num > 0)
                    {
                        builder.Append(",");
                    }
                    str = "@" + column.ColumnName;
                    builder.AppendFormat("[{0}]={1}", column.ColumnName, str);
                    num++;
                }
            }
            builder.Append(" where ");
            num = 0;
            foreach (Column column in this.KeyColumns)
            {
                if (num > 0)
                {
                    builder.Append(" and ");
                }
                str = "@" + column.ColumnName;
                builder.AppendFormat("[{0}]={1}", column.ColumnName, str);
                num++;
            }
            return builder.ToString();
        }

        private void InitSqlCache()
        {
            Interlocked.Exchange<string>(ref this.Fill_SqlServer, this.GetFill_SqlServer());
            Interlocked.Exchange<string>(ref this.Select_SqlServer, this.GetSelect_SqlServer());
            Interlocked.Exchange<string>(ref this.Insert_SqlServer, this.GetInsert_SqlServer());
            Interlocked.Exchange<string>(ref this.Update_SqlServer, this.GetUpdate_SqlServer());
            Interlocked.Exchange<string>(ref this.Delete_SqlServer, this.GetDelete_SqlServer());
            Interlocked.Exchange<string>(ref this.Fill_Oracle, this.GetFill_Oracle());
            Interlocked.Exchange<string>(ref this.Select_Oracle, this.GetSelect_Oracle());
            Interlocked.Exchange<string>(ref this.Insert_Oracle, this.GetInsert_Oracle());
            Interlocked.Exchange<string>(ref this.Update_Oracle, this.GetUpdate_Oracle());
            Interlocked.Exchange<string>(ref this.Delete_Oracle, this.GetDelete_Oracle());
            Interlocked.Exchange<string>(ref this.Fill_ForUpdate_Oracle, this.GetFill_ForUpdate_Oracle());
        }

        public override string ToString()
        {
            return string.Format("{0}", this._tableName);
        }

        public ReadOnlyCollection<Column> Columns
        {
            get
            {
                return this._columns;
            }
        }

        public Column IdentityColumn
        {
            get
            {
                return this._identityColumn;
            }
        }

        public ReadOnlyCollection<Column> KeyColumns
        {
            get
            {
                return this._keyColumns;
            }
        }

        public ReadOnlyCollection<MemberInfo> KeyMembers
        {
            get
            {
                return this._keyMembers;
            }
        }

        public ReadOnlyCollection<Column> NonIdentityColumns
        {
            get
            {
                return this._nonIdentityColumns;
            }
        }

        public ReadOnlyCollection<Column> NonKeyColumns
        {
            get
            {
                return this._nonKeyColumns;
            }
        }

        public Table ParentTable
        {
            get
            {
                return this._parentTable;
            }
        }

        public string SchemaName
        {
            get
            {
                return this._schemaName;
            }
        }

        public string TableName
        {
            get
            {
                return this._tableName;
            }
        }

        public Column TableVersionColumn
        {
            get
            {
                return this._tableVersionColumn;
            }
        }
    }
}
