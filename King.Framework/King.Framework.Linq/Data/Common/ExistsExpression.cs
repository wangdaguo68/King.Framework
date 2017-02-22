namespace King.Framework.Linq.Data.Common
{
    using System;

    public class ExistsExpression : SubqueryExpression
    {
        public ExistsExpression(SelectExpression select) : base(DbExpressionType.Exists, typeof(bool), select)
        {
        }
    }
}
