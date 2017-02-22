namespace King.Framework.DAL.DataAccessLib
{
    using King.Framework.DAL;
    using Oracle.ManagedDataAccess.Client;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Transactions;

    public class OracleDataAccess : BaseDataAccess
    {
        public override void BatchInsert<T>(DbConnection conn, IEnumerable<T> objList)
        {
            foreach (T local in objList)
            {
                base.Insert(conn, local);
            }
        }

        public override void BatchInsert<T>(DbTransaction trans, IEnumerable<T> objList)
        {
            foreach (T local in objList)
            {
                base.Insert(trans, local);
            }
        }

        public override void BatchInsert(DbConnection conn, ICollection objList, Type type)
        {
            foreach (object obj2 in objList)
            {
                base.Insert(conn, obj2);
            }
        }

        public override void BatchInsert(DbTransaction trans, ICollection objList, Type type)
        {
            foreach (object obj2 in objList)
            {
                base.Insert(trans, obj2);
            }
        }

        protected override DbCommand BuildDeleteCommand(Table t)
        {
            OracleCommand command = new OracleCommand();
            foreach (Column column in t.KeyColumns)
            {
                string str = ":" + column.ColumnName;
                OracleParameter param = new OracleParameter
                {
                    ParameterName = str
                };
                command.Parameters.Add(param);
            }
            command.CommandType = CommandType.Text;
            command.CommandText = t.Delete_Oracle;
            return command;
        }

        protected override DbCommand BuildFillAndLockCommand(Table t, int? wait)
        {
            OracleCommand command = new OracleCommand();
            foreach (Column column in t.KeyColumns)
            {
                string str = ":" + column.ColumnName;
                OracleParameter param = new OracleParameter
                {
                    ParameterName = str
                };
                command.Parameters.Add(param);
            }
            command.CommandType = CommandType.Text;
            if (!wait.HasValue)
            {
                command.CommandText = t.Fill_ForUpdate_Oracle;
                return command;
            }
            if (wait > 0)
            {
                command.CommandText = string.Format("{0} wait {1}", t.Fill_ForUpdate_Oracle, wait);
                return command;
            }
            command.CommandText = string.Format("{0} nowait", t.Fill_ForUpdate_Oracle);
            return command;
        }

        protected override DbCommand BuildFillCommand(Table t)
        {
            OracleCommand command = new OracleCommand();
            foreach (Column column in t.KeyColumns)
            {
                string str = ":" + column.ColumnName;
                OracleParameter param = new OracleParameter
                {
                    ParameterName = str
                };
                command.Parameters.Add(param);
            }
            command.CommandType = CommandType.Text;
            command.CommandText = t.Fill_Oracle;
            return command;
        }

        protected override DbCommand BuildInsertCommand(Table t)
        {
            OracleCommand command = new OracleCommand();
            foreach (Column column in t.NonIdentityColumns)
            {
                string str = ":" + column.ColumnName;
                OracleParameter param = new OracleParameter
                {
                    ParameterName = str,
                    Direction = ParameterDirection.Input,
                    OracleDbType = base.GetOracleType(column.DataType)
                };
                command.Parameters.Add(param);
            }
            command.CommandType = CommandType.Text;
            command.CommandText = t.Insert_Oracle;
            return command;
        }

        protected override DbCommand BuildSelectCommand(Table t)
        {
            return new OracleCommand { CommandType = CommandType.Text, CommandText = t.Select_Oracle };
        }

        protected override DbCommand BuildUpdateCommand(Table t)
        {
            string str;
            OracleCommand command = new OracleCommand();
            foreach (Column column in t.NonKeyColumns)
            {
                str = ":" + column.ColumnName;
                OracleParameter param = new OracleParameter
                {
                    ParameterName = str,
                    Direction = ParameterDirection.Input,
                    OracleDbType = base.GetOracleType(column.DataType)
                };
                command.Parameters.Add(param);
            }
            foreach (Column column in t.KeyColumns)
            {
                str = ":" + column.ColumnName;
                OracleParameter parameter2 = new OracleParameter
                {
                    ParameterName = str,
                    Direction = ParameterDirection.Input,
                    OracleDbType = base.GetOracleType(column.DataType)
                };
                command.Parameters.Add(parameter2);
            }
            command.CommandType = CommandType.Text;
            command.CommandText = t.Update_Oracle;
            return command;
        }

        protected override DbCommand BuildUpdatePartialCommand(Table t, Dictionary<Column, object> keyDict, Dictionary<Column, object> valueDict)
        {
            string str;
            OracleCommand command = new OracleCommand();
            StringBuilder builder = new StringBuilder(0x400);
            builder.AppendFormat("update {0} set ", t.TableName);
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
                    str = ":" + column.ColumnName;
                    builder.AppendFormat("{0} = {1}", column.ColumnName, str);
                    OracleParameter param = new OracleParameter
                    {
                        ParameterName = str,
                        Direction = ParameterDirection.Input,
                        OracleDbType = base.GetOracleType(column.DataType),
                        Value = obj2
                    };
                    command.Parameters.Add(param);
                    num++;
                }
            }
            builder.Append(" where ");
            num = 0;
            foreach (Column column in keyDict.Keys)
            {
                if (num > 0)
                {
                    builder.Append(" and ");
                }
                str = ":" + column.ColumnName;
                builder.AppendFormat("{0} = {1}", column.ColumnName, str);
                OracleParameter parameter2 = new OracleParameter
                {
                    ParameterName = str,
                    DbType = base.GetDbType(column.DataType),
                    Direction = ParameterDirection.Input,
                    OracleDbType = base.GetOracleType(column.DataType),
                    Value = keyDict[column]
                };
                command.Parameters.Add(parameter2);
                num++;
            }
            command.CommandType = CommandType.Text;
            command.CommandText = builder.ToString();
            return command;
        }

        public override DbDataAdapter CreateDataAdapter()
        {
            return new OracleDataAdapter();
        }

        public override DbParameter CreateParameter()
        {
            return new OracleParameter();
        }

        public override string CreateParameterName(string nameWithOutPrefix)
        {
            return string.Format(":{0}", nameWithOutPrefix);
        }

        public override void CreateTable(DbConnection conn, Type type)
        {
            Table tableOrCreate = TableCache.GetTableOrCreate(type);
            StringBuilder builder = new StringBuilder(0x800);
            builder.AppendFormat("CREATE TABLE {0}(", tableOrCreate.TableName);
            foreach (Column column in tableOrCreate.Columns)
            {
                builder.AppendFormat(" {0} {1} {2} NULL, ", column.ColumnName, column.GetOracleTypeString(), column.IsPrimaryKey ? "NOT" : string.Empty);
            }
            string tableName = tableOrCreate.TableName;
            if (tableOrCreate.TableName.Length > 0x19)
            {
                tableName = tableOrCreate.TableName.Substring(0, 0x19);
            }
            builder.AppendFormat(" CONSTRAINT PK_{0} PRIMARY KEY ( ", tableName);
            int num = -1;
            foreach (Column column in tableOrCreate.KeyColumns)
            {
                num++;
                if (num > 0)
                {
                    builder.Append(",");
                }
                builder.AppendFormat(" {0} ", column.ColumnName);
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

        public override void DeleteAll(DbConnection conn, Type type)
        {
            Table tableOrCreate = TableCache.GetTableOrCreate(type);
            DbCommand command = new OracleCommand(string.Format("delete from {0}", tableOrCreate.TableName))
            {
                Connection =(OracleConnection) conn
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

        public override void DeleteAll(DbTransaction trans, Type type)
        {
            Table tableOrCreate = TableCache.GetTableOrCreate(type);
            DbCommand command = new OracleCommand(string.Format("delete from {0}", tableOrCreate.TableName))
            {
                Transaction = (OracleTransaction)trans,
                Connection = (OracleConnection)trans.Connection
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

        public override bool ExistTable(DbConnection conn, Type type)
        {
            Table tableOrCreate = TableCache.GetTableOrCreate(type);
            string sql = string.Format("select count(*) from user_tables where table_name = '{0}'", tableOrCreate.TableName.ToUpper());
            return (Convert.ToInt32(base.ExecuteScalar(conn, null, sql, new DbParameter[0])) > 0);
        }

        public override IList FilterWithOneField(DbConnection conn, Type type, string columnName, object value)
        {
            return base.FilterWithOneField(conn, type, columnName, value);
        }

        protected override void FormatDbNull(DbCommand cmd)
        {
            foreach (DbParameter parameter in cmd.Parameters)
            {
                if (parameter.Value == null)
                {
                    OracleParameter parameter2 = parameter as OracleParameter;
                    if (parameter2.OracleDbType != OracleDbType.Blob)
                    {
                        parameter2.OracleDbType = OracleDbType.NVarchar2;
                    }
                    parameter.Value = DBNull.Value;
                    parameter.Size = 0;
                }
            }
        }

        public override int GetNextIdentity_Int(DbProviderFactory factory, string connStr, string key, bool rollBack = false)
        {
            object obj2 = -1;
            Exception exception = null;
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
                this.SetDbParameter(parameter, this.CreateParameterName("Key"), key, 0x77);
                string sql = string.Format("select CurrentId from SysIdSequence where IdKey = {0} for update wait 100", parameter.ParameterName);
                try
                {
                    DbParameter parameter4;
                    DbParameter parameter2 = this.CreateParameter();
                    this.SetDbParameter(parameter2, this.CreateParameterName("Key"), key, 0x77);
                    DbParameter parameter3 = this.CreateParameter();
                    this.SetDbParameter(parameter3, this.CreateParameterName("UpdateTime"), DateTime.Now, 0x7b);
                    object obj3 = base.ExecuteScalar(connection, null, sql, new DbParameter[] { parameter });
                    if (obj3 == null)
                    {
                        obj2 = 1;
                        parameter4 = this.CreateParameter();
                        this.SetDbParameter(parameter4, this.CreateParameterName("CurrentId"), obj2, 0x70);
                        sql = string.Format("insert into SysIdSequence (IdKey,CurrentId,UpdateTime) values ({0},{1},{2})", parameter2.ParameterName, parameter4.ParameterName, parameter3.ParameterName);
                        base.ExecuteNonQuery(connection, null, sql, new DbParameter[] { parameter2, parameter4, parameter3 });
                    }
                    else
                    {
                        obj2 = Convert.ToInt32(obj3) + 1;
                        parameter4 = this.CreateParameter();
                        this.SetDbParameter(parameter4, this.CreateParameterName("CurrentId"), obj2, 0x70);
                        sql = string.Format("update SysIdSequence Set CurrentId = {1},UpdateTime = {2} where IdKey = {0}", parameter2.ParameterName, parameter4.ParameterName, parameter3.ParameterName);
                        base.ExecuteNonQuery(connection, null, sql, new DbParameter[] { parameter4, parameter3, parameter2 });
                    }
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                }
                connection.Close();
            }
            if (!rollBack)
            {
                scope.Complete();
                scope.Dispose();
            }
            if (exception != null)
            {
                throw exception;
            }
            return Convert.ToInt32(obj2);
        }

        public override int GetNextIdentity_Int_Old(DbProviderFactory factory, string connStr)
        {
            object obj2;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                using (DbConnection connection = factory.CreateConnection())
                {
                    connection.ConnectionString = connStr;
                    connection.Open();
                    string sql = "select T_EFSEQUECE_SEQUENCE.nextval from dual";
                    obj2 = base.ExecuteScalar(connection, null, sql, new DbParameter[0]);
                }
                scope.Complete();
            }
            return Convert.ToInt32(obj2);
        }

        public override long GetNextIdentity_Old(DbProviderFactory factory, string connStr)
        {
            object obj2;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                using (DbConnection connection = factory.CreateConnection())
                {
                    connection.ConnectionString = connStr;
                    connection.Open();
                    string sql = "select T_EFSEQUECE_SEQUENCE.nextval from dual";
                    obj2 = base.ExecuteScalar(connection, null, sql, new DbParameter[0]);
                }
                scope.Complete();
            }
            return Convert.ToInt64(obj2);
        }

        public override List<ColumnWrapper> GetSortedColumns(DbConnection conn, Type type)
        {
            List<ColumnWrapper> sortedColumns;
            Table tableOrCreate = TableCache.GetTableOrCreate(type);
            string str = string.Format("select * from {0} where rownum = 1", tableOrCreate.TableName);
            using (DbCommand command = conn.CreateCommand())
            {
                command.CommandText = str;
                command.CommandType = CommandType.Text;
                try
                {
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        sortedColumns = BaseDataAccess.GetSortedColumns(type, reader);
                    }
                }
                finally
                {
                    command.Connection = null;
                }
            }
            return sortedColumns;
        }

        public override void SetDbParameter(DbParameter parameter, string name, object value, int? type)
        {
            OracleParameter parameter2 = parameter as OracleParameter;
            parameter2.ParameterName = name;
            parameter2.Value = value;
            if (type.HasValue)
            {
                parameter2.OracleDbType =(OracleDbType) type.Value;
            }
        }

        public override void SetDbParameter(DbParameter parameter, string name, object value, Type type)
        {
            OracleParameter parameter2 = parameter as OracleParameter;
            parameter2.ParameterName = name;
            parameter2.Value = value;
            parameter2.OracleDbType = base.GetOracleType(type);
        }

        public override void Truncate(DbConnection conn, Type type)
        {
            Table tableOrCreate = TableCache.GetTableOrCreate(type);
            DbCommand command = new OracleCommand
            {
                Connection =(OracleConnection) conn
            };
            try
            {
                command.CommandType = CommandType.Text;
                command.CommandText = string.Format("truncate table {0}", tableOrCreate.TableName);
                command.ExecuteNonQuery();
            }
            finally
            {
                command.Connection = null;
                command.Transaction = null;
            }
        }
    }
}

