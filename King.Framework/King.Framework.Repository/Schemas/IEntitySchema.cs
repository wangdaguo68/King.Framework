namespace King.Framework.Repository.Schemas
{
    using King.Framework.Interfaces;
    using System;
    using System.Collections.Generic;

    public interface IEntitySchema : IMetaEntity
    {
        object CreateInstance();
        string GetDisplayValue(object model);
        int GetKeyValue(object model);

        Dictionary<string, string> DefaultValues { get; }

        Type EntityType { get; }

        List<string> FilePropertys { get; }

        bool IsChangeTableVersion { get; }

        bool IsHistory { get; }

        Dictionary<string, string> MmTables { get; }

        int PrivilegeMode { get; }

        Dictionary<string, int> PropertyTypeEnums { get; }

        Dictionary<string, Type> PropertyTypes { get; }

        List<ReferencedObject> ReferencedObjectList { get; }

        int RequiredLevel { get; }

        string TreeRelationFieldName { get; }

        Dictionary<string, List<string>> UniqueKeyDict { get; }
    }
}

