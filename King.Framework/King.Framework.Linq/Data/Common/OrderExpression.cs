namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Linq.Expressions;

    public class OrderExpression
    {
        private System.Linq.Expressions.Expression expression;
        private King.Framework.Linq.Data.Common.OrderType orderType;

        public OrderExpression(King.Framework.Linq.Data.Common.OrderType orderType, System.Linq.Expressions.Expression expression)
        {
            this.orderType = orderType;
            this.expression = expression;
        }

        public System.Linq.Expressions.Expression Expression
        {
            get
            {
                return this.expression;
            }
        }

        public King.Framework.Linq.Data.Common.OrderType OrderType
        {
            get
            {
                return this.orderType;
            }
        }
    }
}
