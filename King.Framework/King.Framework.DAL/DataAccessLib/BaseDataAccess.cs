using System.Reflection;

namespace King.Framework.DAL.DataAccessLib
{
    using King.Framework.DAL;
    using Oracle.ManagedDataAccess.Client;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;
    using System.Transactions;

    public abstract class BaseDataAccess
    {
        protected BaseDataAccess()
        {
        }

        public virtual void BatchInsert<T>(DbConnection conn, IEnumerable<T> objList)
        {
            foreach (T local in objList)
            {
                this.Insert(conn, local);
            }
        }

        public virtual void BatchInsert<T>(DbTransaction trans, IEnumerable<T> objList)
        {
            foreach (T local in objList)
            {
                this.Insert(trans, local);
            }
        }

        public virtual void BatchInsert(DbConnection conn, ICollection objList, Type type)
        {
            foreach (object obj2 in objList)
            {
                this.Insert(conn, obj2);
            }
        }

        public virtual void BatchInsert(DbTransaction trans, ICollection objList, Type type)
        {
            foreach (object obj2 in objList)
            {
                this.Insert(trans, obj2);
            }
        }

        protected virtual DbCommand BuildDeleteCommand(Table t)
        {
            SqlCommand command = new SqlCommand();
            foreach (Column column in t.KeyColumns)
            {
                string str = "@" + column.ColumnName;
                SqlParameter parameter = new SqlParameter
                {
                    ParameterName = str
                };
                command.Parameters.Add(parameter);
            }
            command.CommandType = CommandType.Text;
            command.CommandText = t.Delete_SqlServer;
            return command;
        }

        protected virtual DbCommand BuildFillAndLockCommand(Table t, int? wait)
        {
            throw new Exception("SQL Server暂时不支持此方法");
        }

        protected virtual DbCommand BuildFillCommand(Table t)
        {
            SqlCommand command = new SqlCommand();
            foreach (Column column in t.KeyColumns)
            {
                string str = "@" + column.ColumnName;
                SqlParameter parameter = new SqlParameter
                {
                    ParameterName = str
                };
                command.Parameters.Add(parameter);
            }
            command.CommandType = CommandType.Text;
            command.CommandText = t.Fill_SqlServer;
            return command;
        }

        protected virtual DbCommand BuildInsertCommand(Table t)
        {
            SqlCommand command = new SqlCommand();
            foreach (Column column in t.NonIdentityColumns)
            {
                string str = "@" + column.ColumnName;
                SqlParameter parameter = new SqlParameter
                {
                    ParameterName = str,
                    DbType = this.GetDbType(column.DataType),
                    Direction = ParameterDirection.Input,
                    SqlDbType = this.GetSqlDbType(column.DataType),
                    Precision = 0,
                    Scale = 0
                };
                command.Parameters.Add(parameter);
            }
            command.CommandType = CommandType.Text;
            command.CommandText = t.Insert_SqlServer;
            return command;
        }

        protected virtual DbCommand BuildSelectCommand(Table t)
        {
            return new SqlCommand {CommandType = CommandType.Text, CommandText = t.Select_SqlServer};
        }

        protected virtual DbCommand BuildUpdateCommand(Table t)
        {
            string str;
            SqlCommand command = new SqlCommand();
            foreach (Column column in t.NonKeyColumns)
            {
                str = "@" + column.ColumnName;
                SqlParameter parameter = new SqlParameter
                {
                    ParameterName = str,
                    DbType = this.GetDbType(column.DataType),
                    Direction = ParameterDirection.Input,
                    SqlDbType = this.GetSqlDbType(column.DataType)
                };
                command.Parameters.Add(parameter);
            }
            foreach (Column column in t.KeyColumns)
            {
                str = "@" + column.ColumnName;
                SqlParameter parameter2 = new SqlParameter
                {
                    ParameterName = str,
                    DbType = this.GetDbType(column.DataType),
                    Direction = ParameterDirection.Input,
                    SqlDbType = this.GetSqlDbType(column.DataType)
                };
                command.Parameters.Add(parameter2);
            }
            command.CommandType = CommandType.Text;
            command.CommandText = t.Update_SqlServer;
            return command;
        }

        protected virtual DbCommand BuildUpdatePartialCommand(Table t, Dictionary<Column, object> keyDict,
            Dictionary<Column, object> valueDict)
        {
            string str;
            SqlCommand command = new SqlCommand();
            StringBuilder builder = new StringBuilder(0x400);
            builder.AppendFormat("update [{0}].[{1}] set ", t.SchemaName, t.TableName);
            int num = 0;
            foreach (Column column in valueDict.Keys)
            {
                object obj2 = valueDict[column];
                if (obj2 == null)
                {
                    obj2 = DBNull.Value;
                }
                if (column != t.IdentityColumn)
                {
                    if (num > 0)
                    {
                        builder.Append(",");
                    }
                    str = "@" + column.ColumnName;
                    builder.AppendFormat("[{0}]={1}", column.ColumnName, str);
                    SqlParameter parameter = new SqlParameter
                    {
                        ParameterName = str,
                        DbType = this.GetDbType(column.DataType),
                        Direction = ParameterDirection.Input,
                        SqlDbType = this.GetSqlDbType(column.DataType),
                        Value = obj2
                    };
                    command.Parameters.Add(parameter);
                    num++;
                }
            }
            builder.Append(" where ");
            num = 0;
            foreach (Column column in keyDict.Keys)
            {
                if (num > 0)
                {
                    builder.Append(",");
                }
                str = "@" + column.ColumnName;
                builder.AppendFormat("[{0}]={1}", column.ColumnName, str);
                SqlParameter parameter2 = new SqlParameter
                {
                    ParameterName = str,
                    DbType = this.GetDbType(column.DataType),
                    Direction = ParameterDirection.Input,
                    SqlDbType = this.GetSqlDbType(column.DataType),
                    Value = keyDict[column]
                };
                command.Parameters.Add(parameter2);
                num++;
            }
            command.CommandType = CommandType.Text;
            command.CommandText = builder.ToString();
            return command;
        }

        public void ClearCommand(DbCommand cmd)
        {
            cmd.Connection = null;
            cmd.Transaction = null;
        }

        public virtual DbDataAdapter CreateDataAdapter()
        {
            return new SqlDataAdapter();
        }

        public virtual DbParameter CreateParameter()
        {
            return new SqlParameter();
        }

        public virtual string CreateParameterName(string nameWithOutPrefix)
        {
            return string.Format("@{0}", nameWithOutPrefix);
        }

        public virtual void CreateTable(DbConnection conn, Type type)
        {
            Table tableOrCreate = TableCache.GetTableOrCreate(type);
            StringBuilder builder = new StringBuilder(0x800);
            builder.AppendFormat("CREATE TABLE [{0}](", tableOrCreate.TableName);
            foreach (Column column in tableOrCreate.Columns)
            {
                builder.AppendFormat(" [{0}] {1} {2} NULL, ", column.ColumnName, column.GetSqlTypeString(),
                    column.IsPrimaryKey ? "NOT" : string.Empty);
            }
            builder.AppendFormat(" CONSTRAINT [PK_{0}] PRIMARY KEY ( ", tableOrCreate.TableName);
            int num = -1;
            foreach (Column column in tableOrCreate.KeyColumns)
            {
                num++;
                if (num > 0)
                {
                    builder.Append(",");
                }
                builder.AppendFormat(" [{0}] ", column.ColumnName);
            }
            builder.AppendFormat(" ))", new object[0]);
            using (DbCommand command = conn.CreateCommand())
            {
                command.Connection = conn;
                try
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = builder.ToString();
                    Trace.WriteLine(command.CommandText);
                    command.ExecuteNonQuery();
                }
                finally
                {
                    command.Connection = null;
                    command.Transaction = null;
                }
            }
        }

        public static List<T> DataReaderToList<T>(DbDataReader reader, IEnumerable<ColumnWrapper> columns)
        {
            List<T> list = new List<T>();
            while (reader.Read())
            {
                T local = Activator.CreateInstance<T>();
                foreach (ColumnWrapper wrapper in columns)
                {
                    object obj2 = reader[wrapper.Index];
                    if ((obj2 != null) && (obj2 != DBNull.Value))
                    {
                        Column col = wrapper.col;
                        col.SetAction(local, col.TypeHelper.ConvertFrom(obj2, col.DataType));
                    }
                }
                list.Add(local);
            }
            return list;
        }

        public static IList DataReaderToList(Type type, DbDataReader reader, IEnumerable<ColumnWrapper> columns)
        {
            IList list = Activator.CreateInstance(typeof (List<>).MakeGenericType(new Type[] {type})) as IList;
            while (reader.Read())
            {
                object obj2 = Activator.CreateInstance(type);
                foreach (ColumnWrapper wrapper in columns)
                {
                    object obj3 = reader[wrapper.Index];
                    if ((obj3 != null) && (obj3 != DBNull.Value))
                    {
                        Column col = wrapper.col;
                        col.SetAction(obj2, col.TypeHelper.ConvertFrom(obj3, col.DataType));
                    }
                }
                list.Add(obj2);
            }
            return list;
        }

        public static List<T> DataTableToList<T>(DataTable data, IEnumerable<ColumnWrapper> columns)
        {
            List<T> list = new List<T>(data.Rows.Count);
            foreach (DataRow row in data.Rows)
            {
                T local = Activator.CreateInstance<T>();
                foreach (ColumnWrapper wrapper in columns)
                {
                    object obj2 = row[wrapper.Index];
                    if ((obj2 != null) && (obj2 != DBNull.Value))
                    {
                        Column col = wrapper.col;
                        col.SetAction(local, col.TypeHelper.ConvertFrom(obj2, col.DataType));
                    }
                }
                list.Add(local);
            }
            return list;
        }

        public static IList DataTableToList(Type type, DataTable data, IEnumerable<ColumnWrapper> columns)
        {
            IList list = Activator.CreateInstance(typeof (List<>).MakeGenericType(new Type[] {type})) as IList;
            foreach (DataRow row in data.Rows)
            {
                object obj2 = Activator.CreateInstance(type);
                foreach (ColumnWrapper wrapper in columns)
                {
                    object obj3 = row[wrapper.Index];
                    if ((obj3 != null) && (obj3 != DBNull.Value))
                    {
                        Column col = wrapper.col;
                        col.SetAction(obj2, col.TypeHelper.ConvertFrom(obj3, col.DataType));
                    }
                }
                list.Add(obj2);
            }
            return list;
        }

        public void Delete(DbConnection conn, object obj)
        {
            Table tableOrCreate = TableCache.GetTableOrCreate(obj.GetType());
            DbCommand deleteCommand = this.GetDeleteCommand(tableOrCreate, obj);
            deleteCommand.Connection = conn;
            try
            {
                Trace.WriteLine(deleteCommand.CommandText);
                deleteCommand.ExecuteNonQuery();
            }
            finally
            {
                deleteCommand.Connection = null;
            }
        }

        public void Delete(DbTransaction trans, object obj)
        {
            Table tableOrCreate = TableCache.GetTableOrCreate(obj.GetType());
            DbCommand deleteCommand = this.GetDeleteCommand(tableOrCreate, obj);
            deleteCommand.Connection = trans.Connection;
            deleteCommand.Transaction = trans;
            try
            {
                Trace.WriteLine(deleteCommand.CommandText);
                deleteCommand.ExecuteNonQuery();
            }
            finally
            {
                deleteCommand.Connection = null;
                deleteCommand.Transaction = null;
            }
        }

        public virtual void DeleteAll(DbConnection conn, Type type)
        {
            Table tableOrCreate = TableCache.GetTableOrCreate(type);
            DbCommand command = new SqlCommand(string.Format("delete from [{0}]", tableOrCreate.TableName))
            {
                Connection = (SqlConnection)conn
            };
            try
            {
                Trace.WriteLine(command.CommandText);
                command.ExecuteNonQuery();
            }
            finally
            {
                command.Connection = null;
            }
        }

        public virtual void DeleteAll(DbTransaction trans, Type type)
        {
            Table tableOrCreate = TableCache.GetTableOrCreate(type);
            DbCommand command = new SqlCommand(string.Format("delete from [{0}]", tableOrCreate.TableName))
            {
                Transaction = (SqlTransaction)trans,
                Connection =(SqlConnection) trans.Connection
            };
            try
            {
                Trace.WriteLine(command.CommandText);
                command.ExecuteNonQuery();
            }
            finally
            {
                command.Transaction = null;
                command.Connection = null;
            }
        }

        public DataTable ExecuteDataTable(DbConnection _connection, DbTransaction _transaction, string sql,
            params DbParameter[] paramArray)
        {
            DataTable table = new DataTable();
            using (DbCommand command = _connection.CreateCommand())
            {
                command.Connection = _connection;
                command.Transaction = _transaction;
                try
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = sql;
                    if ((paramArray != null) && (paramArray.Length > 0))
                    {
                        command.Parameters.AddRange(paramArray);
                    }
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        table.Load(reader);
                    }
                    foreach (DataColumn column in table.Columns)
                    {
                        column.ReadOnly = false;
                        if (column.DataType == typeof (string))
                        {
                            column.MaxLength = 0x7fffffff;
                        }
                        column.AllowDBNull = true;
                    }
                    return table;
                }
                finally
                {
                    command.Connection = null;
                    command.Transaction = null;
                }
            }
        }

        public async Task<DataTable> ExecuteDataTableAsync(DbConnection _connection, DbTransaction _transaction,
            string sql, params DbParameter[] paramArray)
        {
            DataTable dataTable = new DataTable();
            using (DbCommand cmd = _connection.CreateCommand())
            {
                cmd.Connection = _connection;
                cmd.Transaction = _transaction;
                try
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;
                    if ((paramArray != null) && (paramArray.Length > 0))
                    {
                        cmd.Parameters.AddRange(paramArray);
                    }
                    using (DbDataReader asyncVariable0 = await cmd.ExecuteReaderAsync())
                    {
                        dataTable.Load(asyncVariable0);
                    }
                    IEnumerator enumerator = dataTable.Columns.GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            DataColumn current = (DataColumn) enumerator.Current;
                            current.ReadOnly = false;
                            if (current.DataType == typeof (string))
                            {
                                current.MaxLength = 0x7fffffff;
                            }
                            current.AllowDBNull = true;
                        }
                    }
                    finally
                    {
                        IDisposable disposable = enumerator as IDisposable;
                        if (disposable != null)
                        {
                            disposable.Dispose();
                        }
                    }
                }
                finally
                {
                    cmd.Connection = null;
                    cmd.Transaction = null;
                }
            }
            return dataTable;
        }

        public int ExecuteNonQuery(DbConnection _connection, DbTransaction _transaction, string sql,
            params DbParameter[] paramArray)
        {
            int num;
            using (DbCommand command = _connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = sql;
                command.Parameters.AddRange(paramArray);
                command.Connection = _connection;
                command.Transaction = _transaction;
                try
                {
                    num = command.ExecuteNonQuery();
                }
                finally
                {
                    command.Transaction = null;
                    command.Connection = null;
                }
            }
            return num;
        }

        public async Task<int> ExecuteNonQueryAsync(DbConnection _connection, DbTransaction _transaction, string sql,
            params DbParameter[] paramArray)
        {
            int num;
            using (DbCommand cmd = _connection.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
                cmd.Parameters.AddRange(paramArray);
                cmd.Connection = _connection;
                cmd.Transaction = _transaction;
                try
                {
                    num = await cmd.ExecuteNonQueryAsync();
                }
                finally
                {
                    cmd.Transaction = null;
                    cmd.Connection = null;
                }
            }
            return num;
        }

        public List<T> ExecuteObjectSet<T>(DbConnection _connection, DbTransaction _transaction, string sql,
            params DbParameter[] paramArray)
        {
            List<T> list;
            using (DbCommand command = _connection.CreateCommand())
            {
                command.Connection = _connection;
                command.Transaction = _transaction;
                try
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = sql;
                    if ((paramArray != null) && (paramArray.Length > 0))
                    {
                        command.Parameters.AddRange(paramArray);
                    }
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        List<ColumnWrapper> sortedColumns = GetSortedColumns<T>(reader);
                        return DataReaderToList<T>(reader, sortedColumns);
                    }
                }
                finally
                {
                    command.Connection = null;
                    command.Transaction = null;
                }
            }
        }

        public object ExecuteScalar(DbConnection _connection, DbTransaction _transaction, string sql,
            params DbParameter[] paramArray)
        {
            object obj2;
            using (DbCommand command = _connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = sql;
                if ((paramArray != null) && (paramArray.Length > 0))
                {
                    command.Parameters.AddRange(paramArray);
                }
                command.Connection = _connection;
                command.Transaction = _transaction;
                try
                {
                    obj2 = command.ExecuteScalar();
                }
                finally
                {
                    command.Transaction = null;
                    command.Connection = null;
                }
            }
            return obj2;
        }

        public virtual object ExecuteStoredProcedure(DbConnection conn, string procedureName, bool isReturnDataTable,
            params DbParameter[] parameters)
        {
            if (isReturnDataTable)
            {
                return this.ExecuteStoredProcedureDataTable(conn, procedureName, parameters);
            }
            return this.ExecuteStoredProcedureNonQuery(conn, procedureName, parameters);
        }

        protected virtual DataTable ExecuteStoredProcedureDataTable(DbConnection conn, string procedureName,
            params DbParameter[] parameters)
        {
            DbCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = procedureName;
            foreach (DbParameter parameter in parameters)
            {
                cmd.Parameters.Add(parameter);
            }
            this.FormatDbNull(cmd);
            DbDataAdapter adapter = this.CreateDataAdapter();
            adapter.SelectCommand = cmd;
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            return dataTable;
        }

        protected virtual int ExecuteStoredProcedureNonQuery(DbConnection conn, string procedureName,
            params DbParameter[] parameters)
        {
            DbCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = procedureName;
            foreach (DbParameter parameter in parameters)
            {
                cmd.Parameters.Add(parameter);
            }
            this.FormatDbNull(cmd);
            return cmd.ExecuteNonQuery();
        }

        public virtual bool ExistTable(DbConnection conn, Type type)
        {
            Table tableOrCreate = TableCache.GetTableOrCreate(type);
            string sql = string.Format("select COUNT(*) from sysobjects where name= '{0}' and type= 'U'",
                tableOrCreate.TableName);
            return (Convert.ToInt32(this.ExecuteScalar(conn, null, sql, new DbParameter[0])) > 0);
        }

        public bool Fill(DbConnection conn, object obj)
        {
            Table tableOrCreate = TableCache.GetTableOrCreate(obj.GetType());
            DbCommand fillCommand = this.GetFillCommand(tableOrCreate, obj);
            fillCommand.Connection = conn;
            bool flag = false;
            try
            {
                using (DbDataReader reader = fillCommand.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (reader.Read())
                    {
                        FillEntity(obj, reader, false);
                        flag = true;
                    }
                }
            }
            finally
            {
                fillCommand.Connection = null;
            }
            return flag;
        }

        public void Fill(DbTransaction trans, object obj)
        {
            Table tableOrCreate = TableCache.GetTableOrCreate(obj.GetType());
            DbCommand fillCommand = this.GetFillCommand(tableOrCreate, obj);
            fillCommand.Connection = trans.Connection;
            fillCommand.Transaction = trans;
            try
            {
                using (DbDataReader reader = fillCommand.ExecuteReader(CommandBehavior.SingleRow))
                {
                    FillEntity(obj, reader, false);
                }
            }
            finally
            {
                fillCommand.Connection = null;
                fillCommand.Transaction = null;
            }
        }

        public bool FillAndLock(DbConnection conn, object obj, int? wait)
        {
            Table tableOrCreate = TableCache.GetTableOrCreate(obj.GetType());
            DbCommand command = this.GetFillAndLockCommand(tableOrCreate, obj, wait);
            command.Connection = conn;
            bool flag = false;
            try
            {
                Trace.WriteLine(command.CommandText);
                using (DbDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (reader.Read())
                    {
                        FillEntity(obj, reader, false);
                        flag = true;
                    }
                }
            }
            finally
            {
                command.Connection = null;
            }
            return flag;
        }

        public static void FillEntity(object obj, DbDataReader row, bool fillByName = false)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            if (row == null)
            {
                throw new ArgumentNullException("row");
            }
            Type type = obj.GetType();
            Table tableOrCreate = TableCache.GetTableOrCreate(type);
            if (tableOrCreate == null)
            {
                throw new ApplicationException(string.Format("{0}未指定KingTableAttribute", type.Name));
            }
            int num = -1;
            foreach (Column column in tableOrCreate.Columns)
            {
                object obj2;
                num++;
                if (fillByName)
                {
                    obj2 = row[column.ColumnName];
                }
                else
                {
                    obj2 = row[num];
                }
                if ((obj2 != null) && (obj2 != DBNull.Value))
                {
                    column.SetAction(obj, column.TypeHelper.ConvertFrom(obj2, column.DataType));
                }
            }
        }

        public static void FillEntity(object obj, DbDataReader reader, IEnumerable<ColumnWrapper> colWrappers)
        {
            foreach (ColumnWrapper wrapper in colWrappers)
            {
                object obj2 = reader[wrapper.Index];
                if ((obj2 != null) && (obj2 != DBNull.Value))
                {
                    Column col = wrapper.col;
                    col.SetAction(obj, col.TypeHelper.ConvertFrom(obj2, col.DataType));
                }
            }
        }

        public static void FillEntity(object obj, DataRow row, bool fillByName = false)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            if (row == null)
            {
                throw new ArgumentNullException("row");
            }
            Type type = obj.GetType();
            Table tableOrCreate = TableCache.GetTableOrCreate(type);
            if (tableOrCreate == null)
            {
                throw new ApplicationException(string.Format("{0}未指定KingTableAttribute", type.Name));
            }
            int num = -1;
            foreach (Column column in tableOrCreate.Columns)
            {
                object obj2;
                num++;
                if (fillByName)
                {
                    obj2 = row[column.ColumnName];
                }
                else
                {
                    obj2 = row[num];
                }
                if ((obj2 != null) && (obj2 != DBNull.Value))
                {
                    column.SetAction(obj, column.TypeHelper.ConvertFrom(obj2, column.DataType));
                }
            }
        }

        public static void FillEntity(object obj, DataRow row, IEnumerable<ColumnWrapper> colWrappers)
        {
            foreach (ColumnWrapper wrapper in colWrappers)
            {
                object obj2 = row[wrapper.Index];
                if ((obj2 != null) && (obj2 != DBNull.Value))
                {
                    Column col = wrapper.col;
                    col.SetAction(obj, col.TypeHelper.ConvertFrom(obj2, col.DataType));
                }
            }
        }

        public virtual IList FilterWithOneField(DbConnection conn, Type type, string columnName, object value)
        {
            DbParameter parameter = this.CreateParameter();
            parameter.ParameterName = this.CreateParameterName(columnName);
            parameter.Value = value;
            string whereCondition = string.Format("{0} = {1}", columnName, parameter.ParameterName);
            return this.Select(type, conn, whereCondition, new DbParameter[] {parameter});
        }

        public List<T> FindMore<T>(DbConnection conn, int[] idArray) where T : class
        {
            if ((idArray == null) || (idArray.Length <= 0))
            {
                return new List<T>();
            }
            Table tableOrCreate = TableCache.GetTableOrCreate(typeof (T));
            if (tableOrCreate.KeyColumns.Count <= 0)
            {
                throw new ApplicationException("表未指定主键");
            }
            if (tableOrCreate.KeyColumns.Count > 1)
            {
                throw new ApplicationException("FindMore只能查一个主键的表");
            }
            if (tableOrCreate.KeyColumns[0].DataType != typeof (int))
            {
                throw new ApplicationException("FindMore只能查主键是int类型的表");
            }
            StringBuilder builder = new StringBuilder(0x200);
            builder.Append("SELECT ");
            int num = -1;
            foreach (Column column in tableOrCreate.Columns)
            {
                num++;
                if (num > 0)
                {
                    builder.Append(",");
                }
                builder.Append(column.ColumnName);
            }
            builder.Append(" FROM ");
            builder.Append(tableOrCreate.TableName);
            builder.Append(" WHERE ");
            builder.Append(tableOrCreate.KeyColumns[0].ColumnName);
            builder.Append(" IN (");
            int length = builder.Length;
            idArray = idArray.Distinct<int>().ToArray<int>();
            List<T> list = new List<T>(idArray.Length);
            using (DbCommand command = conn.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                for (int i = 0; i < idArray.Length; i += 0x80)
                {
                    int count = 0x80;
                    if ((i + count) > idArray.Length)
                    {
                        count = idArray.Length - i;
                    }
                    builder.Remove(length, builder.Length - length);
                    builder.Append(string.Join<int>(",", idArray.Skip<int>(i).Take<int>(count)));
                    builder.Append(")");
                    command.CommandText = builder.ToString();
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(RowToEntity<T>(reader, false));
                        }
                    }
                }
            }
            return list;
        }

        protected virtual void FormatDbNull(DbCommand cmd)
        {
            foreach (DbParameter parameter in cmd.Parameters)
            {
                if (parameter.Value == null)
                {
                    SqlParameter parameter2 = parameter as SqlParameter;
                    if (parameter2.SqlDbType != SqlDbType.Image)
                    {
                        parameter2.SqlDbType = SqlDbType.NVarChar;
                    }
                    parameter.Value = DBNull.Value;
                    parameter.Size = 0;
                }
            }
        }

        protected DbType GetDbType(Type propertyType)
        {
            if (propertyType.IsEnum)
            {
                return DbType.Int32;
            }
            if (IsNullableEnum(propertyType))
            {
                return DbType.Int32;
            }
            if (propertyType == typeof (string))
            {
                return DbType.String;
            }
            if (propertyType == typeof (int))
            {
                return DbType.Int32;
            }
            if (propertyType == typeof (int?))
            {
                return DbType.Int32;
            }
            if (propertyType == typeof (long))
            {
                return DbType.Int64;
            }
            if (propertyType == typeof (long?))
            {
                return DbType.Int64;
            }
            if (propertyType == typeof (decimal))
            {
                return DbType.Decimal;
            }
            if (propertyType == typeof (decimal?))
            {
                return DbType.Decimal;
            }
            if (propertyType == typeof (DateTime))
            {
                return DbType.DateTime;
            }
            if (propertyType == typeof (DateTime?))
            {
                return DbType.DateTime;
            }
            if (propertyType == typeof (bool))
            {
                return DbType.Boolean;
            }
            if (propertyType == typeof (bool?))
            {
                return DbType.Boolean;
            }
            if (propertyType == typeof (Guid))
            {
                return DbType.Guid;
            }
            if (propertyType == typeof (Guid?))
            {
                return DbType.Guid;
            }
            if (propertyType == typeof (byte[]))
            {
                return DbType.Binary;
            }
            if (propertyType == typeof (byte))
            {
                return DbType.Byte;
            }
            if (propertyType == typeof (byte?))
            {
                return DbType.Byte;
            }
            if (propertyType == typeof (short))
            {
                return DbType.Int16;
            }
            if (propertyType == typeof (short?))
            {
                return DbType.Int16;
            }
            if ((propertyType != typeof (double)) && (propertyType != typeof (double?)))
            {
                throw new ApplicationException(string.Format("不支持的数据类型:{0}", propertyType.ToString()));
            }
            return DbType.Double;
        }

        private DbCommand GetDeleteCommand(Table t, object obj)
        {
            DbCommand cmd = null;
            cmd = this.BuildDeleteCommand(t);
            int num = 0;
            foreach (Column column in t.KeyColumns)
            {
                cmd.Parameters[num].Value = column.GetFunction(obj);
                num++;
            }
            this.FormatDbNull(cmd);
            return cmd;
        }

        private DbCommand GetFillAndLockCommand(Table t, object obj, int? wait)
        {
            DbCommand cmd = null;
            cmd = this.BuildFillAndLockCommand(t, wait);
            int num = 0;
            foreach (Column column in t.KeyColumns)
            {
                cmd.Parameters[num].Value = column.GetFunction(obj);
                num++;
            }
            this.FormatDbNull(cmd);
            return cmd;
        }

        private DbCommand GetFillCommand(Table t, object obj)
        {
            DbCommand cmd = null;
            cmd = this.BuildFillCommand(t);
            int num = 0;
            foreach (Column column in t.KeyColumns)
            {
                cmd.Parameters[num].Value = column.GetFunction(obj);
                num++;
            }
            this.FormatDbNull(cmd);
            return cmd;
        }

        private DbCommand GetInsertCommand(Table t, object obj)
        {
            DbCommand cmd = null;
            cmd = this.BuildInsertCommand(t);
            int num = 0;
            foreach (Column column in t.NonIdentityColumns)
            {
                cmd.Parameters[num].Value = column.GetFunction(obj);
                if (column.IsTableVersion)
                {
                }
                num++;
            }
            this.FormatDbNull(cmd);
            return cmd;
        }

        public static IEnumerable<Column> GetMappedColumns(Type type)
        {
            return TableCache.GetTableOrCreate(type).GetMappedColumns();
        }

        public static IEnumerable<MemberInfo> GetMappedMembers(Type type)
        {
            return TableCache.GetTableOrCreate(type).GetMappedMembers();
        }

        public virtual long GetNextIdentity(DbProviderFactory factory, string connStr, string key, bool rollBack = false)
        {
            return Convert.ToInt64(this.GetNextIdentity_Int(factory, connStr, key, rollBack));
        }

        public virtual int GetNextIdentity_Int(DbProviderFactory factory, string connStr, string key,
            bool rollBack = false)
        {
            object obj2 = -1;
            TransactionScope scope = null;
            if (!rollBack)
            {
                scope = new TransactionScope(TransactionScopeOption.Suppress);
            }
            using (DbConnection connection = factory.CreateConnection())
            {
                connection.ConnectionString = connStr;
                connection.Open();
                if (string.IsNullOrEmpty(key))
                {
                    key = "CommonIdentity";
                }
                DbParameter parameter = this.CreateParameter();
                this.SetDbParameter(parameter, this.CreateParameterName("Key"), key, 12);
                string sql =
                    string.Format("select CurrentId from SysIdSequence with (UPDLOCK,ROWLOCK) where IdKey = {0}",
                        parameter.ParameterName);
                DbTransaction transaction = null;
                try
                {
                    DbParameter parameter4;
                    transaction = connection.BeginTransaction(System.Data.IsolationLevel.Serializable);
                    DbParameter parameter2 = this.CreateParameter();
                    this.SetDbParameter(parameter2, this.CreateParameterName("Key"), key, 12);
                    DbParameter parameter3 = this.CreateParameter();
                    this.SetDbParameter(parameter3, this.CreateParameterName("UpdateTime"), DateTime.Now, 4);
                    object obj3 = this.ExecuteScalar(connection, transaction, sql, new DbParameter[] {parameter});
                    if (obj3 == null)
                    {
                        obj2 = 1;
                        parameter4 = this.CreateParameter();
                        this.SetDbParameter(parameter4, this.CreateParameterName("CurrentId"), obj2, 8);
                        sql = string.Format("insert SysIdSequence (IdKey,CurrentId,UpdateTime) values ({0},{1},{2})",
                            parameter2.ParameterName, parameter4.ParameterName, parameter3.ParameterName);
                        this.ExecuteNonQuery(connection, transaction, sql,
                            new DbParameter[] {parameter2, parameter4, parameter3});
                    }
                    else
                    {
                        obj2 = Convert.ToInt32(obj3) + 1;
                        parameter4 = this.CreateParameter();
                        this.SetDbParameter(parameter4, this.CreateParameterName("CurrentId"), obj2, 8);
                        sql =
                            string.Format(
                                "update SysIdSequence Set CurrentId = {1},UpdateTime = {2} where IdKey = {0}",
                                parameter2.ParameterName, parameter4.ParameterName, parameter3.ParameterName);
                        this.ExecuteNonQuery(connection, transaction, sql,
                            new DbParameter[] {parameter4, parameter3, parameter2});
                    }
                    transaction.Commit();
                }
                catch
                {
                    if (transaction != null)
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch
                        {
                        }
                    }
                }
                connection.Close();
            }
            if (!rollBack)
            {
                scope.Complete();
                scope.Dispose();
            }
            return Convert.ToInt32(obj2);
        }

        public virtual int GetNextIdentity_Int_Old(DbProviderFactory factory, string connStr)
        {
            object obj2;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                using (DbConnection connection = factory.CreateConnection())
                {
                    connection.ConnectionString = connStr;
                    connection.Open();
                    string sql = " INSERT INTO T_EfSequence(Temp) VALUES(0) SELECT SCOPE_IDENTITY()";
                    obj2 = this.ExecuteScalar(connection, null, sql, new DbParameter[0]);
                }
                scope.Complete();
            }
            return Convert.ToInt32(obj2);
        }

        public virtual long GetNextIdentity_Old(DbProviderFactory factory, string connStr)
        {
            object obj2;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                using (DbConnection connection = factory.CreateConnection())
                {
                    connection.ConnectionString = connStr;
                    connection.Open();
                    string sql = " INSERT INTO T_EfSequence(Temp) VALUES(0) SELECT SCOPE_IDENTITY()";
                    obj2 = this.ExecuteScalar(connection, null, sql, new DbParameter[0]);
                }
                scope.Complete();
            }
            return Convert.ToInt64(obj2);
        }

        protected OracleDbType GetOracleType(Type propertyType)
        {
            if (propertyType.IsEnum)
            {
                return OracleDbType.Int32;
            }
            if (IsNullableEnum(propertyType))
            {
                return OracleDbType.Int32;
            }
            if (propertyType == typeof (string))
            {
                return OracleDbType.NVarchar2;
            }
            if (propertyType == typeof (int))
            {
                return OracleDbType.Int32;
            }
            if (propertyType == typeof (int?))
            {
                return OracleDbType.Int32;
            }
            if (propertyType == typeof (long))
            {
                return OracleDbType.Int64;
            }
            if (propertyType == typeof (long?))
            {
                return OracleDbType.Int64;
            }
            if (propertyType == typeof (decimal))
            {
                return OracleDbType.Decimal;
            }
            if (propertyType == typeof (decimal?))
            {
                return OracleDbType.Decimal;
            }
            if (propertyType == typeof (DateTime))
            {
                return OracleDbType.TimeStamp;
            }
            if (propertyType == typeof (DateTime?))
            {
                return OracleDbType.TimeStamp;
            }
            if (propertyType == typeof (bool))
            {
                return OracleDbType.Int32;
            }
            if (propertyType == typeof (bool?))
            {
                return OracleDbType.Int32;
            }
            if (propertyType == typeof (Guid))
            {
                return OracleDbType.NVarchar2;
            }
            if (propertyType == typeof (Guid?))
            {
                return OracleDbType.NVarchar2;
            }
            if (propertyType == typeof (byte[]))
            {
                return OracleDbType.Blob;
            }
            if (propertyType == typeof (byte))
            {
                return OracleDbType.Int16;
            }
            if (propertyType == typeof (byte?))
            {
                return OracleDbType.Int16;
            }
            if (propertyType == typeof (short))
            {
                return OracleDbType.Int16;
            }
            if (propertyType == typeof (short?))
            {
                return OracleDbType.Int16;
            }
            if ((propertyType != typeof (double)) && (propertyType != typeof (double?)))
            {
                throw new ApplicationException(string.Format("不支持的数据类型:{0}", propertyType.ToString()));
            }
            return OracleDbType.Double;
        }

        private DbCommand GetSelectCommand(Table t, string whereCondition, params DbParameter[] cmdParams)
        {
            DbCommand cmd = this.BuildSelectCommand(t);
            if (!string.IsNullOrWhiteSpace(whereCondition))
            {
                StringBuilder builder = new StringBuilder(cmd.CommandText);
                builder.Append("where ");
                builder.Append(whereCondition);
                cmd.CommandText = builder.ToString();
            }
            if (cmdParams != null)
            {
                cmd.Parameters.AddRange(cmdParams);
            }
            this.FormatDbNull(cmd);
            return cmd;
        }

        public static List<ColumnWrapper> GetSortedColumns<T>(DbDataReader reader)
        {
            return GetSortedColumns(typeof (T), reader);
        }

        public static List<ColumnWrapper> GetSortedColumns<T>(DataTable data)
        {
            return GetSortedColumns(typeof (T), data);
        }

        public virtual List<ColumnWrapper> GetSortedColumns(DbConnection conn, Type type)
        {
            List<ColumnWrapper> sortedColumns;
            Table tableOrCreate = TableCache.GetTableOrCreate(type);
            string str = string.Format("select top 1 * from {0}", tableOrCreate.TableName);
            using (DbCommand command = conn.CreateCommand())
            {
                command.CommandText = str;
                command.CommandType = CommandType.Text;
                try
                {
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        sortedColumns = GetSortedColumns(type, reader);
                    }
                }
                finally
                {
                    command.Connection = null;
                }
            }
            return sortedColumns;
        }

        public static List<ColumnWrapper> GetSortedColumns(Type type, DbDataReader reader)
        {
            List<ColumnWrapper> list = new List<ColumnWrapper>();
            Table tableOrCreate = TableCache.GetTableOrCreate(type);
            int fieldCount = reader.FieldCount;
            for (int i = 0; i < fieldCount; i++)
            {
                string name = reader.GetName(i);
                Column columnByColumnName = tableOrCreate.GetColumnByColumnName(name);
                if (columnByColumnName != null)
                {
                    list.Add(new ColumnWrapper(i, columnByColumnName));
                }
            }
            return list;
        }

        public static List<ColumnWrapper> GetSortedColumns(Type type, DataTable data)
        {
            List<ColumnWrapper> list = new List<ColumnWrapper>();
            Table tableOrCreate = TableCache.GetTableOrCreate(type);
            for (int i = 0; i < data.Columns.Count; i++)
            {
                string columnName = data.Columns[i].ColumnName;
                Column columnByColumnName = tableOrCreate.GetColumnByColumnName(columnName);
                if (columnByColumnName != null)
                {
                    list.Add(new ColumnWrapper(i, columnByColumnName));
                }
            }
            return list;
        }

        protected SqlDbType GetSqlDbType(Type propertyType)
        {
            if (propertyType.IsEnum)
            {
                return SqlDbType.Int;
            }
            if (IsNullableEnum(propertyType))
            {
                return SqlDbType.Int;
            }
            if (propertyType == typeof (string))
            {
                return SqlDbType.NVarChar;
            }
            if (propertyType == typeof (int))
            {
                return SqlDbType.Int;
            }
            if (propertyType == typeof (int?))
            {
                return SqlDbType.Int;
            }
            if (propertyType == typeof (long))
            {
                return SqlDbType.BigInt;
            }
            if (propertyType == typeof (long?))
            {
                return SqlDbType.BigInt;
            }
            if (propertyType == typeof (decimal))
            {
                return SqlDbType.Decimal;
            }
            if (propertyType == typeof (decimal?))
            {
                return SqlDbType.Decimal;
            }
            if (propertyType == typeof (DateTime))
            {
                return SqlDbType.DateTime;
            }
            if (propertyType == typeof (DateTime?))
            {
                return SqlDbType.DateTime;
            }
            if (propertyType == typeof (bool))
            {
                return SqlDbType.Bit;
            }
            if (propertyType == typeof (bool?))
            {
                return SqlDbType.Bit;
            }
            if (propertyType == typeof (Guid))
            {
                return SqlDbType.UniqueIdentifier;
            }
            if (propertyType == typeof (Guid?))
            {
                return SqlDbType.UniqueIdentifier;
            }
            if (propertyType == typeof (byte[]))
            {
                return SqlDbType.Image;
            }
            if (propertyType == typeof (byte))
            {
                return SqlDbType.Int;
            }
            if (propertyType == typeof (byte?))
            {
                return SqlDbType.Int;
            }
            if (propertyType == typeof (short))
            {
                return SqlDbType.SmallInt;
            }
            if (propertyType == typeof (short?))
            {
                return SqlDbType.SmallInt;
            }
            if ((propertyType != typeof (double)) && (propertyType != typeof (double?)))
            {
                throw new ApplicationException(string.Format("不支持的数据类型:{0}", propertyType.ToString()));
            }
            return SqlDbType.Float;
        }

        public static string GetTableName(Type type)
        {
            return TableCache.GetTableOrCreate(type).TableName;
        }

        private DbCommand GetUpdateCommand(Table t, object obj)
        {
            DbCommand cmd = null;
            cmd = this.BuildUpdateCommand(t);
            int num = 0;
            foreach (Column column in t.NonKeyColumns)
            {
                if (column != t.IdentityColumn)
                {
                    cmd.Parameters[num].Value = column.GetFunction(obj);
                    num++;
                }
            }
            this.FormatDbNull(cmd);
            foreach (Column column in t.KeyColumns)
            {
                cmd.Parameters[num].Value = column.GetFunction(obj);
                num++;
            }
            return cmd;
        }

        public void Insert(DbConnection conn, object obj)
        {
            Table tableOrCreate = TableCache.GetTableOrCreate(obj.GetType());
            DbCommand insertCommand = this.GetInsertCommand(tableOrCreate, obj);
            insertCommand.Connection = conn;
            try
            {
                Trace.WriteLine(insertCommand.CommandText);
                if (tableOrCreate.IdentityColumn != null)
                {
                    object obj2 = insertCommand.ExecuteScalar();
                    tableOrCreate.IdentityColumn.TypeHelper.SetValue(obj, tableOrCreate.IdentityColumn, obj2);
                }
                else
                {
                    insertCommand.ExecuteNonQuery();
                }
            }
            finally
            {
                insertCommand.Connection = null;
            }
        }

        public void Insert(DbTransaction trans, object obj)
        {
            Table tableOrCreate = TableCache.GetTableOrCreate(obj.GetType());
            DbCommand insertCommand = this.GetInsertCommand(tableOrCreate, obj);
            insertCommand.Connection = trans.Connection;
            insertCommand.Transaction = trans;
            try
            {
                Trace.WriteLine(insertCommand.CommandText);
                if (tableOrCreate.IdentityColumn != null)
                {
                    object obj2 = insertCommand.ExecuteScalar();
                    tableOrCreate.IdentityColumn.TypeHelper.SetValue(obj, tableOrCreate.IdentityColumn, obj2);
                }
                else
                {
                    insertCommand.ExecuteNonQuery();
                }
            }
            finally
            {
                insertCommand.Connection = null;
                insertCommand.Transaction = null;
            }
        }

        public async Task InsertAsync(DbConnection conn, object obj)
        {
            Table tableOrCreate = TableCache.GetTableOrCreate(obj.GetType());
            DbCommand insertCommand = this.GetInsertCommand(tableOrCreate, obj);
            insertCommand.Connection = conn;
            try
            {
                Trace.WriteLine(insertCommand.CommandText);
                if (tableOrCreate.IdentityColumn != null)
                {
                    object id = await insertCommand.ExecuteScalarAsync();
                    tableOrCreate.IdentityColumn.TypeHelper.SetValue(obj, tableOrCreate.IdentityColumn, id);
                }
                else
                {
                    await insertCommand.ExecuteNonQueryAsync();
                }
            }
            finally
            {
                insertCommand.Connection = null;
            }
        }

        public async Task InsertAsync(DbTransaction trans, object obj)
        {
            Table tableOrCreate = TableCache.GetTableOrCreate(obj.GetType());
            DbCommand insertCommand = this.GetInsertCommand(tableOrCreate, obj);
            insertCommand.Connection = trans.Connection;
            insertCommand.Transaction = trans;
            try
            {
                Trace.WriteLine(insertCommand.CommandText);
                if (tableOrCreate.IdentityColumn != null)
                {
                    object id = await insertCommand.ExecuteScalarAsync();
                    tableOrCreate.IdentityColumn.TypeHelper.SetValue(obj, tableOrCreate.IdentityColumn, id);
                }
                else
                {
                    await insertCommand.ExecuteNonQueryAsync();
                }
            }
            finally
            {
                insertCommand.Connection = null;
                insertCommand.Transaction = null;
            }
        }

        protected virtual void InternalBatchInsert(DbConnection conn, IEnumerable objList, Type type)
        {
            Table tableOrCreate = TableCache.GetTableOrCreate(type);
            DataTable table = new DataTable(tableOrCreate.TableName);
            List<ColumnWrapper> sortedColumns = this.GetSortedColumns(conn, type);
            int sourceColumnIndex = 0;
            while (sourceColumnIndex < sortedColumns.Count)
            {
                Column col = sortedColumns[sourceColumnIndex].col;
                table.Columns.Add(col.ColumnName, col.SimpleDataType);
                sourceColumnIndex++;
            }
            foreach (object obj2 in objList)
            {
                DataRow row = table.NewRow();
                for (sourceColumnIndex = 0; sourceColumnIndex < sortedColumns.Count; sourceColumnIndex++)
                {
                    object obj3 = sortedColumns[sourceColumnIndex].col.GetFunction(obj2);
                    if (obj3 == null)
                    {
                        obj3 = DBNull.Value;
                    }
                    row[sourceColumnIndex] = obj3;
                }
                table.Rows.Add(row);
            }
            using (SqlBulkCopy copy = new SqlBulkCopy(conn as SqlConnection))
            {
                copy.BatchSize = 100;
                copy.BulkCopyTimeout = 0x4b0;
                copy.DestinationTableName = tableOrCreate.TableName;
                for (sourceColumnIndex = 0; sourceColumnIndex < sortedColumns.Count; sourceColumnIndex++)
                {
                    copy.ColumnMappings.Add(sourceColumnIndex, sortedColumns[sourceColumnIndex].Index);
                }
                copy.WriteToServer(table);
            }
        }

        public static bool IsEnumOrNullableEnum(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return (type.IsEnum || IsNullableEnum(type));
        }

        public static bool IsGenericNullableType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof (Nullable<>)));
        }

        public static bool IsNullableEnum(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof (Nullable<>)))
            {
                Type type2 = type.GetGenericArguments()[0];
                if (type2.IsEnum)
                {
                    return true;
                }
            }
            return false;
        }

        public static T RowToEntity<T>(DbDataReader reader, bool fillByName = false)
        {
            return (T) RowToEntity(typeof (T), reader, fillByName);
        }

        public static object RowToEntity(Type type, DbDataReader reader, bool fillByName = false)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            object obj2 = Activator.CreateInstance(type);
            FillEntity(obj2, reader, fillByName);
            return obj2;
        }

        public static object RowToEntity(Type type, DbDataReader reader, IEnumerable<ColumnWrapper> colWrappers)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            object obj2 = Activator.CreateInstance(type);
            FillEntity(obj2, reader, colWrappers);
            return obj2;
        }

        public static object RowToEntity(Type type, DataRow row, bool fillByName = false)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            object obj2 = Activator.CreateInstance(type);
            FillEntity(obj2, row, fillByName);
            return obj2;
        }

        public static object RowToEntity(Type type, DataRow row, IEnumerable<ColumnWrapper> colWrappers)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            object obj2 = Activator.CreateInstance(type);
            FillEntity(obj2, row, colWrappers);
            return obj2;
        }

        public List<T> Select<T>(DbConnection conn, string whereCondition, params DbParameter[] cmdParams)
        {
            List<T> list2;
            Table tableOrCreate = TableCache.GetTableOrCreate(typeof (T));
            using (DbCommand command = this.GetSelectCommand(tableOrCreate, whereCondition, cmdParams))
            {
                command.Connection = conn;
                try
                {
                    Trace.WriteLine(command.CommandText);
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        List<ColumnWrapper> sortedColumns = GetSortedColumns<T>(reader);
                        list2 = DataReaderToList<T>(reader, sortedColumns);
                    }
                }
                finally
                {
                    command.Connection = null;
                }
            }
            return list2;
        }

        public IList Select(Type type, DbConnection conn, string whereCondition, params DbParameter[] cmdParams)
        {
            IList list2;
            Table tableOrCreate = TableCache.GetTableOrCreate(type);
            using (DbCommand command = this.GetSelectCommand(tableOrCreate, whereCondition, cmdParams))
            {
                command.Connection = conn;
                try
                {
                    Trace.WriteLine(command.CommandText);
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        List<ColumnWrapper> sortedColumns = GetSortedColumns(type, reader);
                        list2 = DataReaderToList(type, reader, sortedColumns);
                    }
                }
                finally
                {
                    command.Connection = null;
                }
            }
            return list2;
        }

        public virtual void SetDbParameter(DbParameter parameter, string name, object value, int? type)
        {
            SqlParameter parameter2 = parameter as SqlParameter;
            parameter2.ParameterName = name;
            parameter2.Value = value;
            if (type.HasValue)
            {
                parameter2.SqlDbType = (SqlDbType)type.Value;
            }
        }

        public virtual void SetDbParameter(DbParameter parameter, string name, object value, Type type)
        {
            SqlParameter parameter2 = parameter as SqlParameter;
            parameter2.ParameterName = name;
            parameter2.Value = value;
            parameter2.DbType = this.GetDbType(type);
            parameter2.SqlDbType = this.GetSqlDbType(type);
        }

        public virtual void Truncate(DbConnection conn, Type type)
        {
            Table tableOrCreate = TableCache.GetTableOrCreate(type);
            using (DbCommand command = new SqlCommand())
            {
                command.Connection = conn;
                try
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = string.Format("truncate table [{0}]", tableOrCreate.TableName);
                    command.ExecuteNonQuery();
                }
                finally
                {
                    command.Connection = null;
                    command.Transaction = null;
                }
            }
        }

        public void Update(DbConnection conn, object obj)
        {
            Table tableOrCreate = TableCache.GetTableOrCreate(obj.GetType());
            DbCommand updateCommand = this.GetUpdateCommand(tableOrCreate, obj);
            updateCommand.Connection = conn;
            try
            {
                Trace.WriteLine(updateCommand.CommandText);
                updateCommand.ExecuteNonQuery();
            }
            finally
            {
                updateCommand.Connection = null;
            }
        }

        public void Update(DbTransaction trans, object obj)
        {
            Table tableOrCreate = TableCache.GetTableOrCreate(obj.GetType());
            DbCommand updateCommand = this.GetUpdateCommand(tableOrCreate, obj);
            updateCommand.Connection = trans.Connection;
            updateCommand.Transaction = trans;
            try
            {
                Trace.WriteLine(updateCommand.CommandText);
                updateCommand.ExecuteNonQuery();
            }
            finally
            {
                updateCommand.Connection = null;
                updateCommand.Transaction = null;
            }
        }

        public void UpdatePartial(DbConnection conn, Type type, Dictionary<Column, object> keyDict,
            Dictionary<Column, object> valueDict)
        {
            Table tableOrCreate = TableCache.GetTableOrCreate(type);
            DbCommand cmd = this.BuildUpdatePartialCommand(tableOrCreate, keyDict, valueDict);
            this.FormatDbNull(cmd);
            cmd.Connection = conn;
            try
            {
                Trace.WriteLine(cmd.CommandText);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                cmd.Connection = null;
            }
        }

        public void UpdatePartial(DbTransaction trans, Type type, Dictionary<Column, object> keyDict,
            Dictionary<Column, object> valueDict)
        {
            Table tableOrCreate = TableCache.GetTableOrCreate(type);
            DbCommand cmd = this.BuildUpdatePartialCommand(tableOrCreate, keyDict, valueDict);
            this.FormatDbNull(cmd);
            cmd.Connection = trans.Connection;
            cmd.Transaction = trans;
            try
            {
                Trace.WriteLine(cmd.CommandText);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                cmd.Connection = null;
            }
        }
    }
}



