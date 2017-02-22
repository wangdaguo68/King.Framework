using System.Linq;

namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;

    public static class DbExpressionExtensions
    {
        public static SelectExpression AddColumn(this SelectExpression select, ColumnDeclaration column)
        {
            List<ColumnDeclaration> columns = new List<ColumnDeclaration>(select.Columns) {
                column
            };
            return select.SetColumns(columns);
        }

        public static SelectExpression AddGroupExpression(this SelectExpression select, Expression expression)
        {
            List<Expression> groupBy = new List<Expression>();
            if (select.GroupBy != null)
            {
                groupBy.AddRange(select.GroupBy);
            }
            groupBy.Add(expression);
            return select.SetGroupBy(groupBy);
        }

        public static SelectExpression AddOrderExpression(this SelectExpression select, OrderExpression ordering)
        {
            List<OrderExpression> orderBy = new List<OrderExpression>();
            if (select.OrderBy != null)
            {
                orderBy.AddRange(select.OrderBy);
            }
            orderBy.Add(ordering);
            return select.SetOrderBy(orderBy);
        }

        public static ProjectionExpression AddOuterJoinTest(this ProjectionExpression proj, QueryLanguage language, Expression expression)
        {
            string availableColumnName = proj.Select.Columns.GetAvailableColumnName("Test");
            QueryType columnType = language.TypeSystem.GetColumnType(expression.Type);
            SelectExpression source = proj.Select.AddColumn(new ColumnDeclaration(availableColumnName, expression, columnType));
            return new ProjectionExpression(source, new OuterJoinedExpression(new ColumnExpression(expression.Type, columnType, source.Alias, availableColumnName), proj.Projector), proj.Aggregator);
        }

        public static SelectExpression AddRedundantSelect(this SelectExpression sel, QueryLanguage language, TableAlias newAlias)
        {
            IEnumerable<ColumnDeclaration> columns = from d in sel.Columns
                let qt = (d.Expression is ColumnExpression) ? ((IEnumerable<ColumnDeclaration>) ((ColumnExpression) d.Expression).QueryType) : ((IEnumerable<ColumnDeclaration>) language.TypeSystem.GetColumnType(d.Expression.Type))
                select new ColumnDeclaration(d.Name, new ColumnExpression(d.Expression.Type, qt as QueryType, newAlias, d.Name), qt as QueryType);
            return new SelectExpression(sel.Alias, columns, new SelectExpression(newAlias, sel.Columns, sel.From, sel.Where, sel.OrderBy, sel.GroupBy, sel.IsDistinct, sel.Skip, sel.Take, sel.IsReverse), null, null, null, false, null, null, false);
        }

        public static string GetAvailableColumnName(this IList<ColumnDeclaration> columns, string baseName)
        {
            string name = baseName;
            int num = 0;
            while (!IsUniqueName(columns, name))
            {
                name = baseName + num++;
            }
            return name;
        }

        public static bool IsDbExpression(ExpressionType nodeType)
        {
            return (nodeType >= ((ExpressionType) 0x3e8));
        }

        private static bool IsUniqueName(IList<ColumnDeclaration> columns, string name)
        {
            foreach (ColumnDeclaration declaration in columns)
            {
                if (declaration.Name == name)
                {
                    return false;
                }
            }
            return true;
        }

        public static SelectExpression RemoveColumn(this SelectExpression select, ColumnDeclaration column)
        {
            List<ColumnDeclaration> columns = new List<ColumnDeclaration>(select.Columns);
            columns.Remove(column);
            return select.SetColumns(columns);
        }

        public static SelectExpression RemoveGroupExpression(this SelectExpression select, Expression expression)
        {
            if ((select.GroupBy != null) && (select.GroupBy.Count > 0))
            {
                List<Expression> groupBy = new List<Expression>(select.GroupBy);
                groupBy.Remove(expression);
                return select.SetGroupBy(groupBy);
            }
            return select;
        }

        public static SelectExpression RemoveOrderExpression(this SelectExpression select, OrderExpression ordering)
        {
            if ((select.OrderBy != null) && (select.OrderBy.Count > 0))
            {
                List<OrderExpression> orderBy = new List<OrderExpression>(select.OrderBy);
                orderBy.Remove(ordering);
                return select.SetOrderBy(orderBy);
            }
            return select;
        }

        public static SelectExpression RemoveRedundantFrom(this SelectExpression select)
        {
            SelectExpression from = select.From as SelectExpression;
            if (from != null)
            {
                return SubqueryRemover.Remove(select, new SelectExpression[] { from });
            }
            return select;
        }

        public static SelectExpression SetColumns(this SelectExpression select, IEnumerable<ColumnDeclaration> columns)
        {
            return new SelectExpression(select.Alias, from c in columns
                orderby c.Name
                select c, select.From, select.Where, select.OrderBy, select.GroupBy, select.IsDistinct, select.Skip, select.Take, select.IsReverse);
        }

        public static SelectExpression SetDistinct(this SelectExpression select, bool isDistinct)
        {
            if (select.IsDistinct != isDistinct)
            {
                return new SelectExpression(select.Alias, select.Columns, select.From, select.Where, select.OrderBy, select.GroupBy, isDistinct, select.Skip, select.Take, select.IsReverse);
            }
            return select;
        }

        public static SelectExpression SetFrom(this SelectExpression select, Expression from)
        {
            if (select.From != from)
            {
                return new SelectExpression(select.Alias, select.Columns, from, select.Where, select.OrderBy, select.GroupBy, select.IsDistinct, select.Skip, select.Take, select.IsReverse);
            }
            return select;
        }

        public static SelectExpression SetGroupBy(this SelectExpression select, IEnumerable<Expression> groupBy)
        {
            return new SelectExpression(select.Alias, select.Columns, select.From, select.Where, select.OrderBy, groupBy, select.IsDistinct, select.Skip, select.Take, select.IsReverse);
        }

        public static SelectExpression SetOrderBy(this SelectExpression select, IEnumerable<OrderExpression> orderBy)
        {
            return new SelectExpression(select.Alias, select.Columns, select.From, select.Where, orderBy, select.GroupBy, select.IsDistinct, select.Skip, select.Take, select.IsReverse);
        }

        public static SelectExpression SetReverse(this SelectExpression select, bool isReverse)
        {
            if (select.IsReverse != isReverse)
            {
                return new SelectExpression(select.Alias, select.Columns, select.From, select.Where, select.OrderBy, select.GroupBy, select.IsDistinct, select.Skip, select.Take, isReverse);
            }
            return select;
        }

        public static SelectExpression SetSkip(this SelectExpression select, Expression skip)
        {
            if (skip != select.Skip)
            {
                return new SelectExpression(select.Alias, select.Columns, select.From, select.Where, select.OrderBy, select.GroupBy, select.IsDistinct, skip, select.Take, select.IsReverse);
            }
            return select;
        }

        public static SelectExpression SetTake(this SelectExpression select, Expression take)
        {
            if (take != select.Take)
            {
                return new SelectExpression(select.Alias, select.Columns, select.From, select.Where, select.OrderBy, select.GroupBy, select.IsDistinct, select.Skip, take, select.IsReverse);
            }
            return select;
        }

        public static SelectExpression SetWhere(this SelectExpression select, Expression where)
        {
            if (where != select.Where)
            {
                return new SelectExpression(select.Alias, select.Columns, select.From, where, select.OrderBy, select.GroupBy, select.IsDistinct, select.Skip, select.Take, select.IsReverse);
            }
            return select;
        }
    }
}
