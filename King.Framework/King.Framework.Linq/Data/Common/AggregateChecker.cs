namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Linq.Expressions;

    internal class AggregateChecker : DbExpressionVisitor
    {
        private bool hasAggregate = false;

        private AggregateChecker()
        {
        }

        internal static bool HasAggregates(SelectExpression expression)
        {
            AggregateChecker checker = new AggregateChecker();
            checker.Visit(expression);
            return checker.hasAggregate;
        }

        protected override Expression VisitAggregate(AggregateExpression aggregate)
        {
            this.hasAggregate = true;
            return aggregate;
        }

        protected override Expression VisitSelect(SelectExpression select)
        {
            this.Visit(select.Where);
            this.VisitOrderBy(select.OrderBy);
            this.VisitColumnDeclarations(select.Columns);
            return select;
        }

        protected override Expression VisitSubquery(SubqueryExpression subquery)
        {
            return subquery;
        }
    }
}
