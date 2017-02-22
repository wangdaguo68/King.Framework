namespace King.Framework.Linq
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    public interface IEntityProvider : IQueryProvider
    {
        bool CanBeEvaluatedLocally(Expression expression);
        bool CanBeParameter(Expression expression);
        IEntityTable<T> GetTable<T>(string tableId);
        IEntityTable GetTable(Type type, string tableId);
    }
}
