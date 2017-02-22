namespace King.Framework.Linq.Data.Common
{
    using System;

    public abstract class QueryTypeSystem
    {
        protected QueryTypeSystem()
        {
        }

        public abstract QueryType GetColumnType(Type type);
        public abstract string GetVariableDeclaration(QueryType type, bool suppressSize);
        public abstract QueryType Parse(string typeDeclaration);
    }
}
