namespace King.Framework.Interfaces
{
    using System;
    using System.Collections.Generic;

    public interface IMetaEntity
    {
        IEnumerable<IMetaField> GetFields();

        string DisplayName { get; }

        long EntityId { get; }

        string EntityName { get; }

        string KeyName { get; }
    }
}

