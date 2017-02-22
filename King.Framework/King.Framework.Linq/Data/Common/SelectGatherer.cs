namespace King.Framework.Linq.Data.Common
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;

    public class SelectGatherer : DbExpressionVisitor
    {
        private List<SelectExpression> selects = new List<SelectExpression>();

        public static ReadOnlyCollection<SelectExpression> Gather(Expression expression)
        {
            SelectGatherer gatherer = new SelectGatherer();
            gatherer.Visit(expression);
            return gatherer.selects.AsReadOnly();
        }

        protected override Expression VisitSelect(SelectExpression select)
        {
            this.selects.Add(select);
            return select;
        }
    }
}
