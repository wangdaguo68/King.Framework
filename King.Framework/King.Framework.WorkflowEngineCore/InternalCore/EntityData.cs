namespace King.Framework.WorkflowEngineCore.InternalCore
{
    using King.Framework.EntityLibrary;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public class EntityData : IEnumerable<KeyValuePair<string, object>>, IEnumerable
    {
        private Dictionary<string, object> _dict = new Dictionary<string, object>();
        private readonly SysEntity _entity;
        private readonly long _entityId;
        private readonly string _entityName;

        public EntityData(SysEntity entity)
        {
            this._entityName = entity.EntityName;
            this._entityId = entity.EntityId;
            this._entity = entity;
        }

        public bool ContainsKey(string key)
        {
            if (key == null)
            {
                return false;
            }
            return this._dict.ContainsKey(key.ToLower());
        }

        public void Remove(string key)
        {
            string str = key.ToLower();
            if (this._dict.ContainsKey(str))
            {
                this._dict.Remove(str);
            }
        }

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            return this._dict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._dict.GetEnumerator();
        }

        public int Count
        {
            get
            {
                return this._dict.Count;
            }
        }

        public Dictionary<string, object> Dictionary
        {
            get
            {
                return this._dict;
            }
        }

        public SysEntity Entity
        {
            get
            {
                return this._entity;
            }
        }

        public long EntityId
        {
            get
            {
                return this._entityId;
            }
        }

        public string EntityName
        {
            get
            {
                return this._entityName;
            }
        }

        public object this[string key]
        {
            get
            {
                string str = key.ToLower();
                if (this._dict.ContainsKey(str))
                {
                    return this._dict[str];
                }
                return null;
            }
            set
            {
                string str = key.ToLower();
                this._dict[str] = value;
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                return this._dict.Keys;
            }
        }

        public int? ObjectId { get; set; }
    }
}

