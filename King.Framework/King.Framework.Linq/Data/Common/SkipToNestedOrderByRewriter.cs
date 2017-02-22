using System.Linq;

namespace King.Framework.Linq.Data.Common
{
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public class SkipToNestedOrderByRewriter : DbExpressionVisitor
    {
        private QueryLanguage language;

        private SkipToNestedOrderByRewriter(QueryLanguage language)
        {
            this.language = language;
        }

        public static Expression Rewrite(QueryLanguage language, Expression expression)
        {
            return new SkipToNestedOrderByRewriter(language).Visit(expression);
        }

        protected override Expression VisitSelect(SelectExpression selectExpression)
        {
            selectExpression = (SelectExpression) base.VisitSelect(selectExpression);
            if (((selectExpression.Skip != null) && (selectExpression.Take != null)) && (selectExpression.OrderBy.Count > 0))
            {
                Expression skip = selectExpression.Skip;
                Expression take = selectExpression.Take;
                Expression expression3 = PartialEvaluator.Eval(Expression.Add(skip, take));
                selectExpression = selectExpression.SetTake(expression3).SetSkip(null);
                selectExpression = selectExpression.AddRedundantSelect(this.language, new TableAlias());
                selectExpression = selectExpression.SetTake(take);
                selectExpression = (SelectExpression) OrderByRewriter.Rewrite(this.language, selectExpression);
                IEnumerable<OrderExpression> orderBy = from ob in selectExpression.OrderBy select new OrderExpression((ob.OrderType == OrderType.Ascending) ? OrderType.Descending : OrderType.Ascending, ob.Expression);
                selectExpression = selectExpression.SetOrderBy(orderBy);
                selectExpression = selectExpression.AddRedundantSelect(this.language, new TableAlias());
                selectExpression = selectExpression.SetTake(Expression.Constant(0));
                selectExpression = (SelectExpression) OrderByRewriter.Rewrite(this.language, selectExpression);
                IEnumerable<OrderExpression> enumerable2 = from ob in selectExpression.OrderBy select new OrderExpression((ob.OrderType == OrderType.Ascending) ? OrderType.Descending : OrderType.Ascending, ob.Expression);
                selectExpression = selectExpression.SetOrderBy(enumerable2);
                selectExpression = selectExpression.SetTake(null);
            }
            return selectExpression;
        }
    }
}
