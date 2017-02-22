namespace King.Framework.Linq.Data.Common
{
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public class QueryDuplicator : DbExpressionVisitor
    {
        private Dictionary<TableAlias, TableAlias> map = new Dictionary<TableAlias, TableAlias>();

        public static Expression Duplicate(Expression expression)
        {
            return new QueryDuplicator().Visit(expression);
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

        protected override Expression VisitSelect(SelectExpression select)
        {
            TableAlias alias = new TableAlias();
            this.map[select.Alias] = alias;
            select = (SelectExpression) base.VisitSelect(select);
            return new SelectExpression(alias, select.Columns, select.From, select.Where, select.OrderBy, select.GroupBy, select.IsDistinct, select.Skip, select.Take, select.IsReverse);
        }

        protected override Expression VisitTable(TableExpression table)
        {
            TableAlias alias = new TableAlias();
            this.map[table.Alias] = alias;
            return new TableExpression(alias, table.Entity, table.Name);
        }
    }
}
