namespace King.Framework.Linq.Data
{
    using King.Framework.Linq;
    using King.Framework.Linq.Data.Common;
    using King.Framework.Linq.Data.Mapping;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public abstract class EntityProvider : QueryProvider, IEntityProvider, IQueryProvider, ICreateExecutor
    {
        private QueryCache cache;
        private QueryLanguage language;
        private TextWriter log;
        private QueryMapping mapping;
        private QueryPolicy policy;
        private Dictionary<MappingEntity, IEntityTable> tables;

        public EntityProvider(DataContext context, QueryLanguage language, QueryMapping mapping, QueryPolicy policy) : base(context)
        {
            if (language == null)
            {
                throw new InvalidOperationException("Language not specified");
            }
            if (mapping == null)
            {
                throw new InvalidOperationException("Mapping not specified");
            }
            if (policy == null)
            {
                throw new InvalidOperationException("Policy not specified");
            }
            this.language = language;
            this.mapping = mapping;
            this.policy = policy;
            this.tables = new Dictionary<MappingEntity, IEntityTable>();
            this.Log = new TraceWritter();
        }

        public bool CanBeEvaluatedLocally(Expression expression)
        {
            return this.Mapping.CanBeEvaluatedLocally(expression);
        }

        public virtual bool CanBeParameter(Expression expression)
        {
            if (Type.GetTypeCode(King.Framework.Linq.TypeHelper.GetNonNullableType(expression.Type)) == TypeCode.Object)
            {
                return ((expression.Type == typeof(byte[])) || (expression.Type == typeof(char[])));
            }
            return true;
        }

        protected abstract QueryExecutor CreateExecutor();
        protected virtual IEntityTable CreateTable(MappingEntity entity)
        {
            return (IEntityTable)Activator.CreateInstance(typeof(EntityTable<>).MakeGenericType(new Type[] { entity.ElementType }), new object[] { this, entity });
        }

        protected virtual QueryTranslator CreateTranslator()
        {
            return new QueryTranslator(this.language, this.mapping, this.policy);
        }

        public abstract void DoConnected(Action action);
        public abstract void DoTransacted(Action action);
        QueryExecutor ICreateExecutor.CreateExecutor()
        {
            return this.CreateExecutor();
        }

        public override object Execute(Expression expression)
        {
            LambdaExpression expression2 = expression as LambdaExpression;
            if (((expression2 == null) && (this.cache != null)) && (expression.NodeType != ExpressionType.Constant))
            {
                return this.cache.Execute(expression);
            }
            Expression executionPlan = this.GetExecutionPlan(expression);
            if (expression2 != null)
            {
                return Expression.Lambda(expression2.Type, executionPlan, expression2.Parameters).Compile();
            }
            Expression<Func<object>> expression5 = Expression.Lambda<Func<object>>(Expression.Convert(executionPlan, typeof(object)), new ParameterExpression[0]);
            return expression5.Compile()();
        }

        public abstract int ExecuteCommand(string commandText);
        private Expression Find(Expression expression, IList<ParameterExpression> parameters, Type type)
        {
            Func<ParameterExpression, bool> predicate = null;
            if (parameters != null)
            {
                if (predicate == null)
                {
                    predicate = p => type.IsAssignableFrom(p.Type);
                }
                Expression expression2 = parameters.FirstOrDefault<ParameterExpression>(predicate);
                if (expression2 != null)
                {
                    return expression2;
                }
            }
            return TypedSubtreeFinder.Find(expression, type);
        }

        private static IEnumerable<Type> FindInstancesIn(Type type, string assemblyName)
        {
            Assembly assemblyForNamespace = GetAssemblyForNamespace(assemblyName);
            if (assemblyForNamespace != null)
            {
                foreach (Type iteratorVariable1 in assemblyForNamespace.GetTypes())
                {
                    if ((string.Compare(iteratorVariable1.Namespace, assemblyName, true) == 0) && type.IsAssignableFrom(iteratorVariable1))
                    {
                        yield return iteratorVariable1;
                    }
                }
            }
        }

        private static Type FindLoadedType(string typeName)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType(typeName, false, true);
                if (type != null)
                {
                    return type;
                }
            }
            return null;
        }

        private static Assembly GetAssemblyForNamespace(string nspace)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.FullName.Contains(nspace))
                {
                    return assembly;
                }
            }
            return Load(nspace + ".dll");
        }

        public virtual Expression GetExecutionPlan(Expression expression)
        {
            LambdaExpression expression2 = expression as LambdaExpression;
            if (expression2 != null)
            {
                expression = expression2.Body;
            }
            QueryTranslator translator = this.CreateTranslator();
            Expression query = translator.Translate(expression);
            ReadOnlyCollection<ParameterExpression> parameters = (expression2 != null) ? expression2.Parameters : null;
            Expression provider = this.Find(expression, parameters, typeof(EntityProvider));
            if (provider == null)
            {
                provider = Expression.Property(this.Find(expression, parameters, typeof(IQueryable)), typeof(IQueryable).GetProperty("Provider"));
            }
            return translator.Police.BuildExecutionPlan(query, provider);
        }

        public static QueryMapping GetMapping(string mappingId)
        {
            if (mappingId != null)
            {
                Type contextType = FindLoadedType(mappingId);
                if (contextType != null)
                {
                    return new AttributeMapping(contextType);
                }
                if (File.Exists(mappingId))
                {
                    return XmlMapping.FromXml(File.ReadAllText(mappingId));
                }
            }
            return new ImplicitMapping();
        }

        public static Type GetProviderType(string providerName)
        {
            if (!string.IsNullOrEmpty(providerName))
            {
                Type type = FindInstancesIn(typeof(EntityProvider), providerName).FirstOrDefault<Type>();
                if (type != null)
                {
                    return type;
                }
            }
            return null;
        }

        public string GetQueryPlan(Expression expression)
        {
            Expression executionPlan = this.GetExecutionPlan(expression);
            return DbExpressionWriter.WriteToString(this.Language, executionPlan);
        }

        public override string GetQueryText(Expression expression)
        {
            string[] strArray = (from c in CommandGatherer.Gather(this.GetExecutionPlan(expression)) select c.CommandText).ToArray<string>();
            return string.Join("\n\n", strArray);
        }

        public virtual IEntityTable<T> GetTable<T>()
        {
            return this.GetTable<T>(null);
        }

        public IEntityTable GetTable(MappingEntity entity)
        {
            IEntityTable table;
            if (!this.tables.TryGetValue(entity, out table))
            {
                table = this.CreateTable(entity);
                this.tables.Add(entity, table);
            }
            return table;
        }

        public virtual IEntityTable<T> GetTable<T>(string tableId)
        {
            return (IEntityTable<T>)this.GetTable(typeof(T), tableId);
        }

        public virtual IEntityTable GetTable(Type type)
        {
            return this.GetTable(type, null);
        }

        public virtual IEntityTable GetTable(Type type, string tableId)
        {
            return this.GetTable(this.Mapping.GetEntity(type, tableId));
        }

        private static Assembly Load(string name)
        {
            try
            {
                return Assembly.LoadFrom(name);
            }
            catch
            {
            }
            return null;
        }

        public QueryCache Cache
        {
            get
            {
                return this.cache;
            }
            set
            {
                this.cache = value;
            }
        }

        public QueryLanguage Language
        {
            get
            {
                return this.language;
            }
        }

        public TextWriter Log
        {
            get
            {
                return this.log;
            }
            set
            {
                this.log = value;
            }
        }

        public QueryMapping Mapping
        {
            get
            {
                return this.mapping;
            }
        }

        public QueryPolicy Policy
        {
            get
            {
                return this.policy;
            }
            set
            {
                if (value == null)
                {
                    this.policy = QueryPolicy.Default;
                }
                else
                {
                    this.policy = value;
                }
            }
        }

    }

    public class CommandGatherer : DbExpressionVisitor
    {
        private List<QueryCommand> commands = new List<QueryCommand>();

        public static ReadOnlyCollection<QueryCommand> Gather(Expression expression)
        {
            CommandGatherer gatherer = new CommandGatherer();
            gatherer.Visit(expression);
            return gatherer.commands.AsReadOnly();
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            QueryCommand item = c.Value as QueryCommand;
            if (item != null)
            {
                this.commands.Add(item);
            }
            return c;
        }
    }

    public class EntityTable<T> : Query<T>, IEntityTable<T>, IEntityTable, IUpdatable<T>, IUpdatable, IQueryable<T>, IEnumerable<T>, IQueryable, IEnumerable, IHaveMappingEntity
    {
        private MappingEntity entity;
        private EntityProvider provider;

        public EntityTable(EntityProvider provider, MappingEntity entity) : base(provider, typeof(IEntityTable<T>))
        {
            this.provider = provider;
            this.entity = entity;
        }

        public int Delete(T instance)
        {
            return this.Delete<T>(instance);
        }

        int IEntityTable.Delete(object instance)
        {
            return this.Delete((T)instance);
        }

        object IEntityTable.GetById(object id)
        {
            return this.GetById(id);
        }

        int IEntityTable.Insert(object instance)
        {
            return this.Insert((T)instance);
        }

        int IEntityTable.InsertOrUpdate(object instance)
        {
            return this.InsertOrUpdate((T)instance);
        }

        int IEntityTable.Update(object instance)
        {
            return this.Update((T)instance);
        }

        public T GetById(object id)
        {
            IEntityProvider provider = this.Provider;
            if (provider != null)
            {
                IEnumerable<object> enumerable = id as IEnumerable<object>;
                if (enumerable == null)
                {
                    enumerable = new object[] { id };
                }
                Expression expression = ((EntityProvider)provider).Mapping.GetPrimaryKeyQuery(this.entity, base.Expression, (from v in enumerable select Expression.Constant(v)).ToArray<ConstantExpression>());
                return this.Provider.Execute<T>(expression);
            }
            return default(T);
        }

        public int Insert(T instance)
        {
            return this.Insert<T>(instance);
        }

        public int InsertOrUpdate(T instance)
        {
            return this.InsertOrUpdate<T>(instance);
        }

        public int Update(T instance)
        {
            return this.Update<T>(instance);
        }

        public MappingEntity Entity
        {
            get
            {
                return this.entity;
            }
        }

        public Type EntityType
        {
            get
            {
                return this.entity.EntityType;
            }
        }

        public IEntityProvider Provider
        {
            get
            {
                return this.provider;
            }
        }

        public string TableId
        {
            get
            {
                return this.entity.TableId;
            }
        }
    }
}

