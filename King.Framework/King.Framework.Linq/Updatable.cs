namespace King.Framework.Linq
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public static class Updatable
    {
        public static IEnumerable<S> Batch<U, T, S>(this IUpdatable<U> collection, IEnumerable<T> instances, Expression<Func<IUpdatable<U>, T, S>> fnOperation)
        {
            return collection.Batch<U, T, S>(instances, fnOperation, 50, false);
        }

        public static IEnumerable Batch(IUpdatable collection, IEnumerable items, LambdaExpression fnOperation, int batchSize, bool stream)
        {
            MethodCallExpression expression = Expression.Call(null, (MethodInfo) MethodBase.GetCurrentMethod(), new Expression[] { collection.Expression, Expression.Constant(items), (fnOperation != null) ? ((Expression) Expression.Quote(fnOperation)) : ((Expression) Expression.Constant(null, typeof(LambdaExpression))), Expression.Constant(batchSize), Expression.Constant(stream) });
            return (IEnumerable) collection.Provider.Execute(expression);
        }

        public static IEnumerable<S> Batch<U, T, S>(this IUpdatable<U> collection, IEnumerable<T> instances, Expression<Func<IUpdatable<U>, T, S>> fnOperation, int batchSize, bool stream)
        {
            MethodCallExpression expression = Expression.Call(null, ((MethodInfo) MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(U), typeof(T), typeof(S) }), new Expression[] { collection.Expression, Expression.Constant(instances), (fnOperation != null) ? ((Expression) Expression.Quote(fnOperation)) : ((Expression) Expression.Constant(null, typeof(Expression<Func<IUpdatable<U>, T, S>>))), Expression.Constant(batchSize), Expression.Constant(stream) });
            return (IEnumerable<S>) collection.Provider.Execute(expression);
        }

        public static int Delete(IUpdatable collection, LambdaExpression predicate)
        {
            MethodCallExpression expression = Expression.Call(null, (MethodInfo) MethodBase.GetCurrentMethod(), collection.Expression, (predicate != null) ? ((Expression) Expression.Quote(predicate)) : ((Expression) Expression.Constant(null, typeof(LambdaExpression))));
            return (int) collection.Provider.Execute(expression);
        }

        public static int Delete<T>(this IUpdatable<T> collection, T instance)
        {
            return collection.Delete<T>(instance, null);
        }

        public static int Delete<T>(this IUpdatable<T> collection, Expression<Func<T, bool>> predicate)
        {
            MethodCallExpression expression = Expression.Call(null, ((MethodInfo) MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(T) }), collection.Expression, (predicate != null) ? ((Expression) Expression.Quote(predicate)) : ((Expression) Expression.Constant(null, typeof(Expression<Func<T, bool>>))));
            return (int) collection.Provider.Execute(expression);
        }

        public static object Delete(IUpdatable collection, object instance, LambdaExpression deleteCheck)
        {
            MethodCallExpression expression = Expression.Call(null, (MethodInfo) MethodBase.GetCurrentMethod(), collection.Expression, Expression.Constant(instance), (deleteCheck != null) ? ((Expression) Expression.Quote(deleteCheck)) : ((Expression) Expression.Constant(null, typeof(LambdaExpression))));
            return collection.Provider.Execute(expression);
        }

        public static int Delete<T>(this IUpdatable<T> collection, T instance, Expression<Func<T, bool>> deleteCheck)
        {
            MethodCallExpression expression = Expression.Call(null, ((MethodInfo) MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(T) }), collection.Expression, Expression.Constant(instance), (deleteCheck != null) ? ((Expression) Expression.Quote(deleteCheck)) : ((Expression) Expression.Constant(null, typeof(Expression<Func<T, bool>>))));
            return (int) collection.Provider.Execute(expression);
        }

        public static int Insert<T>(this IUpdatable<T> collection, T instance)
        {
            return collection.Insert<T, int>(instance, null);
        }

        public static object Insert(IUpdatable collection, object instance, LambdaExpression resultSelector)
        {
            MethodCallExpression expression = Expression.Call(null, (MethodInfo) MethodBase.GetCurrentMethod(), collection.Expression, Expression.Constant(instance), (resultSelector != null) ? ((Expression) Expression.Quote(resultSelector)) : ((Expression) Expression.Constant(null, typeof(LambdaExpression))));
            return collection.Provider.Execute(expression);
        }

        public static S Insert<T, S>(this IUpdatable<T> collection, T instance, Expression<Func<T, S>> resultSelector)
        {
            MethodCallExpression expression = Expression.Call(null, ((MethodInfo) MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(T), typeof(S) }), collection.Expression, Expression.Constant(instance), (resultSelector != null) ? ((Expression) Expression.Quote(resultSelector)) : ((Expression) Expression.Constant(null, typeof(Expression<Func<T, S>>))));
            return (S) collection.Provider.Execute(expression);
        }

        public static int InsertOrUpdate<T>(this IUpdatable<T> collection, T instance)
        {
            return collection.InsertOrUpdate<T, int>(instance, null, null);
        }

        public static int InsertOrUpdate<T>(this IUpdatable<T> collection, T instance, Expression<Func<T, bool>> updateCheck)
        {
            return collection.InsertOrUpdate<T, int>(instance, updateCheck, null);
        }

        public static object InsertOrUpdate(IUpdatable collection, object instance, LambdaExpression updateCheck, LambdaExpression resultSelector)
        {
            MethodCallExpression expression = Expression.Call(null, (MethodInfo) MethodBase.GetCurrentMethod(), new Expression[] { collection.Expression, Expression.Constant(instance), (updateCheck != null) ? ((Expression) Expression.Quote(updateCheck)) : ((Expression) Expression.Constant(null, typeof(LambdaExpression))), (resultSelector != null) ? ((Expression) Expression.Quote(resultSelector)) : ((Expression) Expression.Constant(null, typeof(LambdaExpression))) });
            return collection.Provider.Execute(expression);
        }

        public static S InsertOrUpdate<T, S>(this IUpdatable<T> collection, T instance, Expression<Func<T, bool>> updateCheck, Expression<Func<T, S>> resultSelector)
        {
            MethodCallExpression expression = Expression.Call(null, ((MethodInfo) MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(T), typeof(S) }), new Expression[] { collection.Expression, Expression.Constant(instance), (updateCheck != null) ? ((Expression) Expression.Quote(updateCheck)) : ((Expression) Expression.Constant(null, typeof(Expression<Func<T, bool>>))), (resultSelector != null) ? ((Expression) Expression.Quote(resultSelector)) : ((Expression) Expression.Constant(null, typeof(Expression<Func<T, S>>))) });
            return (S) collection.Provider.Execute(expression);
        }

        public static int Update<T>(this IUpdatable<T> collection, T instance)
        {
            return collection.Update<T, int>(instance, null, null);
        }

        public static int Update<T>(this IUpdatable<T> collection, T instance, Expression<Func<T, bool>> updateCheck)
        {
            return collection.Update<T, int>(instance, updateCheck, null);
        }

        public static object Update(IUpdatable collection, object instance, LambdaExpression updateCheck, LambdaExpression resultSelector)
        {
            MethodCallExpression expression = Expression.Call(null, (MethodInfo) MethodBase.GetCurrentMethod(), new Expression[] { collection.Expression, Expression.Constant(instance), (updateCheck != null) ? ((Expression) Expression.Quote(updateCheck)) : ((Expression) Expression.Constant(null, typeof(LambdaExpression))), (resultSelector != null) ? ((Expression) Expression.Quote(resultSelector)) : ((Expression) Expression.Constant(null, typeof(LambdaExpression))) });
            return collection.Provider.Execute(expression);
        }

        public static S Update<T, S>(this IUpdatable<T> collection, T instance, Expression<Func<T, bool>> updateCheck, Expression<Func<T, S>> resultSelector)
        {
            MethodCallExpression expression = Expression.Call(null, ((MethodInfo) MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(T), typeof(S) }), new Expression[] { collection.Expression, Expression.Constant(instance), (updateCheck != null) ? ((Expression) Expression.Quote(updateCheck)) : ((Expression) Expression.Constant(null, typeof(Expression<Func<T, bool>>))), (resultSelector != null) ? ((Expression) Expression.Quote(resultSelector)) : ((Expression) Expression.Constant(null, typeof(Expression<Func<T, S>>))) });
            return (S) collection.Provider.Execute(expression);
        }
    }
}
