using System.Collections.Generic;

namespace King.Framework.Linq
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public static class QueryCompiler
    {
        public static D Compile<D>(Expression<D> query)
        {
            return (D) Compile(query);
        }

        public static Func<TResult> Compile<TResult>(Expression<Func<TResult>> query)
        {
            return new Func<TResult>(new CompiledQuery(query).Invoke<TResult>);
        }

        public static Func<T1, TResult> Compile<T1, TResult>(Expression<Func<T1, TResult>> query)
        {
            return new Func<T1, TResult>(new CompiledQuery(query).Invoke<T1, TResult>);
        }

        public static Func<T1, T2, TResult> Compile<T1, T2, TResult>(Expression<Func<T1, T2, TResult>> query)
        {
            return new Func<T1, T2, TResult>(new CompiledQuery(query).Invoke<T1, T2, TResult>);
        }

        public static Func<T1, T2, T3, TResult> Compile<T1, T2, T3, TResult>(Expression<Func<T1, T2, T3, TResult>> query)
        {
            return new Func<T1, T2, T3, TResult>(new CompiledQuery(query).Invoke<T1, T2, T3, TResult>);
        }

        public static Func<T1, T2, T3, T4, TResult> Compile<T1, T2, T3, T4, TResult>(Expression<Func<T1, T2, T3, T4, TResult>> query)
        {
            return new Func<T1, T2, T3, T4, TResult>(new CompiledQuery(query).Invoke<T1, T2, T3, T4, TResult>);
        }

        public static Delegate Compile(LambdaExpression query)
        {
            CompiledQuery query2 = new CompiledQuery(query);
            return StrongDelegate.CreateDelegate(query.Type, new Func<object[], object>(query2.Invoke));
        }

        public static Func<IEnumerable<T>> Compile<T>(this IQueryable<T> source)
        {
            return Compile<IEnumerable<T>>(Expression.Lambda<Func<IEnumerable<T>>>(source.Expression, new ParameterExpression[0]));
        }

        public class CompiledQuery
        {
            private bool checkedForInvoker;
            private Delegate fnQuery;
            private Func<object[], object> invoker;
            private LambdaExpression query;

            internal CompiledQuery(LambdaExpression query)
            {
                this.query = query;
            }

            internal void Compile(params object[] args)
            {
                if (this.fnQuery == null)
                {
                    Expression body = this.query.Body;
                    IQueryProvider provider = this.FindProvider(body, args);
                    if (provider == null)
                    {
                        throw new InvalidOperationException("Could not find query provider");
                    }
                    Delegate delegate2 = (Delegate) provider.Execute(this.query);
                    Interlocked.CompareExchange<Delegate>(ref this.fnQuery, delegate2, null);
                }
            }

            public object FastInvoke1<R>(object[] args)
            {
                return ((Func<R>) this.fnQuery)();
            }

            public object FastInvoke2<A1, R>(object[] args)
            {
                return ((Func<A1, R>) this.fnQuery)((A1) args[0]);
            }

            public object FastInvoke3<A1, A2, R>(object[] args)
            {
                return ((Func<A1, A2, R>) this.fnQuery)((A1) args[0], (A2) args[1]);
            }

            public object FastInvoke4<A1, A2, A3, R>(object[] args)
            {
                return ((Func<A1, A2, A3, R>) this.fnQuery)((A1) args[0], (A2) args[1], (A3) args[2]);
            }

            public object FastInvoke5<A1, A2, A3, A4, R>(object[] args)
            {
                return ((Func<A1, A2, A3, A4, R>) this.fnQuery)((A1) args[0], (A2) args[1], (A3) args[2], (A4) args[3]);
            }

            internal IQueryProvider FindProvider(Expression expression, object[] args)
            {
                Func<object, int, ConstantExpression> selector = null;
                Expression expression2 = this.FindProviderInExpression(expression) as ConstantExpression;
                if (((expression2 == null) && (args != null)) && (args.Length > 0))
                {
                    if (selector == null)
                    {
                        selector = (a, i) => Expression.Constant(a, this.query.Parameters[i].Type);
                    }
                    Expression expression3 = ExpressionReplacer.ReplaceAll(expression, this.query.Parameters.ToArray<ParameterExpression>(), args.Select<object, ConstantExpression>(selector).ToArray<ConstantExpression>());
                    expression2 = this.FindProviderInExpression(expression3);
                }
                if (expression2 != null)
                {
                    ConstantExpression expression4 = expression2 as ConstantExpression;
                    if (expression4 == null)
                    {
                        expression4 = PartialEvaluator.Eval(expression2) as ConstantExpression;
                    }
                    if (expression4 != null)
                    {
                        IQueryProvider provider = expression4.Value as IQueryProvider;
                        if (provider == null)
                        {
                            IQueryable queryable = expression4.Value as IQueryable;
                            if (queryable != null)
                            {
                                provider = queryable.Provider;
                            }
                        }
                        return provider;
                    }
                }
                return null;
            }

            private Expression FindProviderInExpression(Expression expression)
            {
                Expression expression2 = TypedSubtreeFinder.Find(expression, typeof(IQueryProvider));
                if (expression2 == null)
                {
                    expression2 = TypedSubtreeFinder.Find(expression, typeof(IQueryable));
                }
                return expression2;
            }

            private Func<object[], object> GetInvoker()
            {
                if (((this.fnQuery != null) && (this.invoker == null)) && !this.checkedForInvoker)
                {
                    this.checkedForInvoker = true;
                    Type type = this.fnQuery.GetType();
                    if (type.FullName.StartsWith("System.Func`"))
                    {
                        Type[] genericArguments = type.GetGenericArguments();
                        MethodInfo method = base.GetType().GetMethod("FastInvoke" + genericArguments.Length, BindingFlags.Public | BindingFlags.Instance);
                        if (method != null)
                        {
                            this.invoker = (Func<object[], object>) Delegate.CreateDelegate(typeof(Func<object[], object>), this, method.MakeGenericMethod(genericArguments));
                        }
                    }
                }
                return this.invoker;
            }

            internal TResult Invoke<TResult>()
            {
                this.Compile(null);
                return ((Func<TResult>) this.fnQuery)();
            }

            public object Invoke(object[] args)
            {
                object obj2;
                this.Compile(args);
                if (this.invoker == null)
                {
                    this.invoker = this.GetInvoker();
                }
                if (this.invoker != null)
                {
                    return this.invoker(args);
                }
                try
                {
                    obj2 = this.fnQuery.DynamicInvoke(args);
                }
                catch (TargetInvocationException exception)
                {
                    throw exception.InnerException;
                }
                return obj2;
            }

            internal TResult Invoke<T1, TResult>(T1 arg)
            {
                this.Compile(new object[] { arg });
                return ((Func<T1, TResult>) this.fnQuery)(arg);
            }

            internal TResult Invoke<T1, T2, TResult>(T1 arg1, T2 arg2)
            {
                this.Compile(new object[] { arg1, arg2 });
                return ((Func<T1, T2, TResult>) this.fnQuery)(arg1, arg2);
            }

            internal TResult Invoke<T1, T2, T3, TResult>(T1 arg1, T2 arg2, T3 arg3)
            {
                this.Compile(new object[] { arg1, arg2, arg3 });
                return ((Func<T1, T2, T3, TResult>) this.fnQuery)(arg1, arg2, arg3);
            }

            internal TResult Invoke<T1, T2, T3, T4, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
            {
                this.Compile(new object[] { arg1, arg2, arg3, arg4 });
                return ((Func<T1, T2, T3, T4, TResult>) this.fnQuery)(arg1, arg2, arg3, arg4);
            }

            public LambdaExpression Query
            {
                get
                {
                    return this.query;
                }
            }
        }
    }
}
