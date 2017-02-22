namespace King.Framework.Repository.Schemas
{
    using King.Framework.Interfaces;
    using System;

    public class MetaEntityProvider : IEntityProvider
    {
        public IMetaEntity FindById(long entityId)
        {
            return EntitySchemaHelper.GetEntitySchema(entityId);
        }

        public IMetaEntity FindByName(string entityName)
        {
            return EntitySchemaHelper.GetEntitySchema(entityName);
        }
    }
}

