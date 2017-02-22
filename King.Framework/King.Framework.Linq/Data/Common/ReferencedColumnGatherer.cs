namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public class ReferencedColumnGatherer : DbExpressionVisitor
    {
        private HashSet<ColumnExpression> columns = new HashSet<ColumnExpression>();
        private bool first = true;

        public static HashSet<ColumnExpression> Gather(Expression expression)
        {
            ReferencedColumnGatherer gatherer = new ReferencedColumnGatherer();
            gatherer.Visit(expression);
            return gatherer.columns;
        }

        protected override Expression VisitColumn(ColumnExpression column)
        {
            this.columns.Add(column);
            return column;
        }

        protected override Expression VisitSelect(SelectExpression select)
        {
            if (this.first)
            {
                this.first = false;
                return base.VisitSelect(select);
            }
            return select;
        }
    }
}
