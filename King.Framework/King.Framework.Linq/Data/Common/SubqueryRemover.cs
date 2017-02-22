namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public class SubqueryRemover : DbExpressionVisitor
    {
        private Dictionary<TableAlias, Dictionary<string, Expression>> map;
        private HashSet<SelectExpression> selectsToRemove;

        private SubqueryRemover(IEnumerable<SelectExpression> selectsToRemove)
        {
            this.selectsToRemove = new HashSet<SelectExpression>(selectsToRemove);
            this.map = this.selectsToRemove.ToDictionary<SelectExpression, TableAlias, Dictionary<string, Expression>>(d => d.Alias, d => d.Columns.ToDictionary<ColumnDeclaration, string, Expression>(d2 => d2.Name, d2 => d2.Expression));
        }

        public static ProjectionExpression Remove(ProjectionExpression projection, IEnumerable<SelectExpression> selectsToRemove)
        {
            return (ProjectionExpression) new SubqueryRemover(selectsToRemove).Visit(projection);
        }

        public static ProjectionExpression Remove(ProjectionExpression projection, params SelectExpression[] selectsToRemove)
        {
            return Remove(projection, (IEnumerable<SelectExpression>) selectsToRemove);
        }

        public static SelectExpression Remove(SelectExpression outerSelect, IEnumerable<SelectExpression> selectsToRemove)
        {
            return (SelectExpression) new SubqueryRemover(selectsToRemove).Visit(outerSelect);
        }

        public static SelectExpression Remove(SelectExpression outerSelect, params SelectExpression[] selectsToRemove)
        {
            return Remove(outerSelect, (IEnumerable<SelectExpression>) selectsToRemove);
        }

        protected override Expression VisitColumn(ColumnExpression column)
        {
            Dictionary<string, Expression> dictionary;
            if (this.map.TryGetValue(column.Alias, out dictionary))
            {
                Expression expression;
                if (!dictionary.TryGetValue(column.Name, out expression))
                {
                    throw new Exception("Reference to undefined column");
                }
                return this.Visit(expression);
            }
            return column;
        }

        protected override Expression VisitSelect(SelectExpression select)
        {
            if (this.selectsToRemove.Contains(select))
            {
                return this.Visit(select.From);
            }
            return base.VisitSelect(select);
        }
    }
}
