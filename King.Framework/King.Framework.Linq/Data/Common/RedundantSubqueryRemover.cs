namespace King.Framework.Linq.Data.Common
{
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;

    public class RedundantSubqueryRemover : DbExpressionVisitor
    {
        private RedundantSubqueryRemover()
        {
        }

        internal static bool IsInitialProjection(SelectExpression select)
        {
            return (select.From is TableExpression);
        }

        internal static bool IsNameMapProjection(SelectExpression select)
        {
            if (select.From is TableExpression)
            {
                return false;
            }
            SelectExpression from = select.From as SelectExpression;
            if ((from == null) || (select.Columns.Count != from.Columns.Count))
            {
                return false;
            }
            ReadOnlyCollection<ColumnDeclaration> columns = from.Columns;
            int num = 0;
            int count = select.Columns.Count;
            while (num < count)
            {
                ColumnExpression expression = select.Columns[num].Expression as ColumnExpression;
                if ((expression == null) || (expression.Name != columns[num].Name))
                {
                    return false;
                }
                num++;
            }
            return true;
        }

        internal static bool IsSimpleProjection(SelectExpression select)
        {
            foreach (ColumnDeclaration declaration in select.Columns)
            {
                ColumnExpression expression = declaration.Expression as ColumnExpression;
                if ((expression == null) || (declaration.Name != expression.Name))
                {
                    return false;
                }
            }
            return true;
        }

        public static Expression Remove(Expression expression)
        {
            expression = new RedundantSubqueryRemover().Visit(expression);
            expression = SubqueryMerger.Merge(expression);
            return expression;
        }

        protected override Expression VisitProjection(ProjectionExpression proj)
        {
            proj = (ProjectionExpression) base.VisitProjection(proj);
            if (proj.Select.From is SelectExpression)
            {
                List<SelectExpression> selectsToRemove = RedundantSubqueryGatherer.Gather(proj.Select);
                if (selectsToRemove != null)
                {
                    proj = SubqueryRemover.Remove(proj, selectsToRemove);
                }
            }
            return proj;
        }

        protected override Expression VisitSelect(SelectExpression select)
        {
            select = (SelectExpression) base.VisitSelect(select);
            List<SelectExpression> selectsToRemove = RedundantSubqueryGatherer.Gather(select.From);
            if (selectsToRemove != null)
            {
                select = SubqueryRemover.Remove(select, selectsToRemove);
            }
            return select;
        }

        private class RedundantSubqueryGatherer : DbExpressionVisitor
        {
            private List<SelectExpression> redundant;

            private RedundantSubqueryGatherer()
            {
            }

            internal static List<SelectExpression> Gather(Expression source)
            {
                RedundantSubqueryRemover.RedundantSubqueryGatherer gatherer = new RedundantSubqueryRemover.RedundantSubqueryGatherer();
                gatherer.Visit(source);
                return gatherer.redundant;
            }

            private static bool IsRedudantSubquery(SelectExpression select)
            {
                return (((((RedundantSubqueryRemover.IsSimpleProjection(select) || RedundantSubqueryRemover.IsNameMapProjection(select)) && (!select.IsDistinct && !select.IsReverse)) && (((select.Take == null) && (select.Skip == null)) && (select.Where == null))) && ((select.OrderBy == null) || (select.OrderBy.Count == 0))) && ((select.GroupBy == null) || (select.GroupBy.Count == 0)));
            }

            protected override Expression VisitSelect(SelectExpression select)
            {
                if (IsRedudantSubquery(select))
                {
                    if (this.redundant == null)
                    {
                        this.redundant = new List<SelectExpression>();
                    }
                    this.redundant.Add(select);
                }
                return select;
            }

            protected override Expression VisitSubquery(SubqueryExpression subquery)
            {
                return subquery;
            }
        }

        private class SubqueryMerger : DbExpressionVisitor
        {
            private bool isTopLevel = true;

            private SubqueryMerger()
            {
            }

            private static bool CanMergeWithFrom(SelectExpression select, bool isTopLevel)
            {
                SelectExpression leftMostSelect = GetLeftMostSelect(select.From);
                if (leftMostSelect == null)
                {
                    return false;
                }
                if (!IsColumnProjection(leftMostSelect))
                {
                    return false;
                }
                bool flag = RedundantSubqueryRemover.IsNameMapProjection(select);
                bool flag2 = (select.OrderBy != null) && (select.OrderBy.Count > 0);
                bool flag3 = (select.GroupBy != null) && (select.GroupBy.Count > 0);
                bool flag4 = AggregateChecker.HasAggregates(select);
                bool flag5 = select.From is JoinExpression;
                bool flag6 = (leftMostSelect.OrderBy != null) && (leftMostSelect.OrderBy.Count > 0);
                bool flag7 = (leftMostSelect.GroupBy != null) && (leftMostSelect.GroupBy.Count > 0);
                bool flag8 = AggregateChecker.HasAggregates(leftMostSelect);
                if (flag2 && flag6)
                {
                    return false;
                }
                if (flag3 && flag7)
                {
                    return false;
                }
                if (select.IsReverse || leftMostSelect.IsReverse)
                {
                    return false;
                }
                if (flag6 && ((flag3 || flag4) || select.IsDistinct))
                {
                    return false;
                }
                if (flag7)
                {
                    return false;
                }
                if ((leftMostSelect.Take != null) && (((((select.Take != null) || (select.Skip != null)) || (select.IsDistinct || flag4)) || flag3) || flag5))
                {
                    return false;
                }
                if ((leftMostSelect.Skip != null) && ((((select.Skip != null) || select.IsDistinct) || (flag4 || flag3)) || flag5))
                {
                    return false;
                }
                if (leftMostSelect.IsDistinct && (((((select.Take != null) || (select.Skip != null)) || (!flag || flag3)) || (flag4 || (flag2 && !isTopLevel))) || flag5))
                {
                    return false;
                }
                if (flag8 && (((((select.Take != null) || (select.Skip != null)) || (select.IsDistinct || flag4)) || flag3) || flag5))
                {
                    return false;
                }
                return true;
            }

            private static SelectExpression GetLeftMostSelect(Expression source)
            {
                SelectExpression expression = source as SelectExpression;
                if (expression != null)
                {
                    return expression;
                }
                JoinExpression expression2 = source as JoinExpression;
                if (expression2 != null)
                {
                    return GetLeftMostSelect(expression2.Left);
                }
                return null;
            }

            private static bool IsColumnProjection(SelectExpression select)
            {
                int num = 0;
                int count = select.Columns.Count;
                while (num < count)
                {
                    ColumnDeclaration declaration = select.Columns[num];
                    if ((declaration.Expression.NodeType != ((ExpressionType) 0x3ea)) && (declaration.Expression.NodeType != ExpressionType.Constant))
                    {
                        return false;
                    }
                    num++;
                }
                return true;
            }

            internal static Expression Merge(Expression expression)
            {
                return new RedundantSubqueryRemover.SubqueryMerger().Visit(expression);
            }

            protected override Expression VisitSelect(SelectExpression select)
            {
                bool isTopLevel = this.isTopLevel;
                this.isTopLevel = false;
                select = (SelectExpression) base.VisitSelect(select);
                while (CanMergeWithFrom(select, isTopLevel))
                {
                    SelectExpression leftMostSelect = GetLeftMostSelect(select.From);
                    select = SubqueryRemover.Remove(select, new SelectExpression[] { leftMostSelect });
                    Expression where = select.Where;
                    if (leftMostSelect.Where != null)
                    {
                        if (where != null)
                        {
                            where = leftMostSelect.Where.And(where);
                        }
                        else
                        {
                            where = leftMostSelect.Where;
                        }
                    }
                    ReadOnlyCollection<OrderExpression> orderBy = ((select.OrderBy != null) && (select.OrderBy.Count > 0)) ? select.OrderBy : leftMostSelect.OrderBy;
                    ReadOnlyCollection<Expression> groupBy = ((select.GroupBy != null) && (select.GroupBy.Count > 0)) ? select.GroupBy : leftMostSelect.GroupBy;
                    Expression skip = (select.Skip != null) ? select.Skip : leftMostSelect.Skip;
                    Expression take = (select.Take != null) ? select.Take : leftMostSelect.Take;
                    bool isDistinct = select.IsDistinct | leftMostSelect.IsDistinct;
                    if (((((where != select.Where) || (orderBy != select.OrderBy)) || ((groupBy != select.GroupBy) || (isDistinct != select.IsDistinct))) || (skip != select.Skip)) || (take != select.Take))
                    {
                        select = new SelectExpression(select.Alias, select.Columns, select.From, where, orderBy, groupBy, isDistinct, skip, take, select.IsReverse);
                    }
                }
                return select;
            }
        }
    }
}
