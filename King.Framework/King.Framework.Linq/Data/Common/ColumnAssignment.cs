namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Linq.Expressions;

    public class ColumnAssignment
    {
        private ColumnExpression column;
        private System.Linq.Expressions.Expression expression;

        public ColumnAssignment(ColumnExpression column, System.Linq.Expressions.Expression expression)
        {
            this.column = column;
            this.expression = expression;
        }

        public ColumnExpression Column
        {
            get
            {
                return this.column;
            }
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
