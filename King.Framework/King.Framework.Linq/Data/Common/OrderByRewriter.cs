namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;

    public class OrderByRewriter : DbExpressionVisitor
    {
        private IList<OrderExpression> gatheredOrderings;
        private bool isOuterMostSelect;
        private QueryLanguage language;

        private OrderByRewriter(QueryLanguage language)
        {
            this.language = language;
            this.isOuterMostSelect = true;
        }

        protected void PrependOrderings(IList<OrderExpression> newOrderings)
        {
            if (newOrderings != null)
            {
                int num;
                if (this.gatheredOrderings == null)
                {
                    this.gatheredOrderings = new List<OrderExpression>();
                }
                for (num = newOrderings.Count - 1; num >= 0; num--)
                {
                    this.gatheredOrderings.Insert(0, newOrderings[num]);
                }
                HashSet<string> set = new HashSet<string>();
                num = 0;
                while (num < this.gatheredOrderings.Count)
                {
                    ColumnExpression expression = this.gatheredOrderings[num].Expression as ColumnExpression;
                    if (expression != null)
                    {
                        string item = expression.Alias + ":" + expression.Name;
                        if (set.Contains(item))
                        {
                            this.gatheredOrderings.RemoveAt(num);
                            continue;
                        }
                        set.Add(item);
                    }
                    num++;
                }
            }
        }

        protected virtual BindResult RebindOrderings(IEnumerable<OrderExpression> orderings, TableAlias alias, HashSet<TableAlias> existingAliases, IEnumerable<ColumnDeclaration> existingColumns)
        {
            List<ColumnDeclaration> columns = null;
            List<OrderExpression> list2 = new List<OrderExpression>();
            foreach (OrderExpression expression in orderings)
            {
                Expression expression2 = expression.Expression;
                ColumnExpression expression3 = expression2 as ColumnExpression;
                if ((expression3 == null) || ((existingAliases != null) && existingAliases.Contains(expression3.Alias)))
                {
                    int num = 0;
                    foreach (ColumnDeclaration declaration in existingColumns)
                    {
                        ColumnExpression expression4 = declaration.Expression as ColumnExpression;
                        if ((declaration.Expression == expression.Expression) || ((((expression3 != null) && (expression4 != null)) && (expression3.Alias == expression4.Alias)) && (expression3.Name == expression4.Name)))
                        {
                            expression2 = new ColumnExpression(expression3.Type, expression3.QueryType, alias, declaration.Name);
                            break;
                        }
                        num++;
                    }
                    if (expression2 == expression.Expression)
                    {
                        if (columns == null)
                        {
                            columns = new List<ColumnDeclaration>(existingColumns);
                            existingColumns = columns;
                        }
                        string baseName = (expression3 != null) ? expression3.Name : ("c" + num);
                        baseName = columns.GetAvailableColumnName(baseName);
                        QueryType columnType = this.language.TypeSystem.GetColumnType(expression2.Type);
                        columns.Add(new ColumnDeclaration(baseName, expression.Expression, columnType));
                        expression2 = new ColumnExpression(expression2.Type, columnType, alias, baseName);
                    }
                    list2.Add(new OrderExpression(expression.OrderType, expression2));
                }
            }
            return new BindResult(existingColumns, list2);
        }

        protected void ReverseOrderings()
        {
            if (this.gatheredOrderings != null)
            {
                int num = 0;
                int count = this.gatheredOrderings.Count;
                while (num < count)
                {
                    OrderExpression expression = this.gatheredOrderings[num];
                    this.gatheredOrderings[num] = new OrderExpression((expression.OrderType == OrderType.Ascending) ? OrderType.Descending : OrderType.Ascending, expression.Expression);
                    num++;
                }
            }
        }

        public static Expression Rewrite(QueryLanguage language, Expression expression)
        {
            return new OrderByRewriter(language).Visit(expression);
        }

        protected override Expression VisitJoin(JoinExpression join)
        {
            Expression left = this.VisitSource(join.Left);
            IList<OrderExpression> gatheredOrderings = this.gatheredOrderings;
            this.gatheredOrderings = null;
            Expression right = this.VisitSource(join.Right);
            this.PrependOrderings(gatheredOrderings);
            Expression condition = this.Visit(join.Condition);
            if (((left != join.Left) || (right != join.Right)) || (condition != join.Condition))
            {
                return new JoinExpression(join.Join, left, right, condition);
            }
            return join;
        }

        protected override Expression VisitSelect(SelectExpression select)
        {
            Expression expression;
            bool isOuterMostSelect = this.isOuterMostSelect;
            try
            {
                this.isOuterMostSelect = false;
                select = (SelectExpression) base.VisitSelect(select);
                bool flag2 = (select.OrderBy != null) && (select.OrderBy.Count > 0);
                bool flag3 = (select.GroupBy != null) && (select.GroupBy.Count > 0);
                bool flag4 = (isOuterMostSelect || (select.Take != null)) || (select.Skip != null);
                bool flag5 = ((flag4 && !flag3) && !select.IsDistinct) && !AggregateChecker.HasAggregates(select);
                if (flag2)
                {
                    this.PrependOrderings(select.OrderBy);
                }
                if (select.IsReverse)
                {
                    this.ReverseOrderings();
                }
                IEnumerable<OrderExpression> orderBy = null;
                if (flag5)
                {
                    orderBy = this.gatheredOrderings;
                }
                else if (flag4)
                {
                    orderBy = select.OrderBy;
                }
                bool flag6 = (!isOuterMostSelect && !flag3) && !select.IsDistinct;
                ReadOnlyCollection<ColumnDeclaration> columns = select.Columns;
                if (this.gatheredOrderings != null)
                {
                    if (flag6)
                    {
                        HashSet<TableAlias> existingAliases = DeclaredAliasGatherer.Gather(select.From);
                        BindResult result = this.RebindOrderings(this.gatheredOrderings, select.Alias, existingAliases, select.Columns);
                        this.gatheredOrderings = null;
                        this.PrependOrderings(result.Orderings);
                        columns = result.Columns;
                    }
                    else
                    {
                        this.gatheredOrderings = null;
                    }
                }
                if (((orderBy != select.OrderBy) || (columns != select.Columns)) || select.IsReverse)
                {
                    select = new SelectExpression(select.Alias, columns, select.From, select.Where, orderBy, select.GroupBy, select.IsDistinct, select.Skip, select.Take, false);
                }
                expression = select;
            }
            finally
            {
                this.isOuterMostSelect = isOuterMostSelect;
            }
            return expression;
        }

        protected override Expression VisitSubquery(SubqueryExpression subquery)
        {
            IList<OrderExpression> gatheredOrderings = this.gatheredOrderings;
            this.gatheredOrderings = null;
            Expression expression = base.VisitSubquery(subquery);
            this.gatheredOrderings = gatheredOrderings;
            return expression;
        }

        protected class BindResult
        {
            private ReadOnlyCollection<ColumnDeclaration> columns;
            private ReadOnlyCollection<OrderExpression> orderings;

            public BindResult(IEnumerable<ColumnDeclaration> columns, IEnumerable<OrderExpression> orderings)
            {
                this.columns = columns as ReadOnlyCollection<ColumnDeclaration>;
                if (this.columns == null)
                {
                    this.columns = new List<ColumnDeclaration>(columns).AsReadOnly();
                }
                this.orderings = orderings as ReadOnlyCollection<OrderExpression>;
                if (this.orderings == null)
                {
                    this.orderings = new List<OrderExpression>(orderings).AsReadOnly();
                }
            }

            public ReadOnlyCollection<ColumnDeclaration> Columns
            {
                get
                {
                    return this.columns;
                }
            }

            public ReadOnlyCollection<OrderExpression> Orderings
            {
                get
                {
                    return this.orderings;
                }
            }
        }
    }
}
