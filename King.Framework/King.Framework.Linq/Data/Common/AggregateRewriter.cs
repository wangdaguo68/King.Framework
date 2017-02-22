namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public class AggregateRewriter : DbExpressionVisitor
    {
        private QueryLanguage language;
        private ILookup<TableAlias, AggregateSubqueryExpression> lookup;
        private Dictionary<AggregateSubqueryExpression, Expression> map;

        private AggregateRewriter(QueryLanguage language, Expression expr)
        {
            this.language = language;
            this.map = new Dictionary<AggregateSubqueryExpression, Expression>();
            this.lookup = AggregateGatherer.Gather(expr).ToLookup<AggregateSubqueryExpression, TableAlias>(a => a.GroupByAlias);
        }

        public static Expression Rewrite(QueryLanguage language, Expression expr)
        {
            return new AggregateRewriter(language, expr).Visit(expr);
        }

        protected override Expression VisitAggregateSubquery(AggregateSubqueryExpression aggregate)
        {
            Expression expression;
            if (this.map.TryGetValue(aggregate, out expression))
            {
                return expression;
            }
            return this.Visit(aggregate.AggregateAsSubquery);
        }

        protected override Expression VisitSelect(SelectExpression select)
        {
            select = (SelectExpression)base.VisitSelect(select);
            if (this.lookup.Contains(select.Alias))
            {
                List<ColumnDeclaration> columns = new List<ColumnDeclaration>(select.Columns);
                foreach (AggregateSubqueryExpression expression in this.lookup[select.Alias])
                {
                    string name = "agg" + columns.Count;
                    QueryType columnType = this.language.TypeSystem.GetColumnType(expression.Type);
                    ColumnDeclaration item = new ColumnDeclaration(name, expression.AggregateInGroupSelect, columnType);
                    this.map.Add(expression, new ColumnExpression(expression.Type, columnType, expression.GroupByAlias, name));
                    columns.Add(item);
                }
                return new SelectExpression(select.Alias, columns, select.From, select.Where, select.OrderBy, select.GroupBy, select.IsDistinct, select.Skip, select.Take, select.IsReverse);
            }
            return select;
        }

        private class AggregateGatherer : DbExpressionVisitor
        {
            private List<AggregateSubqueryExpression> aggregates = new List<AggregateSubqueryExpression>();

            private AggregateGatherer()
            {
            }

            internal static List<AggregateSubqueryExpression> Gather(Expression expression)
            {
                AggregateRewriter.AggregateGatherer gatherer = new AggregateRewriter.AggregateGatherer();
                gatherer.Visit(expression);
                return gatherer.aggregates;
            }

            protected override Expression VisitAggregateSubquery(AggregateSubqueryExpression aggregate)
            {
                this.aggregates.Add(aggregate);
                return base.VisitAggregateSubquery(aggregate);
            }
        }
    }
}
