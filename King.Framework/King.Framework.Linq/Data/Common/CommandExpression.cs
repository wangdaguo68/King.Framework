namespace King.Framework.Linq.Data.Common
{
    using System;

    public abstract class CommandExpression : DbExpression
    {
        protected CommandExpression(DbExpressionType eType, Type type) : base(eType, type)
        {
        }
    }
}
