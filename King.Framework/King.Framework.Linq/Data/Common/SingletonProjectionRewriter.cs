namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Linq.Expressions;

    public class SingletonProjectionRewriter : DbExpressionVisitor
    {
        private SelectExpression currentSelect;
        private bool isTopLevel = true;
        private QueryLanguage language;

        private SingletonProjectionRewriter(QueryLanguage language)
        {
            this.language = language;
        }

        private bool CanJoinOnServer(SelectExpression select)
        {
            return ((!select.IsDistinct && ((select.GroupBy == null) || (select.GroupBy.Count == 0))) && !AggregateChecker.HasAggregates(select));
        }

        public static Expression Rewrite(QueryLanguage language, Expression expression)
        {
            return new SingletonProjectionRewriter(language).Visit(expression);
        }

        protected override Expression VisitClientJoin(ClientJoinExpression join)
        {
            bool isTopLevel = this.isTopLevel;
            SelectExpression currentSelect = this.currentSelect;
            this.isTopLevel = true;
            this.currentSelect = null;
            Expression expression2 = base.VisitClientJoin(join);
            this.isTopLevel = isTopLevel;
            this.currentSelect = currentSelect;
            return expression2;
        }

        protected override Expression VisitCommand(CommandExpression command)
        {
            this.isTopLevel = true;
            return base.VisitCommand(command);
        }

        protected override Expression VisitProjection(ProjectionExpression proj)
        {
            if (this.isTopLevel)
            {
                this.isTopLevel = false;
                this.currentSelect = proj.Select;
                Expression projector = this.Visit(proj.Projector);
                if ((projector != proj.Projector) || (this.currentSelect != proj.Select))
                {
                    return new ProjectionExpression(this.currentSelect, projector, proj.Aggregator);
                }
                return proj;
            }
            if (proj.IsSingleton && this.CanJoinOnServer(this.currentSelect))
            {
                TableAlias newAlias = new TableAlias();
                this.currentSelect = this.currentSelect.AddRedundantSelect(this.language, newAlias);
                SelectExpression source = (SelectExpression) ColumnMapper.Map(proj.Select, newAlias, new TableAlias[] { this.currentSelect.Alias });
                ProjectionExpression expression3 = this.language.AddOuterJoinTest(new ProjectionExpression(source, proj.Projector));
                ProjectedColumns columns = ColumnProjector.ProjectColumns(this.language, expression3.Projector, this.currentSelect.Columns, this.currentSelect.Alias, new TableAlias[] { newAlias, proj.Select.Alias });
                JoinExpression from = new JoinExpression(JoinType.OuterApply, this.currentSelect.From, expression3.Select, null);
                this.currentSelect = new SelectExpression(this.currentSelect.Alias, columns.Columns, from, null);
                return this.Visit(columns.Projector);
            }
            bool isTopLevel = this.isTopLevel;
            SelectExpression currentSelect = this.currentSelect;
            this.isTopLevel = true;
            this.currentSelect = null;
            Expression expression6 = base.VisitProjection(proj);
            this.isTopLevel = isTopLevel;
            this.currentSelect = currentSelect;
            return expression6;
        }

        protected override Expression VisitSubquery(SubqueryExpression subquery)
        {
            return subquery;
        }
    }
}
