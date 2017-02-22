namespace King.Framework.Linq.Data.Common
{
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;

    public class InExpression : SubqueryExpression
    {
        private System.Linq.Expressions.Expression expression;
        private ReadOnlyCollection<System.Linq.Expressions.Expression> values;

        public InExpression(System.Linq.Expressions.Expression expression, SelectExpression select) : base(DbExpressionType.In, typeof(bool), select)
        {
            this.expression = expression;
        }

        public InExpression(System.Linq.Expressions.Expression expression, IEnumerable<System.Linq.Expressions.Expression> values) : base(DbExpressionType.In, typeof(bool), null)
        {
            this.expression = expression;
            this.values = values.ToReadOnly<System.Linq.Expressions.Expression>();
        }

        public System.Linq.Expressions.Expression Expression
        {
            get
            {
                return this.expression;
            }
        }

        public ReadOnlyCollection<System.Linq.Expressions.Expression> Values
        {
            get
            {
                return this.values;
            }
        }
    }
}
