namespace King.Framework.WorkflowEngineCore.Cache
{
    using King.Framework.Common;
    using King.Framework.EntityLibrary;
    using King.Framework.WorkflowEngineCore.InternalCore;
    using System;
    using System.Collections.Generic;

    internal class EntityCache
    {
        private Dictionary<string, EntityData> _dict = new Dictionary<string, EntityData>(10);
        private Dictionary<string, EntityData> _dictFull = new Dictionary<string, EntityData>();
        private readonly DynamicManager _manager;

        public EntityCache(DynamicManager dm)
        {
            this._manager = dm;
        }

        private string GetKey(long entity_id, int object_id)
        {
            return string.Format("{0}${1}", entity_id, object_id);
        }

        public EntityData GetObject(SysEntity entity, int object_id)
        {
            long entityId = entity.EntityId;
            string key = this.GetKey(entityId, object_id);
            if (this._dict.ContainsKey(key))
            {
                return this._dict[key];
            }
            EntityData data = this.Load(entity, object_id);
            if (data != null)
            {
                this._dict.Add(key, data);
            }
            return data;
        }

        public string GetObjectDisplayName(SysEntity entity, int object_id)
        {
            EntityData data = this.GetObject(entity, object_id);
            if (data != null)
            {
                string displayFieldName = entity.GetDisplayFieldName();
                if (data.ContainsKey(displayFieldName))
                {
                    return Convert.ToString(data[displayFieldName]);
                }
            }
            return null;
        }

        public EntityData GetObjectFull(SysEntity entity, int object_id, ProcessInstanceCacheFactory piCacheFactory)
        {
            long entityId = entity.EntityId;
            string key = this.GetKey(entityId, object_id);
            if (this._dictFull.ContainsKey(key))
            {
                return this._dictFull[key];
            }
            EntityData data = this.LoadFull(entity, object_id, piCacheFactory);
            if (data != null)
            {
                this._dictFull.Add(key, data);
            }
            return data;
        }

        private EntityData Load(SysEntity entity, int object_id)
        {
            return this._manager.LoadWithEntity(entity, object_id);
        }

        private EntityData LoadFull(SysEntity entity, int object_id, ProcessInstanceCacheFactory piCacheFactory)
        {
            return this._manager.LoadFullWithEntity(entity, object_id, piCacheFactory);
        }
    }
}

