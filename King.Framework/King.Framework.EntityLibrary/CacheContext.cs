namespace King.Framework.EntityLibrary
{
    using King.Framework.Linq;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;

    public class CacheContext : IDisposable
    {
        private Dictionary<Type, Dictionary<long, ICachedEntity>> _cache;
        private string ConnectionString;
        private Dictionary<string, King.Framework.EntityLibrary.SysEntity> entityNameDict;
        internal readonly Dictionary<string, ICachedEntity> idDict;
        internal readonly HashSet<string> idSet;
        private Dictionary<string, ICachedEntity> keyDict;

        protected CacheContext()
        {
            this.idSet = new HashSet<string>();
            this.idDict = new Dictionary<string, ICachedEntity>();
            this.entityNameDict = null;
        }

        private CacheContext(string connStr)
        {
            this.idSet = new HashSet<string>();
            this.idDict = new Dictionary<string, ICachedEntity>();
            this.entityNameDict = null;
            this.Init(connStr, null);
        }

        private CacheContext(string connStr, string providerName)
        {
            this.idSet = new HashSet<string>();
            this.idDict = new Dictionary<string, ICachedEntity>();
            this.entityNameDict = null;
            this.Init(connStr, providerName);
        }

        public void BuildKeyDict()
        {
            this.keyDict = new Dictionary<string, ICachedEntity>(0x400);
            List<Type> cacheTableList = this.GetCacheTableList();
            foreach (Type type in cacheTableList)
            {
                if (type != typeof(King.Framework.EntityLibrary.SysPageControl))
                {
                    IList list = this.GetList(type);
                    if (list != null)
                    {
                        foreach (object obj2 in list)
                        {
                            if (!(obj2 is ICachedEntity))
                            {
                                throw new ApplicationException(string.Format("类型{0}未实现ICachedEntity", obj2.GetType()));
                            }
                            IKeyObject obj3 = obj2 as IKeyObject;
                            if (obj3 != null)
                            {
                                string key = "";
                                try
                                {
                                    key = obj3.GetKey();
                                    if (this.keyDict.ContainsKey(key))
                                    {
                                        King.Framework.EntityLibrary.SysCondition condition = obj3 as King.Framework.EntityLibrary.SysCondition;
                                        if (condition != null)
                                        {
                                            King.Framework.EntityLibrary.SysCondition condition2 = this.keyDict[key] as King.Framework.EntityLibrary.SysCondition;
                                            if (condition2.ParentCondition == condition)
                                            {
                                                this.keyDict[key] = condition;
                                            }
                                        }
                                        King.Framework.EntityLibrary.SysViewCondition condition3 = obj3 as King.Framework.EntityLibrary.SysViewCondition;
                                        if (condition3 != null)
                                        {
                                            King.Framework.EntityLibrary.SysViewCondition condition4 = this.keyDict[key] as King.Framework.EntityLibrary.SysViewCondition;
                                            if (condition4.ParentCondition == condition3)
                                            {
                                                this.keyDict[key] = condition3;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        this.keyDict.Add(key, obj3 as ICachedEntity);
                                    }
                                }
                                catch (Exception exception)
                                {
                                    Trace.WriteLine(string.Format("{0}", obj2));
                                    Trace.WriteLine(key);
                                    Trace.WriteLine(exception.Message);
                                    Trace.WriteLine(exception.StackTrace);
                                    throw exception;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void ClearCache()
        {
            if (this._cache != null)
            {
                this._cache.Clear();
            }
        }

        public void Dispose()
        {
            this.ClearCache();
            this.ConnectionString = null;
            this._cache = null;
        }

        public T FindById<T>(long? id)
        {
            if (id.HasValue)
            {
                Type key = typeof(T);
                if (this._cache.ContainsKey(key))
                {
                    Dictionary<long, ICachedEntity> dictionary = this._cache[key];
                    if (dictionary.ContainsKey(id.Value))
                    {
                        return (T) dictionary[id.Value];
                    }
                }
            }
            return default(T);
        }

        public object FindById(Type type, long? id)
        {
            if (id.HasValue && this._cache.ContainsKey(type))
            {
                Dictionary<long, ICachedEntity> dictionary = this._cache[type];
                if (dictionary.ContainsKey(id.Value))
                {
                    return dictionary[id.Value];
                }
            }
            return null;
        }

        public ICachedEntity FindByKey(string key, bool throwNullException = false)
        {
            if (this.keyDict.ContainsKey(key))
            {
                return this.keyDict[key];
            }
            if (throwNullException)
            {
                throw new ApplicationException(string.Format("keyDict中不包含key: {0}", key));
            }
            return null;
        }

        public King.Framework.EntityLibrary.SysEntity FindEntityByName(string entityName)
        {
            if (this.entityNameDict == null)
            {
                if ((this.SysEntity == null) || (this.SysEntity.Count == 0))
                {
                    throw new ApplicationException("未加载CacheContext的数据之前，不能调用FindEntityByName方法.");
                }
                this.entityNameDict = new Dictionary<string, King.Framework.EntityLibrary.SysEntity>(this.SysEntity.Count + 100);
                List<King.Framework.EntityLibrary.SysEntity> sysEntity = this.SysEntity;
                foreach (King.Framework.EntityLibrary.SysEntity entity in sysEntity)
                {
                    this.entityNameDict.Add(entity.EntityName, entity);
                }
            }
            if (this.entityNameDict.ContainsKey(entityName))
            {
                return this.entityNameDict[entityName];
            }
            return null;
        }

        protected List<T> GetCache<T>()
        {
            Func<KeyValuePair<long, ICachedEntity>, T> selector = null;
            if (this._cache == null)
            {
                this.Init(this.ConnectionString, null);
            }
            Type key = typeof(T);
            if (!this._cache.ContainsKey(key))
            {
                throw new Exception(string.Format("类型{0}没有缓存", key.Name));
            }
            Dictionary<long, ICachedEntity> source = this._cache[key];
            if (selector == null)
            {
                selector = p => (T) p.Value;
            }
            return source.Select<KeyValuePair<long, ICachedEntity>, T>(selector).ToList<T>();
        }

        protected List<ICachedEntity> GetCache(Type type)
        {
            if (type == null)
            {
                throw new Exception("类型为空");
            }
            if (!this._cache.ContainsKey(type))
            {
                throw new Exception(string.Format("类型{0}没有缓存", type.Name));
            }
            Dictionary<long, ICachedEntity> dictionary = this._cache[type];
            return (from p in dictionary select p.Value).ToList<ICachedEntity>();
        }

        public List<Type> GetCacheTableList()
        {
            string interfaceName = typeof(ICachedEntity).Name;
            return (from p in from p in base.GetType().GetProperties() select p.PropertyType
                where p.IsGenericType && (p.GetGenericTypeDefinition() == typeof(List<>))
                select p.GetGenericArguments()[0] into p
                where p.GetInterface(interfaceName) != null
                select p).ToList<Type>();
        }

        public IList GetList(Type type)
        {
            if (this._cache.ContainsKey(type))
            {
                return this._cache[type].Values.ToList<ICachedEntity>();
            }
            return null;
        }

        private void Init(string connStr, string providerName = null)
        {
            InternalMetaDataContext context;
            if (!string.IsNullOrEmpty(connStr))
            {
                this.ConnectionString = connStr;
                using (context = new InternalMetaDataContext(connStr, providerName, true))
                {
                    this.InitCache(context);
                }
            }
            else
            {
                using (context = new InternalMetaDataContext(true))
                {
                    this.ConnectionString = context.ConnectionString;
                    this.InitCache(context);
                }
            }
        }

        private void InitCache(DataContext context)
        {
            Dictionary<Type, IList> data = this.GetCacheTableList().ToDictionary<Type, Type, IList>(p => p, p => context.FetchAll(p));
            this.InitCache(data);
        }

        protected void InitCache(Dictionary<Type, IList> data)
        {
            this._cache = new Dictionary<Type, Dictionary<long, ICachedEntity>>();
            Dictionary<long, ICachedEntity> pageControlDic = new Dictionary<long, ICachedEntity>();
            foreach (Type type in data.Keys)
            {
                if (type != typeof(King.Framework.EntityLibrary.SysPageControl))
                {
                    Dictionary<long, ICachedEntity> dictionary2 = new Dictionary<long, ICachedEntity>();
                    IList list = data[type];
                    foreach (ICachedEntity entity in list)
                    {
                        entity.InitNavigationList();
                        dictionary2[entity.PrimaryKeyValue] = entity;
                        if (entity is King.Framework.EntityLibrary.SysPageControl)
                        {
                            pageControlDic[entity.PrimaryKeyValue] = entity;
                        }
                    }
                    this._cache[type] = dictionary2;
                }
            }
            if (data.ContainsKey(typeof(King.Framework.EntityLibrary.SysPageControl)))
            {
                this.SetTruePageControl(pageControlDic, data[typeof(King.Framework.EntityLibrary.SysPageControl)] as List<King.Framework.EntityLibrary.SysPageControl>);
            }
            foreach (KeyValuePair<Type, Dictionary<long, ICachedEntity>> pair in this._cache)
            {
                if (pair.Key != typeof(King.Framework.EntityLibrary.SysPageControl))
                {
                    foreach (ICachedEntity entity in pair.Value.Values)
                    {
                        entity.SetContext(this);
                    }
                }
            }
            if (data.ContainsKey(typeof(King.Framework.EntityLibrary.SysEntityJoinRelation)))
            {
                List<King.Framework.EntityLibrary.SysEntityJoinRelation> cache = this.GetCache<King.Framework.EntityLibrary.SysEntityJoinRelation>();
                foreach (King.Framework.EntityLibrary.SysEntityJoinRelation relation in cache)
                {
                    King.Framework.EntityLibrary.SysEntityJoinRelation parentEntityJoinRelation = relation;
                    while (parentEntityJoinRelation.ParentEntityJoinRelation != null)
                    {
                        parentEntityJoinRelation = parentEntityJoinRelation.ParentEntityJoinRelation;
                    }
                    relation.RootOwnerEntity = parentEntityJoinRelation.OwnerEntity;
                    if (relation.RootOwnerEntity != null)
                    {
                        relation.RootOwnerEntity.EntityJoinRelations.Add(relation);
                    }
                }
            }
        }

        public static CacheContext New()
        {
            return new CacheContext(null);
        }

        public static CacheContext New(string connStr)
        {
            return new CacheContext(connStr);
        }

        public static CacheContext New(string connStr, string providerName)
        {
            return new CacheContext(connStr, providerName);
        }

        private void SetTruePageControl(Dictionary<long, ICachedEntity> pageControlDic, List<King.Framework.EntityLibrary.SysPageControl> baseControlList)
        {
            foreach (King.Framework.EntityLibrary.SysPageControl control in baseControlList)
            {
                if (!pageControlDic.ContainsKey(control.ControlId))
                {
                    throw new ApplicationException(string.Format("控件ControlId [{0}]，在SysPageControl表中存在，但没有真实控件与之对应", control.ControlId));
                }
                King.Framework.EntityLibrary.SysPageControl control2 = pageControlDic[control.ControlId] as King.Framework.EntityLibrary.SysPageControl;
                control2.ControlName = control.ControlName;
                control2.ControlType = control.ControlType;
                control2.DisplayControlType = control.DisplayControlType;
                control2.EnableConditionId = control.EnableConditionId;
                control2.EntityId = control.EntityId;
                control2.IsHidden = control.IsHidden;
                control2.IsLock = control.IsLock;
                control2.OrderIndex = control.OrderIndex;
                control2.PageId = control.PageId;
                control2.ParentId = control.ParentId;
                control2.VisibleConditionId = control.VisibleConditionId;
            }
            this._cache[typeof(King.Framework.EntityLibrary.SysPageControl)] = pageControlDic;
        }

        public List<T> Where<T>(Func<T, bool> predicate)
        {
            Type key = typeof(T);
            if (this._cache.ContainsKey(key))
            {
                Dictionary<long, ICachedEntity> dictionary = this._cache[key];
                return dictionary.Values.Cast<T>().Where<T>(predicate).ToList<T>();
            }
            return null;
        }

        public List<King.Framework.EntityLibrary.SysAccordion> SysAccordion
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysAccordion>();
            }
        }

        public List<King.Framework.EntityLibrary.SysActivityDataSection> SysActivityDataSection
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysActivityDataSection>();
            }
        }

        public List<King.Framework.EntityLibrary.SysApproveHistory> SysApproveHistory
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysApproveHistory>();
            }
        }

        public List<King.Framework.EntityLibrary.SysApproveItem> SysApproveItem
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysApproveItem>();
            }
        }

        public List<King.Framework.EntityLibrary.SysAttachmentList> SysAttachmentList
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysAttachmentList>();
            }
        }

        public List<King.Framework.EntityLibrary.SysBusinessLogicOperation> SysBusinessLogicOperation
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysBusinessLogicOperation>();
            }
        }

        public List<King.Framework.EntityLibrary.SysCondition> SysCondition
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysCondition>();
            }
        }

        public List<King.Framework.EntityLibrary.SysConditionBox> SysConditionBox
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysConditionBox>();
            }
        }

        public List<King.Framework.EntityLibrary.SysDbSchema> SysDbSchema
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysDbSchema>();
            }
        }

        public List<King.Framework.EntityLibrary.SysEntity> SysEntity
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysEntity>();
            }
        }

        public List<King.Framework.EntityLibrary.SysEntityCategory> SysEntityCategory
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysEntityCategory>();
            }
        }

        public List<King.Framework.EntityLibrary.SysEntityJoinRelation> SysEntityJoinRelation
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysEntityJoinRelation>();
            }
        }

        public List<King.Framework.EntityLibrary.SysEntitySchema> SysEntitySchema
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysEntitySchema>();
            }
        }

        public List<King.Framework.EntityLibrary.SysEnum> SysEnum
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysEnum>();
            }
        }

        public List<King.Framework.EntityLibrary.SysEnumItem> SysEnumItem
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysEnumItem>();
            }
        }

        public List<King.Framework.EntityLibrary.SysField> SysField
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysField>();
            }
        }

        public List<King.Framework.EntityLibrary.SysFieldMultiRef> SysFieldMultiRef
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysFieldMultiRef>();
            }
        }

        public List<King.Framework.EntityLibrary.SysFormTab> SysFormTab
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysFormTab>();
            }
        }

        public List<King.Framework.EntityLibrary.SysFrameControl> SysFrameControl
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysFrameControl>();
            }
        }

        public List<King.Framework.EntityLibrary.SysFunction> SysFunction
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysFunction>();
            }
        }

        public List<King.Framework.EntityLibrary.SysHeaderBox> SysHeaderBox
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysHeaderBox>();
            }
        }

        public List<King.Framework.EntityLibrary.SysMobileApproveComment> SysMobileApproveComment
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysMobileApproveComment>();
            }
        }

        public List<King.Framework.EntityLibrary.SysMobileApproveDeal> SysMobileApproveDeal
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysMobileApproveDeal>();
            }
        }

        public List<King.Framework.EntityLibrary.SysMobilePhotoList> SysMobilePhotoList
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysMobilePhotoList>();
            }
        }

        public List<King.Framework.EntityLibrary.SysMobileStepEntity> SysMobileStepEntity
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysMobileStepEntity>();
            }
        }

        public List<King.Framework.EntityLibrary.SysModule> SysModule
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysModule>();
            }
        }

        public List<King.Framework.EntityLibrary.SysMoreMoreRelation> SysMoreMoreRelation
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysMoreMoreRelation>();
            }
        }

        public List<King.Framework.EntityLibrary.SysOneMoreRelation> SysOneMoreRelation
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysOneMoreRelation>();
            }
        }

        public List<King.Framework.EntityLibrary.SysOperationBar> SysOperationBar
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysOperationBar>();
            }
        }

        public List<King.Framework.EntityLibrary.SysOperationConditionRedirect> SysOperationConditionRedirect
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysOperationConditionRedirect>();
            }
        }

        public List<King.Framework.EntityLibrary.SysPage> SysPage
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysPage>();
            }
        }

        public List<King.Framework.EntityLibrary.SysPageControl> SysPageControl
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysPageControl>();
            }
        }

        public List<King.Framework.EntityLibrary.SysPageField> SysPageField
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysPageField>();
            }
        }

        public List<King.Framework.EntityLibrary.SysPageOperation> SysPageOperation
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysPageOperation>();
            }
        }

        public List<King.Framework.EntityLibrary.SysPageParameter> SysPageParameter
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysPageParameter>();
            }
        }

        public List<King.Framework.EntityLibrary.SysPageSection> SysPageSection
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysPageSection>();
            }
        }

        public List<King.Framework.EntityLibrary.SysParentViewControl> SysParentViewControl
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysParentViewControl>();
            }
        }

        public List<King.Framework.EntityLibrary.SysPatchMatchField> SysPatchMatchField
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysPatchMatchField>();
            }
        }

        public List<King.Framework.EntityLibrary.SysQueryCondition> SysQueryCondition
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysQueryCondition>();
            }
        }

        public List<King.Framework.EntityLibrary.SysQueryConditionRelatedField> SysQueryConditionRelatedField
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysQueryConditionRelatedField>();
            }
        }

        public List<King.Framework.EntityLibrary.SysRedirectPageParaMeterSet> SysRedirectPageParaMeterSet
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysRedirectPageParaMeterSet>();
            }
        }

        public List<King.Framework.EntityLibrary.SysRefControlRelatedField> SysRefControlRelatedField
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysRefControlRelatedField>();
            }
        }

        public List<King.Framework.EntityLibrary.SysRibbonBar> SysRibbonBar
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysRibbonBar>();
            }
        }

        public List<King.Framework.EntityLibrary.SysRibbonBarItem> SysRibbonBarItem
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysRibbonBarItem>();
            }
        }

        public List<King.Framework.EntityLibrary.SysRowOperation> SysRowOperation
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysRowOperation>();
            }
        }

        public List<King.Framework.EntityLibrary.SysStackPanel> SysStackPanel
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysStackPanel>();
            }
        }

        public List<King.Framework.EntityLibrary.SysSubSystem> SysSubSystem
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysSubSystem>();
            }
        }

        public List<SysSubSystemEntity> SysSubSystemEntitys
        {
            get
            {
                return this.GetCache<SysSubSystemEntity>();
            }
        }

        public List<King.Framework.EntityLibrary.SysSubSystemPages> SysSubSystemPages
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysSubSystemPages>();
            }
        }

        public List<King.Framework.EntityLibrary.SysTabPage> SysTabPage
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysTabPage>();
            }
        }

        public List<King.Framework.EntityLibrary.SysTree> SysTree
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysTree>();
            }
        }

        public List<King.Framework.EntityLibrary.SysUniqueKey> SysUniqueKey
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysUniqueKey>();
            }
        }

        public List<King.Framework.EntityLibrary.SysUniqueKeyField> SysUniqueKeyField
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysUniqueKeyField>();
            }
        }

        public List<King.Framework.EntityLibrary.SysUserControl> SysUserControl
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysUserControl>();
            }
        }

        public List<King.Framework.EntityLibrary.SysView> SysView
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysView>();
            }
        }

        public List<King.Framework.EntityLibrary.SysViewCondition> SysViewCondition
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysViewCondition>();
            }
        }

        public List<King.Framework.EntityLibrary.SysViewControl> SysViewControl
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysViewControl>();
            }
        }

        public List<King.Framework.EntityLibrary.SysViewField> SysViewField
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysViewField>();
            }
        }

        public List<King.Framework.EntityLibrary.SysViewList> SysViewList
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysViewList>();
            }
        }

        public List<King.Framework.EntityLibrary.SysViewListItem> SysViewListItem
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysViewListItem>();
            }
        }

        public List<King.Framework.EntityLibrary.SysViewOrderField> SysViewOrderField
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysViewOrderField>();
            }
        }

        public List<King.Framework.EntityLibrary.SysViewSumFields> SysViewSumFields
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysViewSumFields>();
            }
        }
    }
}

