namespace King.Framework.Interfaces
{
    using System;

    public interface IEntityProvider
    {
        IMetaEntity FindById(long entityId);
        IMetaEntity FindByName(string entityName);
    }
}

