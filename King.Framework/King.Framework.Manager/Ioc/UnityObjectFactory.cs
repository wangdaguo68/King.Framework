namespace King.Framework.Manager.Ioc
{
    using King.Framework.Repository.Schemas;
    using System;
    using System.Collections.Generic;

    public class UnityObjectFactory
    {
        private static readonly Dictionary<string, Type> _viewQueryDict = new Dictionary<string, Type>();

        public void AppendViewQueryCache(Type type)
        {
            _viewQueryDict[type.Name] = type;
        }

        public static IEntitySchema GetEntitySchema(long entityId)
        {
            return EntitySchemaHelper.GetEntitySchema(entityId);
        }

        public static IEntitySchema GetEntitySchema(string entityName)
        {
            return EntitySchemaHelper.GetEntitySchema(entityName);
        }

        public static IEntitySchema GetEntitySchema(Type type)
        {
            return EntitySchemaHelper.GetEntitySchema(type);
        }

        public static Type GetViewQueryType(string className)
        {
            if (!_viewQueryDict.ContainsKey(className))
            {
                throw new ApplicationException(string.Format("无法找到名称为\"{0}\"的查询类", className));
            }
            return _viewQueryDict[className];
        }

        public bool IsCached()
        {
            return (_viewQueryDict.Count > 0);
        }
    }
}
