namespace King.Framework.Linq.Data
{
    using King.Framework.DAL;
    using King.Framework.Linq;
    using King.Framework.Linq.Data.Common;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public class DbEntityProvider : EntityProvider
    {
        private bool actionOpenedConnection;
        private DbConnection connection;
        private IsolationLevel isolation;
        private int nConnectedActions;
        private DbTransaction transaction;

        public DbEntityProvider(DataContext context, QueryLanguage language, QueryMapping mapping, QueryPolicy policy) : base(context, language, mapping, policy)
        {
            this.isolation = IsolationLevel.ReadCommitted;
            this.nConnectedActions = 0;
            this.actionOpenedConnection = false;
            DbConnection connection = context.Connection;
            if (connection == null)
            {
                throw new InvalidOperationException("Connection not specified");
            }
            this.connection = connection;
        }

        protected override QueryExecutor CreateExecutor()
        {
            return new Executor(this);
        }

        public override void DoConnected(Action action)
        {
            this.StartUsingConnection();
            try
            {
                action();
            }
            finally
            {
                this.StopUsingConnection();
            }
        }

        public override void DoTransacted(Action action)
        {
            this.StartUsingConnection();
            try
            {
                if (this.Transaction == null)
                {
                    DbTransaction transaction = this.Connection.BeginTransaction(this.Isolation);
                    try
                    {
                        this.Transaction = transaction;
                        action();
                        transaction.Commit();
                    }
                    finally
                    {
                        this.Transaction = null;
                        transaction.Dispose();
                    }
                }
                else
                {
                    action();
                }
            }
            finally
            {
                this.StopUsingConnection();
            }
        }

        public override int ExecuteCommand(string commandText)
        {
            int num;
            if (base.Log != null)
            {
                base.Log.WriteLine(commandText);
            }
            this.StartUsingConnection();
            try
            {
                DbCommand command = this.Connection.CreateCommand();
                command.CommandText = commandText;
                num = command.ExecuteNonQuery();
            }
            finally
            {
                this.StopUsingConnection();
            }
            return num;
        }

        public static DbEntityProvider From(string connectionString, string mappingId)
        {
            return From(connectionString, mappingId, QueryPolicy.Default);
        }

        public static DbEntityProvider From(string connectionString, QueryMapping mapping, QueryPolicy policy)
        {
            return From((string)null, connectionString, mapping, policy);
        }

        public static DbEntityProvider From(string connectionString, string mappingId, QueryPolicy policy)
        {
            return From(null, connectionString, mappingId, policy);
        }

        public static DbEntityProvider From(string provider, string connectionString, string mappingId)
        {
            return From(provider, connectionString, mappingId, QueryPolicy.Default);
        }

        public static DbEntityProvider From(string provider, string connectionString, QueryMapping mapping, QueryPolicy policy)
        {
            if (provider == null)
            {
                string str = connectionString.ToLower();
                if (!str.Contains(".mdb") && !str.Contains(".accdb"))
                {
                    if (!str.Contains(".sdf"))
                    {
                        if (!str.Contains(".sl3") && !str.Contains(".db3"))
                        {
                            if (!str.Contains(".mdf"))
                            {
                                throw new InvalidOperationException(string.Format("Query provider not specified and cannot be inferred.", new object[0]));
                            }
                            provider = "King.Framework.Linq.Data.SqlClient";
                        }
                        else
                        {
                            provider = "King.Framework.Linq.Data.SQLite";
                        }
                    }
                    else
                    {
                        provider = "King.Framework.Linq.Data.SqlServerCe";
                    }
                }
                else
                {
                    provider = "King.Framework.Linq.Data.Access";
                }
            }
            Type providerType = EntityProvider.GetProviderType(provider);
            if (providerType == null)
            {
                throw new InvalidOperationException(string.Format("Unable to find query provider '{0}'", provider));
            }
            return From(providerType, connectionString, mapping, policy);
        }

        public static DbEntityProvider From(string provider, string connectionString, string mappingId, QueryPolicy policy)
        {
            return From(provider, connectionString, EntityProvider.GetMapping(mappingId), policy);
        }

        public static DbEntityProvider From(Type providerType, string connectionString, QueryMapping mapping, QueryPolicy policy)
        {
            Type adoConnectionType = GetAdoConnectionType(providerType);
            if (adoConnectionType == null)
            {
                throw new InvalidOperationException(string.Format("Unable to deduce ADO provider for '{0}'", providerType.Name));
            }
            DbConnection connection = (DbConnection)Activator.CreateInstance(adoConnectionType);
            if (!connectionString.Contains<char>('='))
            {
                MethodInfo info = providerType.GetMethod("GetConnectionString", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null);
                if (info != null)
                {
                    connectionString = (string)info.Invoke(null, new object[] { connectionString });
                }
            }
            connection.ConnectionString = connectionString;
            return (DbEntityProvider)Activator.CreateInstance(providerType, new object[] { connection, mapping, policy });
        }

        public static DbEntityProvider FromApplicationSettings()
        {
            string provider = ConfigurationManager.AppSettings["Provider"];
            string connectionString = ConfigurationManager.AppSettings["Connection"];
            string mappingId = ConfigurationManager.AppSettings["Mapping"];
            return From(provider, connectionString, mappingId);
        }

        private static Type GetAdoConnectionType(Type providerType)
        {
            foreach (ConstructorInfo info in providerType.GetConstructors())
            {
                foreach (ParameterInfo info2 in info.GetParameters())
                {
                    if (info2.ParameterType.IsSubclassOf(typeof(DbConnection)))
                    {
                        return info2.ParameterType;
                    }
                }
            }
            return null;
        }

        public virtual DbEntityProvider New(QueryMapping mapping)
        {
            DbEntityProvider provider = this.New(this.Connection, mapping, base.Policy);
            provider.Log = base.Log;
            return provider;
        }

        public virtual DbEntityProvider New(QueryPolicy policy)
        {
            DbEntityProvider provider = this.New(this.Connection, base.Mapping, policy);
            provider.Log = base.Log;
            return provider;
        }

        public virtual DbEntityProvider New(DbConnection connection)
        {
            DbEntityProvider provider = this.New(connection, base.Mapping, base.Policy);
            provider.Log = base.Log;
            return provider;
        }

        public virtual DbEntityProvider New(DbConnection connection, QueryMapping mapping, QueryPolicy policy)
        {
            return (DbEntityProvider)Activator.CreateInstance(base.GetType(), new object[] { connection, mapping, policy });
        }

        protected void StartUsingConnection()
        {
            if (this.connection.State == ConnectionState.Closed)
            {
                this.connection.Open();
                this.actionOpenedConnection = true;
            }
            this.nConnectedActions++;
        }

        protected void StopUsingConnection()
        {
            System.Diagnostics.Debug.Assert(this.nConnectedActions > 0);
            this.nConnectedActions--;
            if ((this.nConnectedActions == 0) && this.actionOpenedConnection)
            {
                this.connection.Close();
                this.actionOpenedConnection = false;
            }
        }

        protected bool ActionOpenedConnection
        {
            get
            {
                return this.actionOpenedConnection;
            }
        }

        public virtual DbConnection Connection
        {
            get
            {
                return this.connection;
            }
        }

        public IsolationLevel Isolation
        {
            get
            {
                return this.isolation;
            }
            set
            {
                this.isolation = value;
            }
        }

        public virtual DbTransaction Transaction
        {
            get
            {
                return this.transaction;
            }
            set
            {
                if ((value != null) && (value.Connection != this.connection))
                {
                    throw new InvalidOperationException("Transaction does not match connection.");
                }
                this.transaction = value;
            }
        }

        protected class DbFieldReader : FieldReader
        {
            private QueryExecutor executor;
            private DbDataReader reader;

            public DbFieldReader(QueryExecutor executor, DbDataReader reader)
            {
                this.executor = executor;
                this.reader = reader;
                base.Init();
            }

            protected override byte GetByte(int ordinal)
            {
                return this.reader.GetByte(ordinal);
            }

            protected override char GetChar(int ordinal)
            {
                return this.reader.GetChar(ordinal);
            }

            protected override DateTime GetDateTime(int ordinal)
            {
                return this.reader.GetDateTime(ordinal);
            }

            protected override decimal GetDecimal(int ordinal)
            {
                return this.reader.GetDecimal(ordinal);
            }

            protected override double GetDouble(int ordinal)
            {
                return this.reader.GetDouble(ordinal);
            }

            protected override Type GetFieldType(int ordinal)
            {
                return this.reader.GetFieldType(ordinal);
            }

            protected override Guid GetGuid(int ordinal)
            {
                return this.reader.GetGuid(ordinal);
            }

            protected override short GetInt16(int ordinal)
            {
                return this.reader.GetInt16(ordinal);
            }

            protected override int GetInt32(int ordinal)
            {
                return this.reader.GetInt32(ordinal);
            }

            protected override long GetInt64(int ordinal)
            {
                return this.reader.GetInt64(ordinal);
            }

            protected override float GetSingle(int ordinal)
            {
                return this.reader.GetFloat(ordinal);
            }

            protected override string GetString(int ordinal)
            {
                return this.reader.GetString(ordinal);
            }

            protected override T GetValue<T>(int ordinal)
            {
                return (T)this.executor.Convert(this.reader.GetValue(ordinal), typeof(T));
            }

            protected override bool IsDBNull(int ordinal)
            {
                return this.reader.IsDBNull(ordinal);
            }

            protected override int FieldCount
            {
                get
                {
                    return this.reader.FieldCount;
                }
            }
        }

        public class Executor : QueryExecutor
        {
            private DbEntityProvider provider;
            private int rowsAffected;

            public Executor(DbEntityProvider provider)
            {
                this.provider = provider;
            }

            protected virtual void AddParameter(DbCommand command, QueryParameter parameter, object value)
            {
                DbParameter parameter2 = command.CreateParameter();
                parameter2.ParameterName = parameter.Name;
                parameter2.Value = value ?? DBNull.Value;
                command.Parameters.Add(parameter2);
            }

            public override object Convert(object value, Type type)
            {
                if (value == null)
                {
                    return King.Framework.Linq.TypeHelper.GetDefault(type);
                }
                type = King.Framework.Linq.TypeHelper.GetNonNullableType(type);
                Type type2 = value.GetType();
                if (!(type != type2))
                {
                    return value;
                }
                if (type.IsEnum)
                {
                    if (type2 == typeof(string))
                    {
                        return Enum.Parse(type, (string)value);
                    }
                    Type underlyingType = Enum.GetUnderlyingType(type);
                    if (underlyingType != type2)
                    {
                        value = System.Convert.ChangeType(value, underlyingType);
                    }
                    return Enum.ToObject(type, value);
                }
                return King.Framework.DAL.SharpTypeHelper.ConvertFrom(type, value);
            }

            public override IEnumerable<T> Execute<T>(QueryCommand command, Func<FieldReader, T> fnProjector, MappingEntity entity, object[] paramValues)
            {
                IEnumerable<T> enumerable2;
                this.LogCommand(command, paramValues);
                this.StartUsingConnection();
                try
                {
                    DbCommand command2 = this.GetCommand(command, paramValues);
                    using (DbDataReader reader = this.ExecuteReader(command2))
                    {
                        IEnumerable<T> source = this.Project<T>(reader, fnProjector, entity, true);
                        if (this.provider.ActionOpenedConnection)
                        {
                            source = source.ToList<T>();
                        }
                        else
                        {
                            source = new EnumerateOnce<T>(source).ToList<T>();
                        }
                        enumerable2 = source;
                    }
                }
                finally
                {
                    this.StopUsingConnection();
                }
                return enumerable2;
            }

            private IEnumerable<int> ExecuteBatch(QueryCommand query, IEnumerable<object[]> paramSets)
            {
                this.LogCommand(query, null);
                DbCommand command = this.GetCommand(query, null);
                foreach (object[] iteratorVariable1 in paramSets)
                {
                    this.LogParameters(query, iteratorVariable1);
                    this.LogMessage("");
                    this.SetParameterValues(query, command, iteratorVariable1);
                    this.rowsAffected = command.ExecuteNonQuery();
                    yield return this.rowsAffected;
                }
            }

            private IEnumerable<T> ExecuteBatch<T>(QueryCommand query, IEnumerable<object[]> paramSets, Func<FieldReader, T> fnProjector, MappingEntity entity)
            {
                this.LogCommand(query, null);
                DbCommand command = this.GetCommand(query, null);
                command.Prepare();
                List<T> list = new List<T>();
                foreach (object[] objArray in paramSets)
                {
                    this.LogParameters(query, objArray);
                    this.LogMessage("");
                    this.SetParameterValues(query, command, objArray);
                    DbDataReader reader = this.ExecuteReader(command);
                    try
                    {
                        DbEntityProvider.DbFieldReader arg = new DbEntityProvider.DbFieldReader(this, reader);
                        while (reader.Read())
                        {
                            list.Add(fnProjector(arg));
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
                return list;
            }

            public override IEnumerable<int> ExecuteBatch(QueryCommand query, IEnumerable<object[]> paramSets, int batchSize, bool stream)
            {
                IEnumerable<int> enumerable2;
                this.StartUsingConnection();
                try
                {
                    IEnumerable<int> source = this.ExecuteBatch(query, paramSets);
                    if (!(stream && !this.ActionOpenedConnection))
                    {
                        return source.ToList<int>();
                    }
                    enumerable2 = new EnumerateOnce<int>(source);
                }
                finally
                {
                    this.StopUsingConnection();
                }
                return enumerable2;
            }

            public override IEnumerable<T> ExecuteBatch<T>(QueryCommand query, IEnumerable<object[]> paramSets, Func<FieldReader, T> fnProjector, MappingEntity entity, int batchSize, bool stream)
            {
                IEnumerable<T> enumerable2;
                this.StartUsingConnection();
                try
                {
                    IEnumerable<T> source = this.ExecuteBatch<T>(query, paramSets, fnProjector, entity);
                    if (!(stream && !this.ActionOpenedConnection))
                    {
                        return source.ToList<T>();
                    }
                    enumerable2 = new EnumerateOnce<T>(source);
                }
                finally
                {
                    this.StopUsingConnection();
                }
                return enumerable2;
            }

            public override int ExecuteCommand(QueryCommand query, object[] paramValues)
            {
                int rowsAffected;
                this.LogCommand(query, paramValues);
                this.StartUsingConnection();
                try
                {
                    this.rowsAffected = this.GetCommand(query, paramValues).ExecuteNonQuery();
                    rowsAffected = this.rowsAffected;
                }
                finally
                {
                    this.StopUsingConnection();
                }
                return rowsAffected;
            }

            public override IEnumerable<T> ExecuteDeferred<T>(QueryCommand query, Func<FieldReader, T> fnProjector, MappingEntity entity, object[] paramValues)
            {
                IEnumerable<T> enumerable;
                this.LogCommand(query, paramValues);
                this.StartUsingConnection();
                try
                {
                    List<T> list = new List<T>();
                    DbCommand command = this.GetCommand(query, paramValues);
                    using (DbDataReader reader = this.ExecuteReader(command))
                    {
                        DbEntityProvider.DbFieldReader arg = new DbEntityProvider.DbFieldReader(this, reader);
                        while (reader.Read())
                        {
                            list.Add(fnProjector(arg));
                        }
                    }
                    enumerable = list;
                }
                finally
                {
                    this.StopUsingConnection();
                }
                return enumerable;
            }

            protected virtual DbDataReader ExecuteReader(DbCommand command)
            {
                return command.ExecuteReader();
            }

            protected virtual DbCommand GetCommand(QueryCommand query, object[] paramValues)
            {
                DbCommand command = this.provider.Connection.CreateCommand();
                command.CommandText = query.CommandText;
                if (this.provider.Transaction != null)
                {
                    command.Transaction = this.provider.Transaction;
                }
                this.SetParameterValues(query, command, paramValues);
                return command;
            }

            protected virtual void GetParameterValues(DbCommand command, object[] paramValues)
            {
                if (paramValues != null)
                {
                    int index = 0;
                    int count = command.Parameters.Count;
                    while (index < count)
                    {
                        if (command.Parameters[index].Direction != ParameterDirection.Input)
                        {
                            object obj2 = command.Parameters[index].Value;
                            if (obj2 == DBNull.Value)
                            {
                                obj2 = null;
                            }
                            paramValues[index] = obj2;
                        }
                        index++;
                    }
                }
            }

            protected virtual void LogCommand(QueryCommand command, object[] paramValues)
            {
                if (this.provider.Log != null)
                {
                    this.provider.Log.WriteLine(command.CommandText);
                    if (paramValues != null)
                    {
                        this.LogParameters(command, paramValues);
                    }
                    this.provider.Log.WriteLine();
                }
            }

            protected virtual void LogMessage(string message)
            {
                if (this.provider.Log != null)
                {
                    this.provider.Log.WriteLine(message);
                }
            }

            protected virtual void LogParameters(QueryCommand command, object[] paramValues)
            {
                if ((this.provider.Log != null) && (paramValues != null))
                {
                    int index = 0;
                    int count = command.Parameters.Count;
                    while (index < count)
                    {
                        QueryParameter parameter = command.Parameters[index];
                        object obj2 = paramValues[index];
                        if ((obj2 == null) || (obj2 == DBNull.Value))
                        {
                            this.provider.Log.WriteLine("-- {0} = NULL", parameter.Name);
                        }
                        else
                        {
                            this.provider.Log.WriteLine("-- {0} = [{1}]", parameter.Name, obj2);
                        }
                        index++;
                    }
                }
            }

            protected virtual IEnumerable<T> Project<T>(DbDataReader reader, Func<FieldReader, T> fnProjector, MappingEntity entity, bool closeReader)
            {
                DbEntityProvider.DbFieldReader arg = new DbEntityProvider.DbFieldReader(this, reader);
                while (reader.Read())
                {
                    yield return fnProjector(arg);
                }
            }

            protected virtual void SetParameterValues(QueryCommand query, DbCommand command, object[] paramValues)
            {
                int num;
                int count;
                if ((query.Parameters.Count > 0) && (command.Parameters.Count == 0))
                {
                    num = 0;
                    count = query.Parameters.Count;
                    while (num < count)
                    {
                        this.AddParameter(command, query.Parameters[num], (paramValues != null) ? paramValues[num] : null);
                        num++;
                    }
                }
                else if (paramValues != null)
                {
                    num = 0;
                    count = command.Parameters.Count;
                    while (num < count)
                    {
                        DbParameter parameter = command.Parameters[num];
                        if ((parameter.Direction == ParameterDirection.Input) || (parameter.Direction == ParameterDirection.InputOutput))
                        {
                            parameter.Value = paramValues[num] ?? DBNull.Value;
                        }
                        num++;
                    }
                }
            }

            protected void StartUsingConnection()
            {
                this.provider.StartUsingConnection();
            }

            protected void StopUsingConnection()
            {
                this.provider.StopUsingConnection();
            }

            protected bool ActionOpenedConnection
            {
                get
                {
                    return this.provider.actionOpenedConnection;
                }
            }

            protected virtual bool BufferResultRows
            {
                get
                {
                    return false;
                }
            }

            public DbEntityProvider Provider
            {
                get
                {
                    return this.provider;
                }
            }

            public override int RowsAffected
            {
                get
                {
                    return this.rowsAffected;
                }
            }

    }
}
    }

