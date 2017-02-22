namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq.Expressions;

    public class UnusedColumnRemover : DbExpressionVisitor
    {
        private Dictionary<TableAlias, HashSet<string>> allColumnsUsed = new Dictionary<TableAlias, HashSet<string>>();
        private bool retainAllColumns;

        private UnusedColumnRemover()
        {
        }

        private void ClearColumnsUsed(TableAlias alias)
        {
            this.allColumnsUsed[alias] = new HashSet<string>();
        }

        private bool IsColumnUsed(TableAlias alias, string name)
        {
            HashSet<string> set;
            return ((this.allColumnsUsed.TryGetValue(alias, out set) && (set != null)) && set.Contains(name));
        }

        private void MarkColumnAsUsed(TableAlias alias, string name)
        {
            HashSet<string> set;
            if (!this.allColumnsUsed.TryGetValue(alias, out set))
            {
                set = new HashSet<string>();
                this.allColumnsUsed.Add(alias, set);
            }
            set.Add(name);
        }

        public static Expression Remove(Expression expression)
        {
            return new UnusedColumnRemover().Visit(expression);
        }

        protected override Expression VisitAggregate(AggregateExpression aggregate)
        {
            if ((aggregate.AggregateName == "Count") && (aggregate.Argument == null))
            {
                this.retainAllColumns = true;
            }
            return base.VisitAggregate(aggregate);
        }

        protected override Expression VisitClientJoin(ClientJoinExpression join)
        {
            ReadOnlyCollection<Expression> innerKey = this.VisitExpressionList(join.InnerKey);
            ReadOnlyCollection<Expression> outerKey = this.VisitExpressionList(join.OuterKey);
            ProjectionExpression projection = (ProjectionExpression) this.Visit(join.Projection);
            if (((projection != join.Projection) || (innerKey != join.InnerKey)) || (outerKey != join.OuterKey))
            {
                return new ClientJoinExpression(projection, outerKey, innerKey);
            }
            return join;
        }

        protected override Expression VisitColumn(ColumnExpression column)
        {
            this.MarkColumnAsUsed(column.Alias, column.Name);
            return column;
        }

        protected override Expression VisitJoin(JoinExpression join)
        {
            Expression expression;
            Expression expression4;
            if (join.Join == JoinType.SingletonLeftOuter)
            {
                AliasedExpression expression2 = this.Visit(join.Right) as AliasedExpression;
                if (!((expression2 == null) || this.allColumnsUsed.ContainsKey(expression2.Alias)))
                {
                    return this.Visit(join.Left);
                }
                Expression expression3 = this.Visit(join.Condition);
                expression4 = this.Visit(join.Left);
                expression = this.Visit(join.Right);
                return base.UpdateJoin(join, join.Join, expression4, expression, expression3);
            }
            Expression condition = this.Visit(join.Condition);
            expression = this.VisitSource(join.Right);
            expression4 = this.VisitSource(join.Left);
            return base.UpdateJoin(join, join.Join, expression4, expression, condition);
        }

        protected override Expression VisitProjection(ProjectionExpression projection)
        {
            Expression projector = this.Visit(projection.Projector);
            SelectExpression select = (SelectExpression) this.Visit(projection.Select);
            return base.UpdateProjection(projection, select, projector, projection.Aggregator);
        }

        protected override Expression VisitSelect(SelectExpression select)
        {
            ReadOnlyCollection<ColumnDeclaration> columns = select.Columns;
            bool retainAllColumns = this.retainAllColumns;
            this.retainAllColumns = false;
            List<ColumnDeclaration> list = null;
            int num = 0;
            int count = select.Columns.Count;
            while (num < count)
            {
                ColumnDeclaration item = select.Columns[num];
                if ((retainAllColumns || select.IsDistinct) || this.IsColumnUsed(select.Alias, item.Name))
                {
                    Expression expression = this.Visit(item.Expression);
                    if (expression != item.Expression)
                    {
                        item = new ColumnDeclaration(item.Name, expression, item.QueryType);
                    }
                }
                else
                {
                    item = null;
                }
                if ((item != select.Columns[num]) && (list == null))
                {
                    list = new List<ColumnDeclaration>();
                    for (int i = 0; i < num; i++)
                    {
                        list.Add(select.Columns[i]);
                    }
                }
                if ((item != null) && (list != null))
                {
                    list.Add(item);
                }
                num++;
            }
            if (list != null)
            {
                columns = list.AsReadOnly();
            }
            Expression take = this.Visit(select.Take);
            Expression skip = this.Visit(select.Skip);
            ReadOnlyCollection<Expression> groupBy = this.VisitExpressionList(select.GroupBy);
            ReadOnlyCollection<OrderExpression> orderBy = this.VisitOrderBy(select.OrderBy);
            Expression where = this.Visit(select.Where);
            Expression from = this.Visit(select.From);
            this.ClearColumnsUsed(select.Alias);
            if (((((columns != select.Columns) || (take != select.Take)) || ((skip != select.Skip) || (orderBy != select.OrderBy))) || ((groupBy != select.GroupBy) || (where != select.Where))) || (from != select.From))
            {
                select = new SelectExpression(select.Alias, columns, from, where, orderBy, groupBy, select.IsDistinct, skip, take, select.IsReverse);
            }
            this.retainAllColumns = retainAllColumns;
            return select;
        }

        protected override Expression VisitSubquery(SubqueryExpression subquery)
        {
            if (((subquery.NodeType == ((ExpressionType) 0x3f0)) || (subquery.NodeType == ((ExpressionType) 0x3f2))) && (subquery.Select != null))
            {
                Debug.Assert(subquery.Select.Columns.Count == 1);
                this.MarkColumnAsUsed(subquery.Select.Alias, subquery.Select.Columns[0].Name);
            }
            return base.VisitSubquery(subquery);
        }
    }
}
