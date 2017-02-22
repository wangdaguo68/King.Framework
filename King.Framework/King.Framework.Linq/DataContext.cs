namespace King.Framework.Linq
{
    using King.Framework.DAL;
    using King.Framework.DAL.DataAccessLib;
    using King.Framework.Linq.Data.Common;
    using King.Framework.Linq.Data.ODP;
    using King.Framework.Linq.Data.SqlClient;
    using King.Framework.LiteQueryDef;
    using King.Framework.LiteQueryDef.Internal;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;

    public class DataContext : IDisposable
    {
        private DbConnection _connection;
        private string _connectionString;
        private BaseDataAccess _dataAccess;
        private readonly DatabaseTypeEnum _databaseType;
        private static readonly KingMapping _mapping = new KingMapping();
        internal readonly QueryProvider _provider;
        private readonly DbProviderFactory _providerFactory;
        private readonly string _providerInvariantName;
        [ThreadStatic]
        private static ConnectionInfo _sessionConnectionInfo;
        private DbTransaction _transaction;
        public static readonly string BizConnectionStringDefault = "Meta_Biz";
        public static readonly string IdentityConnectionStringDefault = "Identity_Biz";
        public static readonly string Oracle_DataAccess_Client_Provider = "Oracle.DataAccess.Client";
        public static readonly string Oracle_ManagedDataAccess_Client_Provider = "Oracle.ManagedDataAccess.Client";
        public static readonly string System_Data_SqlClient_Provider = "System.Data.SqlClient";

        public DataContext(bool openConnection = false) : this(CurrentConnectionInfo, openConnection)
        {
        }

        public DataContext(ConnectionInfo connInfo, bool openConnection = false) : this(connInfo.ConnectionString, connInfo.ProviderName, openConnection)
        {
        }

        public DataContext(string connectionStringName, bool openConnection = false) : this(ConnectionInfo.FromConfig(connectionStringName, false), openConnection)
        {
        }

        public DataContext(string connectionString, string providerInvariantName, bool openConnection = false)
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[connectionString];
            if (settings != null)
            {
                if (!string.IsNullOrWhiteSpace(settings.ConnectionString))
                {
                    this._connectionString = settings.ConnectionString;
                }
                else
                {
                    this._connectionString = connectionString;
                }
                this._providerInvariantName = settings.ProviderName;
            }
            else
            {
                this._connectionString = connectionString;
                this._providerInvariantName = providerInvariantName;
            }
            this._databaseType = DatabaseFactory.GetDatabaseType(this._providerInvariantName);
            this._dataAccess = DatabaseFactory.CreateDataAccess(this._databaseType);
            this._providerFactory = DbProviderFactories.GetFactory(this._providerInvariantName);
            this._connection = this._providerFactory.CreateConnection();
            this._connection.ConnectionString = this._connectionString;
            if (this._databaseType == DatabaseTypeEnum.Oracle)
            {
                this._provider = new ODPQueryProvider(this, _mapping, QueryPolicy.Default);
            }
            else
            {
                this._provider = new SqlQueryProvider(this, _mapping, QueryPolicy.Default);
            }
            if (openConnection)
            {
                this.Open();
            }
        }

        public string AddPrefixToParameterName(string nameWithOutPrefix)
        {
            return this._dataAccess.CreateParameterName(nameWithOutPrefix);
        }

        public virtual void BatchInsert<T>(IEnumerable<T> objList)
        {
            this._dataAccess.BatchInsert<T>(this._connection, objList);
        }

        public virtual void BatchInsert(Type type, ICollection objList)
        {
            this._dataAccess.BatchInsert(this._connection, objList, type);
        }

        public void BeginTrans()
        {
            if (this._transaction != null)
            {
                throw new ApplicationException("已启用事务");
            }
            this._transaction = this._connection.BeginTransaction();
        }

        public void Close()
        {
            if (this._connection.State != ConnectionState.Closed)
            {
                this._connection.Close();
            }
        }

        public void CommitTrans()
        {
            if (this._transaction == null)
            {
                throw new ApplicationException("未启用事务");
            }
            this._transaction.Commit();
            this._transaction = null;
        }

        public DbDataAdapter CreateDataAdapter()
        {
            return this._dataAccess.CreateDataAdapter();
        }

        public DbParameter CreateParameter()
        {
            return this._dataAccess.CreateParameter();
        }

        public DbParameter CreateParameter(string name, object value, int? type = new int?())
        {
            DbParameter parameter = this.CreateParameter();
            this._dataAccess.SetDbParameter(parameter, name, value, type);
            return parameter;
        }

        public DbParameter CreateParameter(string name, object value, Type type)
        {
            DbParameter parameter = this.CreateParameter();
            this._dataAccess.SetDbParameter(parameter, name, value, type);
            return parameter;
        }

        public DbParameter CreateParameterWithPrefix(string name, object value, int? type = new int?())
        {
            return this.CreateParameter(this.AddPrefixToParameterName(name), value, type);
        }

        public DbParameter CreateParameterWithPrefix(string name, object value, Type type)
        {
            return this.CreateParameter(this.AddPrefixToParameterName(name), value, type);
        }

        public void CreateTable(Type type)
        {
            if (!this._dataAccess.ExistTable(this._connection, type))
            {
                this._dataAccess.CreateTable(this._connection, type);
            }
        }

        public List<T> DataTableToList<T>(DataTable data)
        {
            List<ColumnWrapper> sortedColumns = BaseDataAccess.GetSortedColumns<T>(data);
            return BaseDataAccess.DataTableToList<T>(data, sortedColumns);
        }

        public IList DataTableToList(Type type, DataTable data)
        {
            List<ColumnWrapper> sortedColumns = BaseDataAccess.GetSortedColumns(type, data);
            return BaseDataAccess.DataTableToList(type, data, sortedColumns);
        }

        public virtual void Delete(object obj)
        {
            if (this._transaction != null)
            {
                this._dataAccess.Delete(this._transaction, obj);
            }
            else
            {
                this._dataAccess.Delete(this._connection, obj);
            }
        }

        public void DeleteAll<T>()
        {
            this.DeleteAll(typeof(T));
        }

        public void DeleteAll(Type type)
        {
            if (this._transaction != null)
            {
                this._dataAccess.DeleteAll(this._transaction, type);
            }
            else
            {
                this._dataAccess.DeleteAll(this._connection, type);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._transaction != null)
                {
                    this._transaction.Dispose();
                    this._transaction = null;
                }
                if (this._connection != null)
                {
                    if (this._connection.State == ConnectionState.Open)
                    {
                        this._connection.Dispose();
                    }
                    this._connection = null;
                }
                this._connectionString = null;
            }
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
        }

        public DataTable ExecuteDataTable(string sql, params DbParameter[] paramArray)
        {
            Trace.WriteLine(sql);
            return this._dataAccess.ExecuteDataTable(this._connection, this._transaction, sql, paramArray);
        }

        public async Task<DataTable> ExecuteDataTableAsync(string sql, params DbParameter[] paramArray)
        {
            Trace.WriteLine(sql);
            return await this._dataAccess.ExecuteDataTableAsync(this._connection, this._transaction, sql, paramArray);
        }

        public DataTable ExecuteLiteQuery(LiteQuery q)
        {
            string sql = DatabaseFactory.GetLiteVisitor(this._databaseType, q).Visit();
            return this.ExecuteDataTable(sql, new DbParameter[0]);
        }

        public int ExecuteNonQuery(string sql, params DbParameter[] paramArray)
        {
            return this._dataAccess.ExecuteNonQuery(this._connection, this._transaction, sql, paramArray);
        }

        public async Task<int> ExecuteNonQueryAsync(string sql, params DbParameter[] paramArray)
        {
            return await this._dataAccess.ExecuteNonQueryAsync(this._connection, this._transaction, sql, paramArray);
        }

        public List<T> ExecuteObjectSet<T>(string sql, params DbParameter[] paramArray)
        {
            Trace.WriteLine(sql);
            return this._dataAccess.ExecuteObjectSet<T>(this._connection, this._transaction, sql, paramArray);
        }

        public object ExecuteScalar(string sql, params DbParameter[] paramArray)
        {
            return this._dataAccess.ExecuteScalar(this._connection, this._transaction, sql, paramArray);
        }

        public object ExecuteStoredProcedure(string procedureName, bool isReturnDataTable, params DbParameter[] parameters)
        {
            return this._dataAccess.ExecuteStoredProcedure(this._connection, procedureName, isReturnDataTable, parameters);
        }

        public List<T> FetchAll<T>()
        {
            return this._dataAccess.Select<T>(this._connection, null, new DbParameter[0]);
        }

        public IList FetchAll(Type type)
        {
            return this._dataAccess.Select(type, this._connection, null, new DbParameter[0]);
        }

        ~DataContext()
        {
            this.Dispose(false);
        }

        public T FindById<T>(params object[] keys) where T: class
        {
            Column column;
            Table tableOrCreate = TableCache.GetTableOrCreate(typeof(T));
            if (tableOrCreate.KeyColumns.Count != keys.Length)
            {
                throw new ApplicationException("主键值的数量不正确");
            }
            T local = Activator.CreateInstance<T>();
            if (tableOrCreate.KeyColumns.Count == 1)
            {
                column = tableOrCreate.KeyColumns[0];
                column.SetAction(local, column.TypeHelper.ConvertFrom(keys[0], column.DataType));
            }
            else
            {
                for (int i = 0; i < tableOrCreate.KeyColumns.Count; i++)
                {
                    column = tableOrCreate.KeyColumns[i];
                    column.SetAction(local, column.TypeHelper.ConvertFrom(keys[i], column.DataType));
                }
            }
            if (this._dataAccess.Fill(this._connection, local))
            {
                return local;
            }
            return default(T);
        }

        public object FindById(Type type, params object[] keys)
        {
            Table tableOrCreate = TableCache.GetTableOrCreate(type);
            if (tableOrCreate.KeyColumns.Count != keys.Length)
            {
                throw new ApplicationException("主键值的数量不正确");
            }
            object obj2 = Activator.CreateInstance(type);
            if (tableOrCreate.KeyColumns.Count == 1)
            {
                tableOrCreate.KeyColumns[0].PropertyInfo.SetValue(obj2, keys[0], null);
            }
            else
            {
                for (int i = 0; i < tableOrCreate.KeyColumns.Count; i++)
                {
                    tableOrCreate.KeyColumns[i].PropertyInfo.SetValue(obj2, keys[i], null);
                }
            }
            if (this._dataAccess.Fill(this._connection, obj2))
            {
                return obj2;
            }
            return null;
        }

        public virtual List<T> FindMore<T>(int[] idArray) where T: class
        {
            return this._dataAccess.FindMore<T>(this._connection, idArray);
        }

        public T FirstOrDefault<T>(Expression<Func<T, bool>> expr)
        {
            return new Query<T>(this).FirstOrDefault<T>(expr);
        }

        public T GetAndLock<T>(int? wait, params object[] keys) where T: class
        {
            Column column;
            Table tableOrCreate = TableCache.GetTableOrCreate(typeof(T));
            if (tableOrCreate.KeyColumns.Count != keys.Length)
            {
                throw new ApplicationException("主键值的数量不正确");
            }
            T local = Activator.CreateInstance<T>();
            if (tableOrCreate.KeyColumns.Count == 1)
            {
                column = tableOrCreate.KeyColumns[0];
                column.SetAction(local, column.TypeHelper.ConvertFrom(keys[0], column.DataType));
            }
            else
            {
                for (int i = 0; i < tableOrCreate.KeyColumns.Count; i++)
                {
                    column = tableOrCreate.KeyColumns[i];
                    column.SetAction(local, column.TypeHelper.ConvertFrom(keys[i], column.DataType));
                }
            }
            if (this._dataAccess.FillAndLock(this._connection, local, wait))
            {
                return local;
            }
            return default(T);
        }

        public virtual long GetNextIdentity(bool rollBack = false)
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[IdentityConnectionStringDefault];
            if (settings != null)
            {
                using (DataContext context = new DataContext(ConnectionInfo.FromConfig(IdentityConnectionStringDefault, false), true))
                {
                    return context.DataAccess.GetNextIdentity(context.ProviderFactory, context.ConnectionString, null, rollBack);
                }
            }
            return this.DataAccess.GetNextIdentity(this._providerFactory, this._connectionString, null, rollBack);
        }

        public virtual int GetNextIdentity_Int(bool rollBack = false)
        {
            return this.GetNextIdentity_Int(string.Empty, rollBack);
        }

        public virtual int GetNextIdentity_Int<T>(bool rollBack = false) where T: class
        {
            string name = typeof(T).Name;
            return this.GetNextIdentity_Int(name, rollBack);
        }

        public virtual int GetNextIdentity_Int(string key, bool rollBack = false)
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[IdentityConnectionStringDefault];
            if (settings != null)
            {
                using (DataContext context = new DataContext(ConnectionInfo.FromConfig(IdentityConnectionStringDefault, false), true))
                {
                    return context.DataAccess.GetNextIdentity_Int(context.ProviderFactory, context.ConnectionString, null, rollBack);
                }
            }
            return this.DataAccess.GetNextIdentity_Int(this._providerFactory, this._connectionString, key, rollBack);
        }

        public virtual void Insert(object obj)
        {
            if (this._transaction != null)
            {
                this._dataAccess.Insert(this._transaction, obj);
            }
            else
            {
                this._dataAccess.Insert(this._connection, obj);
            }
        }

        public async virtual Task InsertAsync(object obj)
        {
            if (this._transaction != null)
            {
                await this._dataAccess.InsertAsync(this._transaction, obj);
            }
            else
            {
                await this._dataAccess.InsertAsync(this._connection, obj);
            }
        }

        private void InternalUpdatePartial(Type type, Dictionary<Column, object> keyDict, Dictionary<Column, object> valueDict)
        {
            if (this._transaction != null)
            {
                this._dataAccess.UpdatePartial(this._transaction, type, keyDict, valueDict);
            }
            else
            {
                this._dataAccess.UpdatePartial(this._connection, type, keyDict, valueDict);
            }
        }

        public bool Load<T>(T obj, Expression<Func<T, object>> expr)
        {
            if (expr == null)
            {
                throw new ArgumentNullException("expr");
            }
            LambdaExpression expression = expr;
            if (expression.Body.NodeType != ExpressionType.MemberAccess)
            {
                throw new ApplicationException("lambda.Body.NodeType != ExpressionType.MemberAccess");
            }
            MemberExpression body = expression.Body as MemberExpression;
            PropertyInfo member = body.Member as PropertyInfo;
            object[] customAttributes = member.GetCustomAttributes(typeof(KingReferenceAttribute), false);
            if (customAttributes.Length > 0)
            {
                KingReferenceAttribute attr = customAttributes[0] as KingReferenceAttribute;
                return this.LoadReferenceNavigation<T>(obj, member, attr);
            }
            this.LoadCollectionNavigation<T>(obj, member);
            return true;
        }

        private void LoadCollectionNavigation<T>(T obj, PropertyInfo prop)
        {
            object[] customAttributes = prop.GetCustomAttributes(typeof(KingCollectionAttribute), false);
            if (customAttributes.Length <= 0)
            {
                throw new ApplicationException("未指定KingReference或KingCollection属性");
            }
            KingCollectionAttribute attribute = customAttributes[0] as KingCollectionAttribute;
            Table tableOrCreate = TableCache.GetTableOrCreate(typeof(T));
            if (tableOrCreate.KeyColumns.Count > 1)
            {
                throw new ApplicationException("只支持单主键的集合属性");
            }
            object firstKeyValue = tableOrCreate.GetFirstKeyValue(obj);
            Type type = prop.PropertyType.GetGenericArguments()[0];
            Column columnByProperty = TableCache.GetTableOrCreate(type).GetColumnByProperty(attribute.RefPropertyName);
            IList list = this._dataAccess.FilterWithOneField(this._connection, type, columnByProperty.ColumnName, firstKeyValue);
            if (prop.CanWrite)
            {
                prop.SetValue(obj, list);
            }
            else
            {
                object obj3 = prop.GetValue(obj);
                if (obj3 == null)
                {
                    throw new ApplicationException(string.Format("属性{0}不可写，并且为空.", prop));
                }
                IList list2 = obj3 as IList;
                if (obj3 == null)
                {
                    throw new ApplicationException(string.Format("属性{0}未实现IList", new object[0]));
                }
                foreach (object obj4 in list)
                {
                    list2.Add(obj4);
                }
            }
        }

        private bool LoadReferenceNavigation<T>(T obj, PropertyInfo prop, KingReferenceAttribute attr)
        {
            object obj3;
            object obj2 = TableCache.GetTableOrCreate(typeof(T)).GetColumnByProperty(attr.RefPropertyName).GetFunction(obj);
            if (obj2 == null)
            {
                obj3 = null;
            }
            else
            {
                obj3 = this.FindById(prop.PropertyType, new object[] { obj2 });
            }
            prop.SetValue(obj, obj3);
            return (obj3 == null);
        }

        public void Open()
        {
            if (this._connection.State != ConnectionState.Open)
            {
                this._connection.Open();
            }
        }

        protected Query<T> Query<T>()
        {
            return new Query<T>(this);
        }

        public void RollbackTrans()
        {
            if (this._transaction == null)
            {
                throw new ApplicationException("未启用事务");
            }
            this._transaction.Rollback();
            this._transaction = null;
        }

        internal virtual int SaveChanges()
        {
            return -1;
        }

        public Query<T> Set<T>()
        {
            return new Query<T>(this);
        }

        public void Truncate(Type type)
        {
            this._dataAccess.Truncate(this._connection, type);
        }

        public virtual void Update(object obj)
        {
            if (this._transaction != null)
            {
                this._dataAccess.Update(this._transaction, obj);
            }
            else
            {
                this._dataAccess.Update(this._connection, obj);
            }
        }

        public void UpdatePartial<T>(Expression<Func<T>> columns)
        {
            Table tableOrCreate = TableCache.GetTableOrCreate(typeof(T));
            Dictionary<Column, object> tempDict = new Dictionary<Column, object>();
            MemberInitExpression body = columns.Body as MemberInitExpression;
            if (body == null)
            {
                throw new ApplicationException("部分更新只支持对象初始化的方式");
            }
            foreach (MemberBinding binding in body.Bindings)
            {
                if (binding.BindingType != MemberBindingType.Assignment)
                {
                    throw new ApplicationException("暂不支持复杂的属性初始化方式");
                }
                MemberAssignment assignment = binding as MemberAssignment;
                ConstantExpression expression2 = PartialEvaluator.Eval(assignment.Expression) as ConstantExpression;
                string name = assignment.Member.Name;
                if (expression2 == null)
                {
                    throw new ApplicationException(string.Format("无法计算属性{0}的值", name));
                }
                object obj2 = expression2.Value;
                Column columnByColumnName = tableOrCreate.GetColumnByColumnName(name);
                if (columnByColumnName == null)
                {
                    throw new ApplicationException(string.Format("在表{0}中找不到名为{1}的列", tableOrCreate.TableName, name));
                }
                tempDict[columnByColumnName] = obj2;
                Trace.WriteLine(string.Format("{0}-{1}", name, obj2));
            }
            Dictionary<Column, object> keyDict = tableOrCreate.KeyColumns.ToDictionary<Column, Column, object>(p => p, delegate (Column p) {
                if (!tempDict.ContainsKey(p))
                {
                    throw new ApplicationException(string.Format("未给主键字段{0}赋值", p.ColumnName));
                }
                return tempDict[p];
            });
            Dictionary<Column, object> valueDict = (from p in tempDict
                where !p.Key.IsPrimaryKey
                select p).ToDictionary<KeyValuePair<Column, object>, Column, object>(p => p.Key, p => p.Value);
            this.InternalUpdatePartial(typeof(T), keyDict, valueDict);
        }

        public void UpdatePartial(object obj, List<string> columns)
        {
            Dictionary<Column, object> valueDict = new Dictionary<Column, object>();
            Table tableOrCreate = TableCache.GetTableOrCreate(obj.GetType());
            foreach (string str in columns)
            {
                Column columnByColumnName = tableOrCreate.GetColumnByColumnName(str);
                if (columnByColumnName == null)
                {
                    throw new ApplicationException(string.Format("在表{0}中找不到名为{1}的列", tableOrCreate.TableName, str));
                }
                object obj2 = columnByColumnName.GetFunction(obj);
                valueDict[columnByColumnName] = obj2;
                Trace.WriteLine(string.Format("{0}-{1}", str, obj2));
            }
            Dictionary<Column, object> keyDict = tableOrCreate.KeyColumns.ToDictionary<Column, Column, object>(p => p, p => p.GetFunction(obj));
            this.InternalUpdatePartial(obj.GetType(), keyDict, valueDict);
        }

        public void UpdatePartial<T>(T obj, Expression<Func<T, object>> columns)
        {
            List<string> list = (from p in columns.Body.Type.GetProperties() select p.Name).ToList<string>();
            if (list.Count == 0)
            {
                throw new ApplicationException("未能正确指定需要更新的列");
            }
            this.UpdatePartial(obj, list);
        }

        public List<T> Where<T>(Expression<Func<T, bool>> expr)
        {
            return new Query<T>(this).Where<T>(expr).ToList<T>();
        }

        public List<T> Where<T>(string condition, params DbParameter[] paramArray)
        {
            return this._dataAccess.Select<T>(this._connection, condition, paramArray);
        }

        public IList Where(Type type, string condition, params DbParameter[] paramArray)
        {
            return this._dataAccess.Select(type, this._connection, condition, paramArray);
        }

        public DbConnection Connection
        {
            get
            {
                return this._connection;
            }
        }

        public string ConnectionString
        {
            get
            {
                return this._connectionString;
            }
        }

        public static ConnectionInfo CurrentConnectionInfo
        {
            get
            {
                if (_sessionConnectionInfo != null)
                {
                    return _sessionConnectionInfo;
                }
                return ConnectionInfo.Default;
            }
        }

        public BaseDataAccess DataAccess
        {
            get
            {
                return this._dataAccess;
            }
        }

        public DatabaseTypeEnum DatabaseType
        {
            get
            {
                return this._databaseType;
            }
        }

        public DbProviderFactory ProviderFactory
        {
            get
            {
                return this._providerFactory;
            }
        }

        public static ConnectionInfo SessionConnectionInfo
        {
            get
            {
                return _sessionConnectionInfo;
            }
            set
            {
                _sessionConnectionInfo = value;
            }
        }

       
        }
    }

