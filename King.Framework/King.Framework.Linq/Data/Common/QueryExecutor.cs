namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Collections.Generic;

    public abstract class QueryExecutor
    {
        protected QueryExecutor()
        {
        }

        public abstract object Convert(object value, Type type);
        public abstract IEnumerable<T> Execute<T>(QueryCommand command, Func<FieldReader, T> fnProjector, MappingEntity entity, object[] paramValues);
        public abstract IEnumerable<int> ExecuteBatch(QueryCommand query, IEnumerable<object[]> paramSets, int batchSize, bool stream);
        public abstract IEnumerable<T> ExecuteBatch<T>(QueryCommand query, IEnumerable<object[]> paramSets, Func<FieldReader, T> fnProjector, MappingEntity entity, int batchSize, bool stream);
        public abstract int ExecuteCommand(QueryCommand query, object[] paramValues);
        public abstract IEnumerable<T> ExecuteDeferred<T>(QueryCommand query, Func<FieldReader, T> fnProjector, MappingEntity entity, object[] paramValues);

        public abstract int RowsAffected { get; }
    }
}
