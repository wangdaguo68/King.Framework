namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public class CrossJoinIsolator : DbExpressionVisitor
    {
        private ILookup<TableAlias, ColumnExpression> columns;
        private JoinType? lastJoin;
        private Dictionary<ColumnExpression, ColumnExpression> map = new Dictionary<ColumnExpression, ColumnExpression>();

        private bool IsCrossJoin(Expression expression)
        {
            JoinExpression expression2 = expression as JoinExpression;
            return ((expression2 != null) && (expression2.Join == JoinType.CrossJoin));
        }

        public static Expression Isolate(Expression expression)
        {
            return new CrossJoinIsolator().Visit(expression);
        }

        private Expression MakeSubquery(Expression expression)
        {
            TableAlias alias = new TableAlias();
            HashSet<TableAlias> set = DeclaredAliasGatherer.Gather(expression);
            List<ColumnDeclaration> columns = new List<ColumnDeclaration>();
            foreach (TableAlias alias2 in set)
            {
                foreach (ColumnExpression expression2 in this.columns[alias2])
                {
                    ColumnDeclaration item = new ColumnDeclaration(columns.GetAvailableColumnName(expression2.Name), expression2, expression2.QueryType);
                    columns.Add(item);
                    ColumnExpression expression3 = new ColumnExpression(expression2.Type, expression2.QueryType, alias, expression2.Name);
                    this.map.Add(expression2, expression3);
                }
            }
            return new SelectExpression(alias, columns, expression, null);
        }

        protected override Expression VisitColumn(ColumnExpression column)
        {
            ColumnExpression expression;
            if (this.map.TryGetValue(column, out expression))
            {
                return expression;
            }
            return column;
        }

        protected override Expression VisitJoin(JoinExpression join)
        {
            JoinType? nullable2;
            JoinType? lastJoin = this.lastJoin;
            this.lastJoin = new JoinType?(join.Join);
            join = (JoinExpression) base.VisitJoin(join);
            this.lastJoin = lastJoin;
            if (this.lastJoin.HasValue && ((join.Join == JoinType.CrossJoin) != ((((JoinType) (nullable2 = this.lastJoin).GetValueOrDefault()) == JoinType.CrossJoin) && nullable2.HasValue)))
            {
                return this.MakeSubquery(join);
            }
            return join;
        }

        protected override Expression VisitSelect(SelectExpression select)
        {
            ILookup<TableAlias, ColumnExpression> columns = this.columns;
            this.columns = ReferencedColumnGatherer.Gather(select).ToLookup<ColumnExpression, TableAlias>(c => c.Alias);
            JoinType? lastJoin = this.lastJoin;
            this.lastJoin = null;
            Expression expression = base.VisitSelect(select);
            this.columns = columns;
            this.lastJoin = lastJoin;
            return expression;
        }
    }
}
