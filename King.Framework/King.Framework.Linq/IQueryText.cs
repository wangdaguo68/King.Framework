namespace King.Framework.Linq
{
    using System;
    using System.Linq.Expressions;

    public interface IQueryText
    {
        string GetQueryText(Expression expression);
    }
}
