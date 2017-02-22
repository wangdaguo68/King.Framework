namespace King.Framework.Linq.Data.Common
{
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public class RedundantJoinRemover : DbExpressionVisitor
    {
        private Dictionary<TableAlias, TableAlias> map = new Dictionary<TableAlias, TableAlias>();

        private RedundantJoinRemover()
        {
        }

        private Expression FindSimilarRight(JoinExpression join, JoinExpression compareTo)
        {
            if (join == null)
            {
                return null;
            }
            if ((join.Join == compareTo.Join) && ((join.Right.NodeType == compareTo.Right.NodeType) && DbExpressionComparer.AreEqual(join.Right, compareTo.Right)))
            {
                if (join.Condition == compareTo.Condition)
                {
                    return join.Right;
                }
                ScopedDictionary<TableAlias, TableAlias> aliasScope = new ScopedDictionary<TableAlias, TableAlias>(null);
                aliasScope.Add(((AliasedExpression) join.Right).Alias, ((AliasedExpression) compareTo.Right).Alias);
                if (DbExpressionComparer.AreEqual(null, aliasScope, join.Condition, compareTo.Condition))
                {
                    return join.Right;
                }
            }
            Expression expression = this.FindSimilarRight(join.Left as JoinExpression, compareTo);
            if (expression == null)
            {
                expression = this.FindSimilarRight(join.Right as JoinExpression, compareTo);
            }
            return expression;
        }

        public static Expression Remove(Expression expression)
        {
            return new RedundantJoinRemover().Visit(expression);
        }

        protected override Expression VisitColumn(ColumnExpression column)
        {
            TableAlias alias;
            if (this.map.TryGetValue(column.Alias, out alias))
            {
                return new ColumnExpression(column.Type, column.QueryType, alias, column.Name);
            }
            return column;
        }

        protected override Expression VisitJoin(JoinExpression join)
        {
            Expression expression = base.VisitJoin(join);
            join = expression as JoinExpression;
            if (join != null)
            {
                AliasedExpression right = join.Right as AliasedExpression;
                if (right != null)
                {
                    AliasedExpression expression3 = (AliasedExpression) this.FindSimilarRight(join.Left as JoinExpression, join);
                    if (expression3 != null)
                    {
                        this.map.Add(right.Alias, expression3.Alias);
                        return join.Left;
                    }
                }
            }
            return expression;
        }
    }
}
