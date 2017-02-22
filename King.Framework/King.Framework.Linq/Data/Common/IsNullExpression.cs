namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Linq.Expressions;

    public class IsNullExpression : DbExpression
    {
        private System.Linq.Expressions.Expression expression;

        public IsNullExpression(System.Linq.Expressions.Expression expression) : base(DbExpressionType.IsNull, typeof(bool))
        {
            this.expression = expression;
        }

        public System.Linq.Expressions.Expression Expression
        {
            get
            {
                return this.expression;
            }
        }
    }
}
