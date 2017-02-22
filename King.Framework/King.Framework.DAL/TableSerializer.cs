using King.Framework.DAL.CSharpTypes;

namespace King.Framework.DAL
{
    using King.Framework.DAL.CSharpTypes;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;

    public class TableSerializer
    {
        private readonly StringBuilder sb = new StringBuilder(0x400);

        private void Append(int val)
        {
            this.sb.Append(val);
        }

        private void Append(string s)
        {
            EncodeAndAppend(this.sb, s);
        }

        private void AppendLine(int val)
        {
            this.sb.Append(val);
            this.sb.Append('\n');
        }

        private void AppendLine(string s)
        {
            EncodeAndAppend(this.sb, s);
            this.sb.Append('\n');
        }

        private void AppendSplitter()
        {
            this.sb.Append(',');
        }

        public void AppendTable(IList list)
        {
            if (list != null)
            {
                Table tableOrCreate = TableCache.GetTableOrCreate(TypeSystem.GetElementType(list.GetType()));
                this.Append(tableOrCreate.TableName);
                this.AppendSplitter();
                this.AppendLine(list.Count);
                int num = -1;
                foreach (Column column in tableOrCreate.KeyColumns)
                {
                    num++;
                    if (num > 0)
                    {
                        this.sb.Append(',');
                    }
                    EncodeAndAppend(this.sb, column.ColumnName);
                }
                Column column2 = tableOrCreate.NonKeyColumns.FirstOrDefault<Column>(p => p.ColumnName == "State");
                foreach (Column column in from p in tableOrCreate.NonKeyColumns
                                          where p.ColumnName != "State"
                                          select p)
                {
                    num++;
                    if (num > 0)
                    {
                        this.sb.Append(',');
                    }
                    EncodeAndAppend(this.sb, column.ColumnName);
                }
                if (column2 != null)
                {
                    num++;
                    if (num > 0)
                    {
                        this.sb.Append(',');
                    }
                    EncodeAndAppend(this.sb, column2.ColumnName);
                }
                this.sb.Append('\n');
                foreach (object obj2 in list)
                {
                    object obj3;
                    num = -1;
                    foreach (Column column in tableOrCreate.KeyColumns)
                    {
                        num++;
                        if (num > 0)
                        {
                            this.sb.Append(',');
                        }
                        obj3 = column.GetFunction(obj2);
                        column.TypeHelper.EncodeInto(this.sb, obj3);
                    }
                    foreach (Column column in from p in tableOrCreate.NonKeyColumns
                                              where p.ColumnName != "State"
                                              select p)
                    {
                        num++;
                        if (num > 0)
                        {
                            this.sb.Append(',');
                        }
                        obj3 = column.GetFunction(obj2);
                        column.TypeHelper.EncodeInto(this.sb, obj3);
                    }
                    if (column2 != null)
                    {
                        num++;
                        if (num > 0)
                        {
                            this.sb.Append(',');
                        }
                        obj3 = column2.GetFunction(obj2);
                        column2.TypeHelper.EncodeInto(this.sb, obj3);
                    }
                    this.sb.Append('\n');
                }
            }
        }

        public void AppendTable(DataTable data, string tableName = "")
        {
            if ((data != null) && (data.Rows.Count > 0))
            {
                List<InternalColumnWrapper> sortedInternalColumnsForMobile = this.GetSortedInternalColumnsForMobile(data);
                if (string.IsNullOrEmpty(tableName))
                {
                    tableName = data.TableName;
                }
                if (string.IsNullOrWhiteSpace(tableName))
                {
                    throw new ApplicationException("tableName==null");
                }
                this.Append(tableName);
                this.AppendSplitter();
                this.AppendLine(data.Rows.Count);
                int num = -1;
                foreach (InternalColumnWrapper wrapper in sortedInternalColumnsForMobile)
                {
                    num++;
                    if (num > 0)
                    {
                        this.sb.Append(',');
                    }
                    EncodeAndAppend(this.sb, wrapper.ColumnName);
                }
                this.sb.Append('\n');
                foreach (DataRow row in data.Rows)
                {
                    num = -1;
                    foreach (InternalColumnWrapper wrapper in sortedInternalColumnsForMobile)
                    {
                        num++;
                        if (num > 0)
                        {
                            this.sb.Append(',');
                        }
                        if (row.IsNull(wrapper.Index))
                        {
                            wrapper.TypeHelper.EncodeInto(this.sb, null);
                        }
                        else
                        {
                            object obj2 = row[wrapper.Index];
                            wrapper.TypeHelper.EncodeInto(this.sb, obj2);
                        }
                    }
                    this.sb.Append('\n');
                }
            }
        }

        public void AppendTable(Type type, IList<DataRow> list)
        {
            if ((list != null) && (list.Count > 0))
            {
                Table tableOrCreate = TableCache.GetTableOrCreate(type);
                List<ColumnWrapper> sortedColumnsForMobile = this.GetSortedColumnsForMobile(type, list[0].Table);
                this.Append(tableOrCreate.TableName);
                this.AppendSplitter();
                this.AppendLine(list.Count);
                int num = -1;
                foreach (ColumnWrapper wrapper in sortedColumnsForMobile)
                {
                    num++;
                    if (num > 0)
                    {
                        this.sb.Append(',');
                    }
                    EncodeAndAppend(this.sb, wrapper.col.ColumnName);
                }
                this.sb.Append('\n');
                foreach (DataRow row in list)
                {
                    num = -1;
                    foreach (ColumnWrapper wrapper in sortedColumnsForMobile)
                    {
                        num++;
                        if (num > 0)
                        {
                            this.sb.Append(',');
                        }
                        if (row.IsNull(wrapper.Index))
                        {
                            wrapper.col.TypeHelper.EncodeInto(this.sb, null);
                        }
                        else
                        {
                            object obj2 = row[wrapper.Index];
                            wrapper.col.TypeHelper.EncodeInto(this.sb, obj2);
                        }
                    }
                    this.sb.Append('\n');
                }
            }
        }

        public void Clean()
        {
            this.sb.Clear();
        }

        internal static void EncodeAndAppend(StringBuilder sb, string s)
        {
            if (s != null)
            {
                foreach (char ch in s)
                {
                    char ch2 = ch;
                    if (ch2 <= '\r')
                    {
                        switch (ch2)
                        {
                            case '\n':
                            case '\r':
                                goto Label_0051;
                        }
                        goto Label_006D;
                    }
                    if (ch2 == ',')
                    {
                        goto Label_005F;
                    }
                    if (ch2 != '\\')
                    {
                        goto Label_006D;
                    }
                    sb.Append(@"\\");
                    continue;
                    Label_0051:
                    sb.Append(@"\n");
                    continue;
                    Label_005F:
                    sb.Append(@"\,");
                    continue;
                    Label_006D:
                    sb.Append(ch);
                }
            }
        }

        private List<ColumnWrapper> GetSortedColumnsForMobile(Type type, DataTable data)
        {
            int num;
            List<ColumnWrapper> list = new List<ColumnWrapper>();
            Table tableOrCreate = TableCache.GetTableOrCreate(type);
            foreach (Column column in tableOrCreate.KeyColumns)
            {
                num = data.Columns.IndexOf(column.ColumnName);
                if (num >= 0)
                {
                    list.Add(new ColumnWrapper(num, column));
                }
            }
            IEnumerable<Column> enumerable = from p in tableOrCreate.NonKeyColumns
                                             where p.ColumnName != "State"
                                             select p;
            foreach (Column column in enumerable)
            {
                num = data.Columns.IndexOf(column.ColumnName);
                if (num >= 0)
                {
                    list.Add(new ColumnWrapper(num, column));
                }
            }
            Column columnByColumnName = tableOrCreate.GetColumnByColumnName("State");
            int index = data.Columns.IndexOf("State");
            if (index >= 0)
            {
                list.Add(new ColumnWrapper(index, columnByColumnName));
            }
            return list;
        }

        private List<ColumnWrapper> GetSortedColumnsForMobile(Type type, IDataReader data)
        {
            int ordinal;
            List<ColumnWrapper> list = new List<ColumnWrapper>();
            Table tableOrCreate = TableCache.GetTableOrCreate(type);
            foreach (Column column in tableOrCreate.KeyColumns)
            {
                ordinal = data.GetOrdinal(column.ColumnName);
                if (ordinal >= 0)
                {
                    list.Add(new ColumnWrapper(ordinal, column));
                }
            }
            IEnumerable<Column> enumerable = from p in tableOrCreate.NonKeyColumns
                                             where p.ColumnName != "State"
                                             select p;
            foreach (Column column in enumerable)
            {
                ordinal = data.GetOrdinal(column.ColumnName);
                if (ordinal >= 0)
                {
                    list.Add(new ColumnWrapper(ordinal, column));
                }
            }
            try
            {
                Column columnByColumnName = tableOrCreate.GetColumnByColumnName("State");
                int index = data.GetOrdinal("State");
                if (index >= 0)
                {
                    list.Add(new ColumnWrapper(index, columnByColumnName));
                }
            }
            catch
            {
            }
            return list;
        }

        internal List<InternalColumnWrapper> GetSortedInternalColumnsForMobile(DataTable data)
        {
            List<InternalColumnWrapper> list = new List<InternalColumnWrapper>(data.Columns.Count);
            for (int i = 0; i < data.Columns.Count; i++)
            {
                DataColumn column = data.Columns[i];
                Type dataType = column.DataType;
                ISharpType shartTypeHelper = SharpTypeHelper.GetShartTypeHelper(dataType);
                list.Add(new InternalColumnWrapper(i, dataType, shartTypeHelper, column.ColumnName));
            }
            return list;
        }

        internal List<InternalColumnWrapper> GetSortedInternalColumnsForMobile(IDataReader data)
        {
            List<InternalColumnWrapper> list = new List<InternalColumnWrapper>(data.FieldCount);
            for (int i = 0; i < data.FieldCount; i++)
            {
                Type fieldType = data.GetFieldType(i);
                ISharpType shartTypeHelper = SharpTypeHelper.GetShartTypeHelper(fieldType);
                list[i] = new InternalColumnWrapper(i, fieldType, shartTypeHelper, data.GetName(i));
            }
            return list;
        }

        public override string ToString()
        {
            return this.sb.ToString();
        }
    }
}
