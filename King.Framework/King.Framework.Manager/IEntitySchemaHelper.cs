namespace King.Framework.Manager
{
    using King.Framework.Manager.Ioc;
    using King.Framework.Repository.Schemas;
    using System;

    public class IEntitySchemaHelper
    {
        public static IEntitySchema Get<T>()
        {
            return Get(typeof(T));
        }

        public static IEntitySchema Get(long entityId)
        {
            return UnityObjectFactory.GetEntitySchema(entityId);
        }

        public static IEntitySchema Get(string entityName)
        {
            return UnityObjectFactory.GetEntitySchema(entityName);
        }

        public static IEntitySchema Get(Type entityType)
        {
            return UnityObjectFactory.GetEntitySchema(entityType);
        }
    }
}
