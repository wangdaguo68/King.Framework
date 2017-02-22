namespace King.Framework.Linq.Data.Common
{
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public class CrossJoinRewriter : DbExpressionVisitor
    {
        private Expression currentWhere;

        private bool CanBeJoinCondition(Expression expression, HashSet<TableAlias> left, HashSet<TableAlias> right, HashSet<TableAlias> all)
        {
            HashSet<TableAlias> first = ReferencedAliasGatherer.Gather(expression);
            bool flag = first.Intersect<TableAlias>(left).Any<TableAlias>();
            bool flag2 = first.Intersect<TableAlias>(right).Any<TableAlias>();
            bool flag3 = first.IsSubsetOf(all);
            return ((flag && flag2) && flag3);
        }

        public static Expression Rewrite(Expression expression)
        {
            return new CrossJoinRewriter().Visit(expression);
        }

        protected override Expression VisitJoin(JoinExpression join)
        {
            join = (JoinExpression) base.VisitJoin(join);
            if ((join.Join == JoinType.CrossJoin) && (this.currentWhere != null))
            {
                Func<Expression, bool> predicate = null;
                HashSet<TableAlias> declaredLeft = DeclaredAliasGatherer.Gather(join.Left);
                HashSet<TableAlias> declaredRight = DeclaredAliasGatherer.Gather(join.Right);
                HashSet<TableAlias> declared = new HashSet<TableAlias>(declaredLeft.Union<TableAlias>(declaredRight));
                Expression[] source = this.currentWhere.Split(new ExpressionType[] { ExpressionType.And, ExpressionType.AndAlso });
                List<Expression> good = (from e in source
                    where this.CanBeJoinCondition(e, declaredLeft, declaredRight, declared)
                    select e).ToList<Expression>();
                if (good.Count <= 0)
                {
                    return join;
                }
                Expression condition = good.Join(ExpressionType.And);
                join = base.UpdateJoin(join, JoinType.InnerJoin, join.Left, join.Right, condition);
                if (predicate == null)
                {
                    predicate = e => !good.Contains(e);
                }
                Expression expression2 = source.Where<Expression>(predicate).Join(ExpressionType.And);
                this.currentWhere = expression2;
            }
            return join;
        }

        protected override Expression VisitSelect(SelectExpression select)
        {
            Expression expression3;
            Expression currentWhere = this.currentWhere;
            try
            {
                this.currentWhere = select.Where;
                SelectExpression expression2 = (SelectExpression) base.VisitSelect(select);
                if (this.currentWhere != expression2.Where)
                {
                    return expression2.SetWhere(this.currentWhere);
                }
                expression3 = expression2;
            }
            finally
            {
                this.currentWhere = currentWhere;
            }
            return expression3;
        }
    }
}
