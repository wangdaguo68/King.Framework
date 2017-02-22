namespace King.Framework.EntityLibrary
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public abstract class EntityCacheBase
    {
        private static Dictionary<Type, Dictionary<string, object>> _cache = new Dictionary<Type, Dictionary<string, object>>();
        private static List<EntityCacheBase> _entityCacheList = new List<EntityCacheBase>();
        private static Dictionary<long, Type> _entityTypeCache = new Dictionary<long, Type>();

        protected EntityCacheBase()
        {
            if (_cache.Count == 0)
            {
                this.Init_SysEntity();
                this.Init_SysField();
                this.Init_SysOneMoreRelation();
                this.Init_SysMoreMoreRelation();
                this.Init_SysPage();
                this.Init_SysCondition();
                this.Init_SysModule();
                this.Init_SysEntityCategory();
                this.Init_SysEnum();
                this.Init_SysEnumItem();
                this.Init_SysMobileStepEntity();
                this.Init_SysUniqueKey();
                this.Init_SysUniqueKeyField();
                this.InitCacheRelations();
            }
        }

        protected void Add(object id, object obj)
        {
            Type key = obj.GetType();
            if (!_cache.ContainsKey(key))
            {
                _cache.Add(key, new Dictionary<string, object>());
            }
            _cache[key][Convert.ToString(id)] = obj;
        }

        protected void AddEntityType(long id, Type type)
        {
            _entityTypeCache[id] = type;
        }

        public T FindById<T>(object id)
        {
            if (id != null)
            {
                Type type = typeof(T);
                Dictionary<string, object> dictionary = _cache[type];
                if (dictionary.ContainsKey(Convert.ToString(id)))
                {
                    return (T) dictionary[Convert.ToString(id)];
                }
            }
            return default(T);
        }

        private List<T> GetCache<T>()
        {
            Func<KeyValuePair<string, object>, T> selector = null;
            Type key = typeof(T);
            if (_cache.ContainsKey(key))
            {
                Dictionary<string, object> source = _cache[key];
                if (selector == null)
                {
                    selector = p => (T) p.Value;
                }
                return source.Select<KeyValuePair<string, object>, T>(selector).ToList<T>();
            }
            return new List<T>();
        }

        public Type GetEntityType(long entityId)
        {
            if (!_entityTypeCache.ContainsKey(entityId))
            {
                throw new Exception(string.Format("不存在SysEntity的ID为{0}对应的实体类型", entityId));
            }
            return _entityTypeCache[entityId];
        }

        protected abstract void Init_SysCondition();
        protected abstract void Init_SysEntity();
        protected abstract void Init_SysEntityCategory();
        protected abstract void Init_SysEnum();
        protected abstract void Init_SysEnumItem();
        protected abstract void Init_SysField();
        protected abstract void Init_SysMobileStepEntity();
        protected abstract void Init_SysModule();
        protected abstract void Init_SysMoreMoreRelation();
        protected abstract void Init_SysOneMoreRelation();
        protected abstract void Init_SysPage();
        protected abstract void Init_SysUniqueKey();
        protected abstract void Init_SysUniqueKeyField();
        private void InitCacheRelations()
        {
            foreach (King.Framework.EntityLibrary.SysEntity entity in this.SysEntity)
            {
                entity.Fields = new List<King.Framework.EntityLibrary.SysField>();
                entity.ParentMoreMoreRelations = new List<King.Framework.EntityLibrary.SysMoreMoreRelation>();
                entity.ParentOneMoreRelations = new List<King.Framework.EntityLibrary.SysOneMoreRelation>();
                entity.ChildMoreMoreRelations = new List<King.Framework.EntityLibrary.SysMoreMoreRelation>();
                entity.ChildOneMoreRelations = new List<King.Framework.EntityLibrary.SysOneMoreRelation>();
                entity.Conditions = new List<King.Framework.EntityLibrary.SysCondition>();
                entity.Pages = new List<King.Framework.EntityLibrary.SysPage>();
                entity.UniqueKeys = new List<King.Framework.EntityLibrary.SysUniqueKey>();
            }
            foreach (King.Framework.EntityLibrary.SysCondition condition in this.SysCondition)
            {
                condition.ChildConditions = new List<King.Framework.EntityLibrary.SysCondition>();
            }
            foreach (King.Framework.EntityLibrary.SysModule module in this.SysModule)
            {
                module.Entities = new List<King.Framework.EntityLibrary.SysEntity>();
            }
            foreach (King.Framework.EntityLibrary.SysEntityCategory category in this.SysEntityCategory)
            {
                category.Modules = new List<King.Framework.EntityLibrary.SysModule>();
            }
            foreach (King.Framework.EntityLibrary.SysEnum enum2 in this.SysEnum)
            {
                enum2.EnumItems = new List<King.Framework.EntityLibrary.SysEnumItem>();
            }
            foreach (King.Framework.EntityLibrary.SysUniqueKey key in this.SysUniqueKey)
            {
                key.UniqueKeyFields = new List<King.Framework.EntityLibrary.SysUniqueKeyField>();
            }
            foreach (King.Framework.EntityLibrary.SysEntity entity in this.SysEntity)
            {
                entity.OwnerModule = this.FindById<King.Framework.EntityLibrary.SysModule>(entity.ModuleId);
                if (entity.OwnerModule != null)
                {
                    entity.OwnerModule.Entities.Add(entity);
                }
            }
            foreach (King.Framework.EntityLibrary.SysField field in this.SysField)
            {
                field.OwnerEntity = this.FindById<King.Framework.EntityLibrary.SysEntity>(field.EntityId);
                field.RefEntity = this.FindById<King.Framework.EntityLibrary.SysEntity>(field.RefEntityId);
                field.RefRelation = this.FindById<King.Framework.EntityLibrary.SysOneMoreRelation>(field.RefRelationId);
                field.RefEnum = this.FindById<King.Framework.EntityLibrary.SysEnum>(field.EnumId);
                if (field.OwnerEntity != null)
                {
                    field.OwnerEntity.Fields.Add(field);
                }
            }
            foreach (King.Framework.EntityLibrary.SysOneMoreRelation relation in this.SysOneMoreRelation)
            {
                relation.ChildEntity = this.FindById<King.Framework.EntityLibrary.SysEntity>(relation.ChildEntityId);
                relation.ParentEntity = this.FindById<King.Framework.EntityLibrary.SysEntity>(relation.ParentEntityId);
                relation.ChildField = this.FindById<King.Framework.EntityLibrary.SysField>(relation.ChildFieldId);
                relation.ParentField = this.FindById<King.Framework.EntityLibrary.SysField>(relation.ParentFieldId);
                if (relation.ChildEntity != null)
                {
                    relation.ChildEntity.ChildOneMoreRelations.Add(relation);
                }
                if (relation.ParentEntity != null)
                {
                    relation.ParentEntity.ParentOneMoreRelations.Add(relation);
                }
            }
            foreach (King.Framework.EntityLibrary.SysMoreMoreRelation relation2 in this.SysMoreMoreRelation)
            {
                relation2.ChildEntity = this.FindById<King.Framework.EntityLibrary.SysEntity>(relation2.ChildEntityId);
                relation2.ParentEntity = this.FindById<King.Framework.EntityLibrary.SysEntity>(relation2.ParentEntityId);
                if (relation2.ChildEntity != null)
                {
                    relation2.ChildEntity.ChildMoreMoreRelations.Add(relation2);
                }
                if (relation2.ParentEntity != null)
                {
                    relation2.ParentEntity.ParentMoreMoreRelations.Add(relation2);
                }
            }
            foreach (King.Framework.EntityLibrary.SysPage page in this.SysPage)
            {
                page.OwnerModule = this.FindById<King.Framework.EntityLibrary.SysModule>(page.ModuleId);
                page.OwnerEntity = this.FindById<King.Framework.EntityLibrary.SysEntity>(page.EntityId);
                if (page.OwnerEntity != null)
                {
                    page.OwnerEntity.Pages.Add(page);
                }
            }
            foreach (King.Framework.EntityLibrary.SysCondition condition in this.SysCondition)
            {
                condition.Field = this.FindById<King.Framework.EntityLibrary.SysField>(condition.FieldId);
                condition.RefRelation = this.FindById<King.Framework.EntityLibrary.SysOneMoreRelation>(condition.RelationId);
                condition.OwnerEntity = this.FindById<King.Framework.EntityLibrary.SysEntity>(condition.EntityId);
                if (condition.OwnerEntity != null)
                {
                    condition.OwnerEntity.Conditions.Add(condition);
                }
                condition.ParentCondition = this.FindById<King.Framework.EntityLibrary.SysCondition>(condition.ParentId);
                if (condition.ParentCondition != null)
                {
                    condition.ParentCondition.ChildConditions.Add(condition);
                }
            }
            foreach (King.Framework.EntityLibrary.SysModule module in this.SysModule)
            {
                module.EntityCategory = this.FindById<King.Framework.EntityLibrary.SysEntityCategory>(module.CategoryId);
                if (module.EntityCategory != null)
                {
                    module.EntityCategory.Modules.Add(module);
                }
            }
            foreach (King.Framework.EntityLibrary.SysEnumItem item in this.SysEnumItem)
            {
                item.OwnerEnum = this.FindById<King.Framework.EntityLibrary.SysEnum>(item.EnumId);
                if (item.OwnerEnum != null)
                {
                    item.OwnerEnum.EnumItems.Add(item);
                }
            }
            foreach (King.Framework.EntityLibrary.SysMobileStepEntity entity2 in this.SysMobileStepEntity)
            {
                entity2.OwnerEntity = this.FindById<King.Framework.EntityLibrary.SysEntity>(entity2.EntityId);
            }
            foreach (King.Framework.EntityLibrary.SysUniqueKeyField field2 in this.SysUniqueKeyField)
            {
                field2.OwnerUniqueKey = this.FindById<King.Framework.EntityLibrary.SysUniqueKey>(field2.UniqueKeyId);
                field2.SysField = this.FindById<King.Framework.EntityLibrary.SysField>(field2.FieldId);
                if (field2.OwnerUniqueKey != null)
                {
                    field2.OwnerUniqueKey.UniqueKeyFields.Add(field2);
                }
            }
            foreach (King.Framework.EntityLibrary.SysUniqueKey key in this.SysUniqueKey)
            {
                key.SysEntity = this.FindById<King.Framework.EntityLibrary.SysEntity>(key.EntityId);
                if (key.SysEntity != null)
                {
                    key.SysEntity.UniqueKeys.Add(key);
                }
            }
        }

        public static EntityCacheBase LoadCache()
        {
            string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase.Substring(8));
            if (!directoryName.Contains(@":\"))
            {
                directoryName = "/" + directoryName;
            }
            return LoadCache(Directory.GetFiles(directoryName).FirstOrDefault<string>(p => Path.GetFileName(p).StartsWith("King.Framework.Entity.Auto")));
        }

        public static EntityCacheBase LoadCache(string dll)
        {
            if (!(!string.IsNullOrEmpty(dll) && File.Exists(dll)))
            {
                throw new ApplicationException("未能在目录下找到King.Framework.Entity.Auto.dll，加载EntityCache失败");
            }
            EntityCacheBase item = _entityCacheList.FirstOrDefault<EntityCacheBase>(p => p.DllAssemblyPath == dll);
            if (item == null)
            {
                Type type = Assembly.LoadFrom(dll).GetType("King.Framework.Entity.EntityCache");
                if (type == null)
                {
                    throw new ApplicationException("未能在King.Framework.Entity.Auto.dll下找到EntityCache类，加载EntityCache失败");
                }
                item = Activator.CreateInstance(type) as EntityCacheBase;
                item.DllAssemblyPath = dll;
                _entityCacheList.Add(item);
            }
            return item;
        }

        public List<T> Set<T>()
        {
            return this.GetCache<T>();
        }

        public List<T> Where<T>(Func<T, bool> predicate)
        {
            Type key = typeof(T);
            if (_cache.ContainsKey(key))
            {
                Dictionary<string, object> dictionary = _cache[key];
                return dictionary.Values.Cast<T>().Where<T>(predicate).ToList<T>();
            }
            return new List<T>();
        }

        public string DllAssemblyPath { get; set; }

        public List<King.Framework.EntityLibrary.SysCondition> SysCondition
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysCondition>();
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

        public List<King.Framework.EntityLibrary.SysPage> SysPage
        {
            get
            {
                return this.GetCache<King.Framework.EntityLibrary.SysPage>();
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
    }
}

