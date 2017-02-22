namespace King.Framework.Linq.Data
{
    using King.Framework.Linq;
    using King.Framework.Linq.Data.Common;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;

    public class EntitySession : IEntitySession
    {
        private EntityProvider provider;
        private SessionProvider sessionProvider;
        private Dictionary<MappingEntity, ISessionTable> tables;

        public EntitySession(EntityProvider provider)
        {
            this.provider = provider;
            this.sessionProvider = new SessionProvider(this, provider);
            this.tables = new Dictionary<MappingEntity, ISessionTable>();
        }

        protected virtual ISessionTable CreateTable(MappingEntity entity)
        {
            return (ISessionTable)Activator.CreateInstance(typeof(TrackedTable<>).MakeGenericType(new Type[] { entity.ElementType }), new object[] { this, entity });
        }

        private IEnumerable<Edge> GetEdges(IEnumerable<TrackedItem> items)
        {
            foreach (TrackedItem iteratorVariable0 in items)
            {
                foreach (EntityInfo iteratorVariable1 in this.provider.Mapping.GetDependingEntities(iteratorVariable0.Entity, iteratorVariable0.Instance))
                {
                    TrackedItem trackedItem = this.GetTrackedItem(iteratorVariable1);
                    if (trackedItem != null)
                    {
                        yield return new Edge(trackedItem, iteratorVariable0);
                    }
                }
                foreach (EntityInfo iteratorVariable3 in this.provider.Mapping.GetDependentEntities(iteratorVariable0.Entity, iteratorVariable0.Instance))
                {
                    TrackedItem target = this.GetTrackedItem(iteratorVariable3);
                    if (target != null)
                    {
                        yield return new Edge(iteratorVariable0, target);
                    }
                }
            }
        }

        private IEnumerable<TrackedItem> GetOrderedItems()
        {
            List<TrackedItem> items = (from tab in this.GetTables()
                                       from ti in ((ITrackedTable)tab).TrackedItems
                                       where ti.State != SubmitAction.None
                                       select ti).ToList<TrackedItem>();
            List<Edge> source = this.GetEdges(items).Distinct<Edge>().ToList<Edge>();
            ILookup<TrackedItem, TrackedItem> stLookup = source.ToLookup<Edge, TrackedItem, TrackedItem>(e => e.Source, e => e.Target);
            ILookup<TrackedItem, TrackedItem> tsLookup = source.ToLookup<Edge, TrackedItem, TrackedItem>(e => e.Target, e => e.Source);
            return items.Sort<TrackedItem>(delegate (TrackedItem item) {
                switch (item.State)
                {
                    case SubmitAction.Insert:
                    case SubmitAction.InsertOrUpdate:
                        {
                            IEnumerable<TrackedItem> first = stLookup[item];
                            object instance = item.Table.GetFromCacheById(this.provider.Mapping.GetPrimaryKey(item.Entity, item.Instance));
                            if ((instance != null) && (instance != item.Instance))
                            {
                                TrackedItem trackedItem = item.Table.GetTrackedItem(instance);
                                if ((trackedItem != null) && (trackedItem.State == SubmitAction.Delete))
                                {
                                    first = first.Concat<TrackedItem>(new TrackedItem[] { trackedItem });
                                }
                            }
                            return first;
                        }
                    case SubmitAction.Delete:
                        return tsLookup[item];
                }
                return TrackedItem.EmptyList;
            });
        }

        protected ISessionTable GetTable(MappingEntity entity)
        {
            ISessionTable table;
            if (!this.tables.TryGetValue(entity, out table))
            {
                table = this.CreateTable(entity);
                this.tables.Add(entity, table);
            }
            return table;
        }

        public ISessionTable<T> GetTable<T>(string tableId)
        {
            return (ISessionTable<T>)this.GetTable(typeof(T), tableId);
        }

        public ISessionTable GetTable(Type elementType, string tableId)
        {
            return this.GetTable(this.sessionProvider.Provider.Mapping.GetEntity(elementType, tableId));
        }

        protected IEnumerable<ISessionTable> GetTables()
        {
            return this.tables.Values;
        }

        private TrackedItem GetTrackedItem(EntityInfo entity)
        {
            ITrackedTable table = (ITrackedTable)this.GetTable(entity.Mapping);
            return table.GetTrackedItem(entity.Instance);
        }

        private object OnEntityMaterialized(MappingEntity entity, object instance)
        {
            IEntitySessionTable table = (IEntitySessionTable)this.GetTable(entity);
            return table.OnEntityMaterialized(instance);
        }

        public virtual void SubmitChanges()
        {
            this.provider.DoTransacted(delegate {
                List<TrackedItem> list = new List<TrackedItem>();
                foreach (TrackedItem item in this.GetOrderedItems())
                {
                    if (item.Table.SubmitChanges(item))
                    {
                        list.Add(item);
                    }
                }
                foreach (TrackedItem item in list)
                {
                    item.Table.AcceptChanges(item);
                }
            });
        }

        IEntityProvider IEntitySession.Provider
        {
            get
            {
                return this.Provider;
            }
        }

        public IEntityProvider Provider
        {
            get
            {
                return this.sessionProvider;
            }
        }

       

    private class Edge : IEquatable<EntitySession.Edge>
    {
        private int hash;

        internal Edge(EntitySession.TrackedItem source, EntitySession.TrackedItem target)
        {
            this.Source = source;
            this.Target = target;
            this.hash = this.Source.GetHashCode() + this.Target.GetHashCode();
        }

        public bool Equals(EntitySession.Edge edge)
        {
            return (((edge != null) && (this.Source == edge.Source)) && (this.Target == edge.Target));
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as EntitySession.Edge);
        }

        public override int GetHashCode()
        {
            return this.hash;
        }

        internal EntitySession.TrackedItem Source { get; private set; }

        internal EntitySession.TrackedItem Target { get; private set; }
    }

    private interface IEntitySessionTable : ISessionTable, IQueryable, IEnumerable
    {
        object OnEntityMaterialized(object instance);

        MappingEntity Entity { get; }
    }

    private interface ITrackedTable : EntitySession.IEntitySessionTable, ISessionTable, IQueryable, IEnumerable
    {
        void AcceptChanges(EntitySession.TrackedItem item);
        object GetFromCacheById(object key);
        EntitySession.TrackedItem GetTrackedItem(object instance);
        bool SubmitChanges(EntitySession.TrackedItem item);

        IEnumerable<EntitySession.TrackedItem> TrackedItems { get; }
    }

    private class SessionExecutor : QueryExecutor
    {
        private QueryExecutor executor;
        private EntitySession session;

        public SessionExecutor(EntitySession session, QueryExecutor executor)
        {
            this.session = session;
            this.executor = executor;
        }

        public override object Convert(object value, Type type)
        {
            return this.executor.Convert(value, type);
        }

        public override IEnumerable<T> Execute<T>(QueryCommand command, Func<FieldReader, T> fnProjector, MappingEntity entity, object[] paramValues)
        {
            return this.executor.Execute<T>(command, this.Wrap<T>(fnProjector, entity), entity, paramValues);
        }

        public override IEnumerable<int> ExecuteBatch(QueryCommand query, IEnumerable<object[]> paramSets, int batchSize, bool stream)
        {
            return this.executor.ExecuteBatch(query, paramSets, batchSize, stream);
        }

        public override IEnumerable<T> ExecuteBatch<T>(QueryCommand query, IEnumerable<object[]> paramSets, Func<FieldReader, T> fnProjector, MappingEntity entity, int batchSize, bool stream)
        {
            return this.executor.ExecuteBatch<T>(query, paramSets, this.Wrap<T>(fnProjector, entity), entity, batchSize, stream);
        }

        public override int ExecuteCommand(QueryCommand query, object[] paramValues)
        {
            return this.executor.ExecuteCommand(query, paramValues);
        }

        public override IEnumerable<T> ExecuteDeferred<T>(QueryCommand query, Func<FieldReader, T> fnProjector, MappingEntity entity, object[] paramValues)
        {
            return this.executor.ExecuteDeferred<T>(query, this.Wrap<T>(fnProjector, entity), entity, paramValues);
        }

        private Func<FieldReader, T> Wrap<T>(Func<FieldReader, T> fnProjector, MappingEntity entity)
        {
            return fr => ((T)this.session.OnEntityMaterialized(entity, fnProjector(fr)));
        }

        public override int RowsAffected
        {
            get
            {
                return this.executor.RowsAffected;
            }
        }
    }

    private class SessionProvider : QueryProvider, IEntityProvider, IQueryProvider, ICreateExecutor
    {
        private EntityProvider provider;
        private EntitySession session;

        public SessionProvider(EntitySession session, EntityProvider provider) : base(provider.Context)
        {
            this.session = session;
            this.provider = provider;
        }

        public bool CanBeEvaluatedLocally(Expression expression)
        {
            return this.provider.Mapping.CanBeEvaluatedLocally(expression);
        }

        public bool CanBeParameter(Expression expression)
        {
            return this.provider.CanBeParameter(expression);
        }

        QueryExecutor ICreateExecutor.CreateExecutor()
        {
            return new EntitySession.SessionExecutor(this.session, ((ICreateExecutor)this.provider).CreateExecutor());
        }

        public override object Execute(Expression expression)
        {
            return this.provider.Execute(expression);
        }

        public override string GetQueryText(Expression expression)
        {
            return this.provider.GetQueryText(expression);
        }

        public IEntityTable<T> GetTable<T>(string tableId)
        {
            return this.provider.GetTable<T>(tableId);
        }

        public IEntityTable GetTable(Type type, string tableId)
        {
            return this.provider.GetTable(type, tableId);
        }

        public EntityProvider Provider
        {
            get
            {
                return this.provider;
            }
        }
    }

    private abstract class SessionTable<T> : Query<T>, ISessionTable<T>, IQueryable<T>, IEnumerable<T>, EntitySession.IEntitySessionTable, ISessionTable, IQueryable, IEnumerable
    {
        private MappingEntity entity;
        private EntitySession session;
        private IEntityTable<T> underlyingTable;

        internal SessionTable(EntitySession session, MappingEntity entity) : base(session.sessionProvider, typeof(ISessionTable<T>))
        {
            this.session = session;
            this.entity = entity;
            this.underlyingTable = this.session.Provider.GetTable<T>(entity.TableId);
        }

        object ISessionTable.GetById(object id)
        {
            return this.GetById(id);
        }

        SubmitAction ISessionTable.GetSubmitAction(object instance)
        {
            return this.GetSubmitAction((T)instance);
        }

        void ISessionTable.SetSubmitAction(object instance, SubmitAction action)
        {
            this.SetSubmitAction((T)instance, action);
        }

        public T GetById(object id)
        {
            return this.underlyingTable.GetById(id);
        }

        public virtual SubmitAction GetSubmitAction(T instance)
        {
            throw new NotImplementedException();
        }

        public virtual object OnEntityMaterialized(object instance)
        {
            return instance;
        }

        public virtual void SetSubmitAction(T instance, SubmitAction action)
        {
            throw new NotImplementedException();
        }

        IEntityTable ISessionTable.ProviderTable
        {
            get
            {
                return this.underlyingTable;
            }
        }

        public MappingEntity Entity
        {
            get
            {
                return this.entity;
            }
        }

        public IEntityTable<T> ProviderTable
        {
            get
            {
                return this.underlyingTable;
            }
        }

        public IEntitySession Session
        {
            get
            {
                return this.session;
            }
        }
    }

    private class TrackedItem
    {
        public static readonly IEnumerable<EntitySession.TrackedItem> EmptyList = new EntitySession.TrackedItem[0];
        private bool hookedEvent;
        private object instance;
        private object original;
        private SubmitAction state;
        private EntitySession.ITrackedTable table;

        internal TrackedItem(EntitySession.ITrackedTable table, object instance, object original, SubmitAction state, bool hookedEvent)
        {
            this.table = table;
            this.instance = instance;
            this.original = original;
            this.state = state;
            this.hookedEvent = hookedEvent;
        }

        public MappingEntity Entity
        {
            get
            {
                return this.table.Entity;
            }
        }

        public bool HookedEvent
        {
            get
            {
                return this.hookedEvent;
            }
        }

        public object Instance
        {
            get
            {
                return this.instance;
            }
        }

        public object Original
        {
            get
            {
                return this.original;
            }
        }

        public SubmitAction State
        {
            get
            {
                return this.state;
            }
        }

        public EntitySession.ITrackedTable Table
        {
            get
            {
                return this.table;
            }
        }
    }

    private class TrackedTable<T> : EntitySession.SessionTable<T>, EntitySession.ITrackedTable, EntitySession.IEntitySessionTable, ISessionTable, IQueryable, IEnumerable
    {
        private Dictionary<object, T> identityCache;
        private Dictionary<T, EntitySession.TrackedItem> tracked;

        public TrackedTable(EntitySession session, MappingEntity entity) : base(session, entity)
        {
            this.tracked = new Dictionary<T, EntitySession.TrackedItem>();
            this.identityCache = new Dictionary<object, T>();
        }

        private void AcceptChanges(EntitySession.TrackedItem item)
        {
            switch (item.State)
            {
                case SubmitAction.Update:
                case SubmitAction.PossibleUpdate:
                    this.AssignAction((T)item.Instance, SubmitAction.PossibleUpdate);
                    break;

                case SubmitAction.Insert:
                    this.AddToCache((T)item.Instance);
                    this.AssignAction((T)item.Instance, SubmitAction.PossibleUpdate);
                    break;

                case SubmitAction.InsertOrUpdate:
                    this.AddToCache((T)item.Instance);
                    this.AssignAction((T)item.Instance, SubmitAction.PossibleUpdate);
                    break;

                case SubmitAction.Delete:
                    this.RemoveFromCache((T)item.Instance);
                    this.AssignAction((T)item.Instance, SubmitAction.None);
                    break;
            }
        }

        private T AddToCache(T instance)
        {
            T local;
            object primaryKey = this.Mapping.GetPrimaryKey(base.Entity, instance);
            if (!this.identityCache.TryGetValue(primaryKey, out local))
            {
                local = instance;
                this.identityCache.Add(primaryKey, local);
            }
            return local;
        }

        private void AssignAction(T instance, SubmitAction action)
        {
            EntitySession.TrackedItem item;
            this.tracked.TryGetValue(instance, out item);
            switch (action)
            {
                case SubmitAction.None:
                case SubmitAction.Update:
                case SubmitAction.Insert:
                case SubmitAction.InsertOrUpdate:
                case SubmitAction.Delete:
                    this.tracked[instance] = new EntitySession.TrackedItem(this, instance, (item != null) ? item.Original : null, action, (item != null) ? item.HookedEvent : false);
                    break;

                case SubmitAction.PossibleUpdate:
                    {
                        INotifyPropertyChanging changing = instance as INotifyPropertyChanging;
                        if (changing == null)
                        {
                            object original = this.Mapping.CloneEntity(base.Entity, instance);
                            this.tracked[instance] = new EntitySession.TrackedItem(this, instance, original, SubmitAction.PossibleUpdate, false);
                            break;
                        }
                        if (!item.HookedEvent)
                        {
                            changing.PropertyChanging += new PropertyChangingEventHandler(this.OnPropertyChanging);
                        }
                        this.tracked[instance] = new EntitySession.TrackedItem(this, instance, null, SubmitAction.PossibleUpdate, true);
                        break;
                    }
                default:
                    throw new InvalidOperationException(string.Format("Unknown SubmitAction: {0}", action));
            }
        }

        void EntitySession.ITrackedTable.AcceptChanges(EntitySession.TrackedItem item)
        {
            this.AcceptChanges(item);
        }

        object EntitySession.ITrackedTable.GetFromCacheById(object key)
        {
            T local;
            this.identityCache.TryGetValue(key, out local);
            return local;
        }

        EntitySession.TrackedItem EntitySession.ITrackedTable.GetTrackedItem(object instance)
        {
            EntitySession.TrackedItem item;
            if (this.tracked.TryGetValue((T)instance, out item))
            {
                return item;
            }
            return null;
        }

        bool EntitySession.ITrackedTable.SubmitChanges(EntitySession.TrackedItem item)
        {
            return this.SubmitChanges(item);
        }

        public override SubmitAction GetSubmitAction(T instance)
        {
            EntitySession.TrackedItem item;
            if (this.tracked.TryGetValue(instance, out item))
            {
                if (item.State == SubmitAction.PossibleUpdate)
                {
                    if (this.Mapping.IsModified(item.Entity, item.Instance, item.Original))
                    {
                        return SubmitAction.Update;
                    }
                    return SubmitAction.None;
                }
                return item.State;
            }
            return SubmitAction.None;
        }

        public override object OnEntityMaterialized(object instance)
        {
            T local = (T)instance;
            T local2 = this.AddToCache(local);
            if (local2.Equals(local) )
            {
                this.AssignAction(local, SubmitAction.PossibleUpdate);
            }
            return local2;
        }

        protected virtual void OnPropertyChanging(object sender, PropertyChangingEventArgs args)
        {
            EntitySession.TrackedItem item;
            if (this.tracked.TryGetValue((T)sender, out item) && (item.State == SubmitAction.PossibleUpdate))
            {
                object original = this.Mapping.CloneEntity(item.Entity, item.Instance);
                this.tracked[(T)sender] = new EntitySession.TrackedItem(this, item.Instance, original, SubmitAction.Update, true);
            }
        }

        private void RemoveFromCache(T instance)
        {
            object primaryKey = this.Mapping.GetPrimaryKey(base.Entity, instance);
            this.identityCache.Remove(primaryKey);
        }

        public override void SetSubmitAction(T instance, SubmitAction action)
        {
            switch (action)
            {
                case SubmitAction.None:
                case SubmitAction.Update:
                case SubmitAction.PossibleUpdate:
                case SubmitAction.Delete:
                    if (!this.AddToCache(instance).Equals(instance))
                    {
                        throw new InvalidOperationException("An different instance with the same key is already in the cache.");
                    }
                    break;
            }
            this.AssignAction(instance, action);
        }

        private bool SubmitChanges(EntitySession.TrackedItem item)
        {
            switch (item.State)
            {
                case SubmitAction.Update:
                    base.ProviderTable.Update(item.Instance);
                    return true;

                case SubmitAction.PossibleUpdate:
                    if ((item.Original == null) || !this.Mapping.IsModified(item.Entity, item.Instance, item.Original))
                    {
                        break;
                    }
                    base.ProviderTable.Update(item.Instance);
                    return true;

                case SubmitAction.Insert:
                    base.ProviderTable.Insert(item.Instance);
                    return true;

                case SubmitAction.InsertOrUpdate:
                    base.ProviderTable.InsertOrUpdate(item.Instance);
                    return true;

                case SubmitAction.Delete:
                    base.ProviderTable.Delete(item.Instance);
                    return true;
            }
            return false;
        }

        IEnumerable<EntitySession.TrackedItem> EntitySession.ITrackedTable.TrackedItems
        {
            get
            {
                return this.tracked.Values;
            }
        }

        private QueryMapping Mapping
        {
            get
            {
                return ((EntitySession)base.Session).provider.Mapping;
            }
        }
    }
}
}
