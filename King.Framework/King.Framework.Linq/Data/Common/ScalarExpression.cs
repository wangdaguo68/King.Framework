namespace King.Framework.Linq.Data.Common
{
    using System;

    public class ScalarExpression : SubqueryExpression
    {
        public ScalarExpression(Type type, SelectExpression select) : base(DbExpressionType.Scalar, type, select)
        {
        }
    }
}
