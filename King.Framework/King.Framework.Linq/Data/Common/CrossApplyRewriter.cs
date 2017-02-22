namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public class CrossApplyRewriter : DbExpressionVisitor
    {
        private QueryLanguage language;

        private CrossApplyRewriter(QueryLanguage language)
        {
            this.language = language;
        }

        private bool CanBeColumn(Expression expr)
        {
            return ((expr != null) && (expr.NodeType == ((ExpressionType) 0x3ea)));
        }

        public static Expression Rewrite(QueryLanguage language, Expression expression)
        {
            return new CrossApplyRewriter(language).Visit(expression);
        }

        protected override Expression VisitJoin(JoinExpression join)
        {
            join = (JoinExpression) base.VisitJoin(join);
            if ((join.Join == JoinType.CrossApply) || (join.Join == JoinType.OuterApply))
            {
                if (join.Right is TableExpression)
                {
                    return new JoinExpression(JoinType.CrossJoin, join.Left, join.Right, null);
                }
                SelectExpression right = join.Right as SelectExpression;
                if ((((right != null) && (right.Take == null)) && ((right.Skip == null) && !AggregateChecker.HasAggregates(right))) && ((right.GroupBy == null) || (right.GroupBy.Count == 0)))
                {
                    SelectExpression source = right.SetWhere(null);
                    HashSet<TableAlias> set = ReferencedAliasGatherer.Gather(source);
                    HashSet<TableAlias> other = DeclaredAliasGatherer.Gather(join.Left);
                    set.IntersectWith(other);
                    if (set.Count == 0)
                    {
                        Expression where = right.Where;
                        right = source;
                        ProjectedColumns columns = ColumnProjector.ProjectColumns(this.language, where, right.Columns, right.Alias, DeclaredAliasGatherer.Gather(right.From));
                        right = right.SetColumns(columns.Columns);
                        where = columns.Projector;
                        return new JoinExpression((where == null) ? JoinType.CrossJoin : ((join.Join == JoinType.CrossApply) ? JoinType.InnerJoin : JoinType.LeftOuter), join.Left, right, where);
                    }
                }
            }
            return join;
        }
    }
}
