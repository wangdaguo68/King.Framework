namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Linq.Expressions;

    public class OuterJoinedExpression : DbExpression
    {
        private System.Linq.Expressions.Expression expression;
        private System.Linq.Expressions.Expression test;

        public OuterJoinedExpression(System.Linq.Expressions.Expression test, System.Linq.Expressions.Expression expression) : base(DbExpressionType.OuterJoined, expression.Type)
        {
            this.test = test;
            this.expression = expression;
        }

        public System.Linq.Expressions.Expression Expression
        {
            get
            {
                return this.expression;
            }
        }

        public System.Linq.Expressions.Expression Test
        {
            get
            {
                return this.test;
            }
        }
    }
}
