namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal class WhereBinder : QueryBinder
    {
        public WhereBinder(QueryMapper mapper, Expression root, Dictionary<ParameterExpression, Expression> map, Dictionary<Expression, QueryBinder.GroupByInfo> groupByMap) : base(mapper, root, map, groupByMap)
        {
        }

        protected Expression Visit2(Expression exp)
        {
            Expression expression = this.Visit(exp);
            if (expression.NodeType == ((ExpressionType) 0x3ea))
            {
                if ((expression.Type == typeof(bool)) || (expression.Type == typeof(bool?)))
                {
                }
                return expression;
            }
            return expression;
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            Expression left = this.Visit(b.Left);
            if (left.Type != b.Left.Type)
            {
            }
            Expression right = this.Visit(b.Right);
            if (left.Type != right.Type)
            {
            }
            Expression conversion = this.Visit(b.Conversion);
            return base.UpdateBinary(b, left, right, conversion, b.IsLiftedToNull, b.Method);
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            return base.VisitMemberAccess(m);
        }
    }
}
