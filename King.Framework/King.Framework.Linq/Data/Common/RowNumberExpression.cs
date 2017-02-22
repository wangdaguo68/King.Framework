namespace King.Framework.Linq.Data.Common
{
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class RowNumberExpression : DbExpression
    {
        private ReadOnlyCollection<OrderExpression> orderBy;

        public RowNumberExpression(IEnumerable<OrderExpression> orderBy) : base(DbExpressionType.RowCount, typeof(int))
        {
            this.orderBy = orderBy.ToReadOnly<OrderExpression>();
        }

        public ReadOnlyCollection<OrderExpression> OrderBy
        {
            get
            {
                return this.orderBy;
            }
        }
    }
}
