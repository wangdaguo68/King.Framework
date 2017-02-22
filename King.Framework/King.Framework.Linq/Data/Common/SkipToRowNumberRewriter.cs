namespace King.Framework.Linq.Data.Common
{
    using King.Framework.Linq;
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    public class SkipToRowNumberRewriter : DbExpressionVisitor
    {
        private QueryLanguage language;

        private SkipToRowNumberRewriter(QueryLanguage language)
        {
            this.language = language;
        }

        public static Expression Rewrite(QueryLanguage language, Expression expression)
        {
            return new SkipToRowNumberRewriter(language).Visit(expression);
        }

        protected override Expression VisitSelect(SelectExpression select)
        {
            select = (SelectExpression) base.VisitSelect(select);
            if (select.Skip != null)
            {
                Expression expression3;
                SelectExpression sel = select.SetSkip(null).SetTake(null);
                if (select.IsDistinct || ((select.GroupBy != null) && (select.GroupBy.Count != 0)))
                {
                    sel = sel.AddRedundantSelect(this.language, new TableAlias());
                }
                QueryType columnType = this.language.TypeSystem.GetColumnType(typeof(int));
                sel = sel.AddColumn(new ColumnDeclaration("row__num", new RowNumberExpression(select.OrderBy), columnType)).AddRedundantSelect(this.language, new TableAlias());
                sel = sel.RemoveColumn(sel.Columns.Single<ColumnDeclaration>(c => c.Name == "row__num"));
                TableAlias alias = ((SelectExpression) sel.From).Alias;
                ColumnExpression expression = new ColumnExpression(typeof(int), columnType, alias, "row__num");
                if (select.Take != null)
                {
                    expression3 = new BetweenExpression(expression, Expression.Add(select.Skip, Expression.Constant(1)), Expression.Add(select.Skip, select.Take));
                }
                else
                {
                    expression3 = expression.GreaterThan(select.Skip);
                }
                if (sel.Where != null)
                {
                    expression3 = sel.Where.And(expression3);
                }
                select = sel.SetWhere(expression3);
            }
            return select;
        }
    }
}
