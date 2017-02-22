namespace King.Framework.Repository.Schemas
{
    using King.Framework.Common;
    using King.Framework.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class EntitySchema : IEntitySchema, IMetaEntity
    {
        private readonly List<IMetaField> _fields = new List<IMetaField>();

        public EntitySchema()
        {
            this.PropertyTypes = new Dictionary<string, Type>();
            this.PropertyTypeEnums = new Dictionary<string, int>();
            this.MmTables = new Dictionary<string, string>();
            this.ReferencedObjectList = new List<ReferencedObject>();
            this.DefaultValues = new Dictionary<string, string>();
            this.FilePropertys = new List<string>();
            this.UniqueKeyDict = new Dictionary<string, List<string>>();
        }

        public object CreateInstance()
        {
            return Activator.CreateInstance(this.EntityType);
        }

        public string GetDisplayValue(object model)
        {
            return Convert.ToString(model.GetPropertyValue(this.DisplayName, this.EntityType));
        }

        public IEnumerable<IMetaField> GetFields()
        {
            return this._fields;
        }

        public int GetKeyValue(object model)
        {
            return Convert.ToInt32(model.GetPropertyValue(this.KeyName, this.EntityType));
        }

        public Dictionary<string, string> DefaultValues { get; set; }

        public string DisplayName { get; set; }

        public long EntityId { get; set; }

        public string EntityName { get; set; }

        public Type EntityType { get; set; }

        public List<IMetaField> Fields
        {
            get
            {
                return this._fields;
            }
        }

        public List<string> FilePropertys { get; set; }

        public bool IsChangeTableVersion { get; set; }

        public bool IsHistory { get; set; }

        public string KeyName { get; set; }

        public Dictionary<string, string> MmTables { get; set; }

        public int PrivilegeMode { get; set; }

        public Dictionary<string, int> PropertyTypeEnums { get; set; }

        public Dictionary<string, Type> PropertyTypes { get; set; }

        public List<ReferencedObject> ReferencedObjectList { get; set; }

        public int RequiredLevel { get; set; }

        public string TreeRelationFieldName { get; set; }

        public Dictionary<string, List<string>> UniqueKeyDict { get; set; }
    }
}

