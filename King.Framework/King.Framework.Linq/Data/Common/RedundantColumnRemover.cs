namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public class RedundantColumnRemover : DbExpressionVisitor
    {
        private Dictionary<ColumnExpression, ColumnExpression> map = new Dictionary<ColumnExpression, ColumnExpression>();

        private RedundantColumnRemover()
        {
        }

        public static Expression Remove(Expression expression)
        {
            return new RedundantColumnRemover().Visit(expression);
        }

        private bool SameExpression(Expression a, Expression b)
        {
            if (a == b)
            {
                return true;
            }
            ColumnExpression expression = a as ColumnExpression;
            ColumnExpression expression2 = b as ColumnExpression;
            return ((((expression != null) && (expression2 != null)) && (expression.Alias == expression2.Alias)) && (expression.Name == expression2.Name));
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

        protected override Expression VisitSelect(SelectExpression selectExpression)
        {
            selectExpression = (SelectExpression) base.VisitSelect(selectExpression);
            List<ColumnDeclaration> list = (from c in selectExpression.Columns
                orderby c.Name
                select c).ToList<ColumnDeclaration>();
            BitArray array = new BitArray(selectExpression.Columns.Count);
            bool flag = false;
            int index = 0;
            int count = list.Count;
            while (index < (count - 1))
            {
                ColumnDeclaration declaration = list[index];
                ColumnExpression expression = declaration.Expression as ColumnExpression;
                QueryType queryType = (expression != null) ? expression.QueryType : declaration.QueryType;
                ColumnExpression expression2 = new ColumnExpression(declaration.Expression.Type, queryType, selectExpression.Alias, declaration.Name);
                for (int i = index + 1; i < count; i++)
                {
                    if (!array.Get(i))
                    {
                        ColumnDeclaration declaration2 = list[i];
                        if (this.SameExpression(declaration.Expression, declaration2.Expression))
                        {
                            ColumnExpression key = new ColumnExpression(declaration2.Expression.Type, queryType, selectExpression.Alias, declaration2.Name);
                            this.map.Add(key, expression2);
                            array.Set(i, true);
                            flag = true;
                        }
                    }
                }
                index++;
            }
            if (flag)
            {
                List<ColumnDeclaration> columns = new List<ColumnDeclaration>();
                index = 0;
                count = list.Count;
                while (index < count)
                {
                    if (!array.Get(index))
                    {
                        columns.Add(list[index]);
                    }
                    index++;
                }
                selectExpression = selectExpression.SetColumns(columns);
            }
            return selectExpression;
        }
    }
}
