namespace King.Framework.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;

    public static class ExpressionExtensions
    {
        public static Expression And(this Expression expression1, Expression expression2)
        {
            ConvertExpressions(ref expression1, ref expression2);
            return Expression.And(expression1, expression2);
        }

        public static Expression Binary(this Expression expression1, ExpressionType op, Expression expression2)
        {
            ConvertExpressions(ref expression1, ref expression2);
            return Expression.MakeBinary(op, expression1, expression2);
        }

        private static void ConvertExpressions(ref Expression expression1, ref Expression expression2)
        {
            if (expression1.Type != expression2.Type)
            {
                bool flag = King.Framework.Linq.TypeHelper.IsNullableType(expression1.Type);
                bool flag2 = King.Framework.Linq.TypeHelper.IsNullableType(expression2.Type);
                if ((flag || flag2) && (King.Framework.Linq.TypeHelper.GetNonNullableType(expression1.Type) == King.Framework.Linq.TypeHelper.GetNonNullableType(expression2.Type)))
                {
                    if (!flag)
                    {
                        expression1 = Expression.Convert(expression1, expression2.Type);
                    }
                    else if (!flag2)
                    {
                        expression2 = Expression.Convert(expression2, expression1.Type);
                    }
                }
            }
        }

        public static Expression Equal(this Expression expression1, Expression expression2)
        {
            ConvertExpressions(ref expression1, ref expression2);
            return Expression.Equal(expression1, expression2);
        }

        public static Expression GreaterThan(this Expression expression1, Expression expression2)
        {
            ConvertExpressions(ref expression1, ref expression2);
            return Expression.GreaterThan(expression1, expression2);
        }

        public static Expression GreaterThanOrEqual(this Expression expression1, Expression expression2)
        {
            ConvertExpressions(ref expression1, ref expression2);
            return Expression.GreaterThanOrEqual(expression1, expression2);
        }

        public static Expression Join(this IEnumerable<Expression> list, ExpressionType binarySeparator)
        {
            Func<Expression, Expression, Expression> func = null;
            if (list != null)
            {
                Expression[] source = list.ToArray<Expression>();
                if (source.Length > 0)
                {
                    if (func == null)
                    {
                        func = (x1, x2) => Expression.MakeBinary(binarySeparator, x1, x2);
                    }
                    return source.Aggregate<Expression>(func);
                }
            }
            return null;
        }

        public static Expression LessThan(this Expression expression1, Expression expression2)
        {
            ConvertExpressions(ref expression1, ref expression2);
            return Expression.LessThan(expression1, expression2);
        }

        public static Expression LessThanOrEqual(this Expression expression1, Expression expression2)
        {
            ConvertExpressions(ref expression1, ref expression2);
            return Expression.LessThanOrEqual(expression1, expression2);
        }

        public static Expression NotEqual(this Expression expression1, Expression expression2)
        {
            ConvertExpressions(ref expression1, ref expression2);
            return Expression.NotEqual(expression1, expression2);
        }

        public static Expression Or(this Expression expression1, Expression expression2)
        {
            ConvertExpressions(ref expression1, ref expression2);
            return Expression.Or(expression1, expression2);
        }

        public static Expression[] Split(this Expression expression, params ExpressionType[] binarySeparators)
        {
            List<Expression> list = new List<Expression>();
            Split(expression, list, binarySeparators);
            return list.ToArray();
        }

        private static void Split(Expression expression, List<Expression> list, ExpressionType[] binarySeparators)
        {
            if (expression != null)
            {
                if (binarySeparators.Contains<ExpressionType>(expression.NodeType))
                {
                    BinaryExpression expression2 = expression as BinaryExpression;
                    if (expression2 != null)
                    {
                        Split(expression2.Left, list, binarySeparators);
                        Split(expression2.Right, list, binarySeparators);
                    }
                }
                else
                {
                    list.Add(expression);
                }
            }
        }
    }
}
