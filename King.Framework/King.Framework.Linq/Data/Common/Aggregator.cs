namespace King.Framework.Linq.Data.Common
{
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public static class Aggregator
    {
        private static Expression CoerceElement(Type expectedElementType, Expression expression)
        {
            Type elementType = King.Framework.Linq.TypeHelper.GetElementType(expression.Type);
            if ((expectedElementType != elementType) && (expectedElementType.IsAssignableFrom(elementType) || elementType.IsAssignableFrom(expectedElementType)))
            {
                return Expression.Call(typeof(Enumerable), "Cast", new Type[] { expectedElementType }, new Expression[] { expression });
            }
            return expression;
        }

        public static LambdaExpression GetAggregator(Type expectedType, Type actualType)
        {
            Type elementType = King.Framework.Linq.TypeHelper.GetElementType(actualType);
            if (!expectedType.IsAssignableFrom(actualType))
            {
                Type expectedElementType = King.Framework.Linq.TypeHelper.GetElementType(expectedType);
                ParameterExpression expression = Expression.Parameter(actualType, "p");
                Expression expression2 = null;
                if (expectedType.IsAssignableFrom(elementType))
                {
                    expression2 = Expression.Call(typeof(Enumerable), "SingleOrDefault", new Type[] { elementType }, new Expression[] { expression });
                }
                else if (expectedType.IsGenericType && ((((expectedType == typeof(IQueryable)) || (expectedType == typeof(IOrderedQueryable))) || (expectedType.GetGenericTypeDefinition() == typeof(IQueryable<>))) || (expectedType.GetGenericTypeDefinition() == typeof(IOrderedQueryable<>))))
                {
                    expression2 = Expression.Call(typeof(Queryable), "AsQueryable", new Type[] { expectedElementType }, new Expression[] { CoerceElement(expectedElementType, expression) });
                    if (expression2.Type != expectedType)
                    {
                        expression2 = Expression.Convert(expression2, expectedType);
                    }
                }
                else if (expectedType.IsArray && (expectedType.GetArrayRank() == 1))
                {
                    expression2 = Expression.Call(typeof(Enumerable), "ToArray", new Type[] { expectedElementType }, new Expression[] { CoerceElement(expectedElementType, expression) });
                }
                else if (expectedType.IsGenericType && expectedType.GetGenericTypeDefinition().IsAssignableFrom(typeof(IList<>)))
                {
                    expression2 = Expression.New(typeof(DeferredList<>).MakeGenericType(expectedType.GetGenericArguments()).GetConstructor(new Type[] { typeof(IEnumerable<>).MakeGenericType(expectedType.GetGenericArguments()) }), new Expression[] { CoerceElement(expectedElementType, expression) });
                }
                else if (expectedType.IsAssignableFrom(typeof(List<>).MakeGenericType(new Type[] { elementType })))
                {
                    expression2 = Expression.Call(typeof(Enumerable), "ToList", new Type[] { expectedElementType }, new Expression[] { CoerceElement(expectedElementType, expression) });
                }
                else
                {
                    ConstructorInfo constructor = expectedType.GetConstructor(new Type[] { actualType });
                    if (constructor != null)
                    {
                        expression2 = Expression.New(constructor, new Expression[] { expression });
                    }
                }
                if (expression2 != null)
                {
                    return Expression.Lambda(expression2, new ParameterExpression[] { expression });
                }
            }
            return null;
        }
    }
}
