namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Linq.Expressions;

    public class BetweenExpression : DbExpression
    {
        private System.Linq.Expressions.Expression expression;
        private System.Linq.Expressions.Expression lower;
        private System.Linq.Expressions.Expression upper;

        public BetweenExpression(System.Linq.Expressions.Expression expression, System.Linq.Expressions.Expression lower, System.Linq.Expressions.Expression upper) : base(DbExpressionType.Between, expression.Type)
        {
            this.expression = expression;
            this.lower = lower;
            this.upper = upper;
        }

        public System.Linq.Expressions.Expression Expression
        {
            get
            {
                return this.expression;
            }
        }

        public System.Linq.Expressions.Expression Lower
        {
            get
            {
                return this.lower;
            }
        }

        public System.Linq.Expressions.Expression Upper
        {
            get
            {
                return this.upper;
            }
        }
    }
}
