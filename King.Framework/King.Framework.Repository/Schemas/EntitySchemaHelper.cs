namespace King.Framework.Repository.Schemas
{
    using King.Framework.Common;
    using King.Framework.EntityLibrary;
    using King.Framework.DAL;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class EntitySchemaHelper
    {
        private static readonly Dictionary<long, IEntitySchema> _schemaIdDict = new Dictionary<long, IEntitySchema>();
        private static readonly Dictionary<string, IEntitySchema> _schemaNameDict = new Dictionary<string, IEntitySchema>();
        private static readonly Dictionary<Type, IEntitySchema> _schemaTypeDict = new Dictionary<Type, IEntitySchema>();
        private static Dictionary<DataTypeEnum, Dictionary<bool, Type>> dataTypes = new Dictionary<DataTypeEnum, Dictionary<bool, Type>>();

        static EntitySchemaHelper()
        {
            InitDataTypes();
            LoadEntitySchema();
        }

        private static void Add(IEntitySchema es)
        {
            _schemaIdDict[es.EntityId] = es;
            _schemaNameDict[es.EntityName] = es;
            _schemaTypeDict[es.EntityType] = es;
            TableCache.AppendTableType(es.EntityName, es.EntityType);
        }

        public static IEntitySchema GetEntitySchema(long entityId)
        {
            if (!_schemaIdDict.ContainsKey(entityId))
            {
                throw new ApplicationException(string.Format("不存在EntitySchema,EntityId:{0}", entityId));
            }
            return _schemaIdDict[entityId];
        }

        public static IEntitySchema GetEntitySchema(string entityName)
        {
            if (!_schemaNameDict.ContainsKey(entityName))
            {
                throw new ApplicationException(string.Format("不存在EntitySchema,EntityName:{0}", entityName));
            }
            return _schemaNameDict[entityName];
        }

        public static IEntitySchema GetEntitySchema(Type type)
        {
            if (!_schemaTypeDict.ContainsKey(type))
            {
                throw new ApplicationException(string.Format("不存在EntitySchema,type:{0}", type.FullName));
            }
            return _schemaTypeDict[type];
        }

        private static void InitDataTypes()
        {
            dataTypes[DataTypeEnum.pkey] = new Dictionary<bool, Type>();
            dataTypes[DataTypeEnum.pkey][true] = typeof(int);
            dataTypes[DataTypeEnum.pkey][false] = typeof(int);
            dataTypes[DataTypeEnum.pint] = new Dictionary<bool, Type>();
            dataTypes[DataTypeEnum.pint][true] = typeof(int?);
            dataTypes[DataTypeEnum.pint][false] = typeof(int);
            dataTypes[DataTypeEnum.pstring] = new Dictionary<bool, Type>();
            dataTypes[DataTypeEnum.pstring][true] = typeof(string);
            dataTypes[DataTypeEnum.pstring][false] = typeof(string);
            dataTypes[DataTypeEnum.pdatetime] = new Dictionary<bool, Type>();
            dataTypes[DataTypeEnum.pdatetime][true] = typeof(DateTime?);
            dataTypes[DataTypeEnum.pdatetime][false] = typeof(DateTime);
            dataTypes[DataTypeEnum.pdecimal] = new Dictionary<bool, Type>();
            dataTypes[DataTypeEnum.pdecimal][true] = typeof(decimal?);
            dataTypes[DataTypeEnum.pdecimal][false] = typeof(decimal);
            dataTypes[DataTypeEnum.pfloat] = new Dictionary<bool, Type>();
            dataTypes[DataTypeEnum.pfloat][true] = typeof(decimal?);
            dataTypes[DataTypeEnum.pfloat][false] = typeof(decimal);
            dataTypes[DataTypeEnum.ptext] = new Dictionary<bool, Type>();
            dataTypes[DataTypeEnum.ptext][true] = typeof(string);
            dataTypes[DataTypeEnum.ptext][false] = typeof(string);
            dataTypes[DataTypeEnum.plong] = new Dictionary<bool, Type>();
            dataTypes[DataTypeEnum.plong][true] = typeof(long?);
            dataTypes[DataTypeEnum.plong][false] = typeof(long);
            dataTypes[DataTypeEnum.pbinary] = new Dictionary<bool, Type>();
            dataTypes[DataTypeEnum.pbinary][true] = typeof(byte[]);
            dataTypes[DataTypeEnum.pbinary][false] = typeof(byte[]);
            dataTypes[DataTypeEnum.pfile] = new Dictionary<bool, Type>();
            dataTypes[DataTypeEnum.pfile][true] = typeof(int?);
            dataTypes[DataTypeEnum.pfile][false] = typeof(int);
            dataTypes[DataTypeEnum.pbool] = new Dictionary<bool, Type>();
            dataTypes[DataTypeEnum.pbool][true] = typeof(bool?);
            dataTypes[DataTypeEnum.pbool][false] = typeof(bool);
            dataTypes[DataTypeEnum.pref] = new Dictionary<bool, Type>();
            dataTypes[DataTypeEnum.pref][true] = typeof(int?);
            dataTypes[DataTypeEnum.pref][false] = typeof(int);
            dataTypes[DataTypeEnum.penum] = new Dictionary<bool, Type>();
            dataTypes[DataTypeEnum.penum][true] = typeof(int?);
            dataTypes[DataTypeEnum.penum][false] = typeof(int);
            dataTypes[DataTypeEnum.ppassword] = new Dictionary<bool, Type>();
            dataTypes[DataTypeEnum.ppassword][true] = typeof(string);
            dataTypes[DataTypeEnum.ppassword][false] = typeof(string);
            dataTypes[DataTypeEnum.MultiRef] = new Dictionary<bool, Type>();
            dataTypes[DataTypeEnum.MultiRef][true] = typeof(string);
            dataTypes[DataTypeEnum.MultiRef][false] = typeof(string);
        }

        private static void LoadEntitySchema()
        {
            EntityCacheBase cache = EntityCacheBase.LoadCache();
            using (List<SysEntity>.Enumerator enumerator = cache.SysEntity.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Func<SysMobileStepEntity, bool> predicate = null;
                    SysEntity e = enumerator.Current;
                    EntitySchema es = new EntitySchema();
                    bool? isHistory = e.IsHistory;
                    es.IsHistory = isHistory.HasValue ? isHistory.GetValueOrDefault() : false;
                    SysOneMoreRelation relation = e.ChildOneMoreRelations.FirstOrDefault<SysOneMoreRelation>(p => p.Is_Tree == true);
                    if (relation != null)
                    {
                        es.TreeRelationFieldName = relation.ChildField.FieldName;
                    }
                    es.IsChangeTableVersion = false;
                    if (predicate == null)
                    {
                        predicate = delegate (SysMobileStepEntity p) {
                            long? entityId = p.EntityId;
                            long num = e.EntityId;
                            return (entityId.GetValueOrDefault() == num) && entityId.HasValue;
                        };
                    }
                    if (cache.SysMobileStepEntity.Where<SysMobileStepEntity>(predicate).Count<SysMobileStepEntity>() > 0)
                    {
                        es.IsChangeTableVersion = true;
                    }
                    es.EntityId = e.EntityId;
                    es.EntityName = e.EntityName;
                    es.KeyName = e.GetKeyFieldName();
                    es.EntityType = cache.GetEntityType(e.EntityId);
                    es.DisplayName = e.GetDisplayFieldName();
                    int? privilegeMode = e.PrivilegeMode;
                    es.PrivilegeMode = privilegeMode.HasValue ? privilegeMode.GetValueOrDefault() : -2147483647;
                    privilegeMode = e.RequiredLevel;
                    es.RequiredLevel = privilegeMode.HasValue ? privilegeMode.GetValueOrDefault() : -2147483647;
                    LoadUniqueKey(es, e, cache);
                    LoadFields(es, e, cache);
                    LoadRelations(es, e, cache);
                    Add(es);
                }
            }
        }

        private static void LoadFields(EntitySchema es, SysEntity entity, EntityCacheBase cache)
        {
            foreach (SysField field in entity.Fields)
            {
                bool? isNullable;
                es.Fields.Add(field);
                if (field.DataType == 1)
                {
                    isNullable = field.IsNullable;
                    es.PropertyTypes.Add(es.KeyName, dataTypes[(DataTypeEnum)field.DataType.Value][isNullable.HasValue ? isNullable.GetValueOrDefault() : false]);
                    es.PropertyTypeEnums.Add(es.KeyName, field.DataType.Value);
                }
                else
                {
                    if (!string.IsNullOrEmpty(field.DefaultValue))
                    {
                        es.DefaultValues.Add(field.FieldName, field.DefaultValue);
                    }
                    isNullable = field.IsNullable;
                    es.PropertyTypes.Add(field.FieldName, dataTypes[(DataTypeEnum)field.DataType.Value][isNullable.HasValue ? isNullable.GetValueOrDefault() : false]);
                    es.PropertyTypeEnums.Add(field.FieldName, field.DataType.Value);
                    if (field.DataType == 10)
                    {
                        es.FilePropertys.Add(field.FieldName);
                    }
                }
            }
        }

        private static void LoadRelations(EntitySchema es, SysEntity entity, EntityCacheBase cache)
        {
            List<SysMoreMoreRelation> list = cache.SysMoreMoreRelation.Where<SysMoreMoreRelation>(delegate (SysMoreMoreRelation s) {
                long? childEntityId = s.ChildEntityId;
                long entityId = entity.EntityId;
                return (((childEntityId.GetValueOrDefault() == entityId) && childEntityId.HasValue) || (((childEntityId = s.ParentEntityId).GetValueOrDefault() == (entityId = entity.EntityId)) && childEntityId.HasValue));
            }).ToList<SysMoreMoreRelation>();
            foreach (SysMoreMoreRelation relation in list)
            {
                long oid = relation.ParentEntityId.Value;
                if (oid == entity.EntityId)
                {
                    oid = relation.ChildEntityId.Value;
                }
                SysEntity entity2 = (from s in cache.SysEntity
                    where s.EntityId == oid
                    select s).FirstOrDefault<SysEntity>();
                if (entity2 == null)
                {
                    throw new ApplicationException(oid.ToString());
                }
                es.MmTables.Add(entity2.EntityName + "s", relation.TableName);
                ReferencedObject item = new ReferencedObject {
                    EntityType = cache.GetEntityType(entity2.EntityId),
                    ReferenceField = entity2.EntityName + "s",
                    DeleteFlag = DeleteFlag.SetNull,
                    ReferenceType = ReferenceType.MoreMore
                };
                es.ReferencedObjectList.Add(item);
            }
        }

        private static void LoadUniqueKey(EntitySchema es, SysEntity entity, EntityCacheBase cache)
        {
            foreach (SysUniqueKey key in entity.UniqueKeys)
            {
                string str = string.Format("{0}_id_{1}", key.UniqueKeyName, key.UniqueKeyId);
                List<string> list = (from p in key.UniqueKeyFields select p.SysField.FieldName).ToList<string>();
                es.UniqueKeyDict[str] = list;
            }
        }
    }
}

