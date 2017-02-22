namespace King.Framework.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.InteropServices;

    public class QueryCache
    {
        private MostRecentlyUsedCache<QueryCompiler.CompiledQuery> cache;
        private static readonly Func<QueryCompiler.CompiledQuery, QueryCompiler.CompiledQuery, bool> fnCompareQueries = new Func<QueryCompiler.CompiledQuery, QueryCompiler.CompiledQuery, bool>(QueryCache.CompareQueries);
        private static readonly Func<object, object, bool> fnCompareValues = new Func<object, object, bool>(QueryCache.CompareConstantValues);

        public QueryCache(int maxSize)
        {
            this.cache = new MostRecentlyUsedCache<QueryCompiler.CompiledQuery>(maxSize, fnCompareQueries);
        }

        public void Clear()
        {
            this.cache.Clear();
        }

        private static bool CompareConstantValues(object x, object y)
        {
            if (x == y)
            {
                return true;
            }
            if ((x == null) || (y == null))
            {
                return false;
            }
            return ((((x is IQueryable) && (y is IQueryable)) && (x.GetType() == y.GetType())) || object.Equals(x, y));
        }

        private static bool CompareQueries(QueryCompiler.CompiledQuery x, QueryCompiler.CompiledQuery y)
        {
            return ExpressionComparer.AreEqual(x.Query, y.Query, fnCompareValues);
        }

        public bool Contains(Expression query)
        {
            object[] objArray;
            return (this.Find(query, false, out objArray) != null);
        }

        public bool Contains(IQueryable query)
        {
            return this.Contains(query.Expression);
        }

        public object Execute(Expression query)
        {
            object[] objArray;
            return this.Find(query, true, out objArray).Invoke(objArray);
        }

        public object Execute(IQueryable query)
        {
            return this.Equals(query.Expression);
        }

        public IEnumerable<T> Execute<T>(IQueryable<T> query)
        {
            return (IEnumerable<T>) this.Execute(query.Expression);
        }

        private QueryCompiler.CompiledQuery Find(Expression query, bool add, out object[] args)
        {
            QueryCompiler.CompiledQuery query3;
            QueryCompiler.CompiledQuery item = new QueryCompiler.CompiledQuery(this.Parameterize(query, out args));
            this.cache.Lookup(item, add, out query3);
            return query3;
        }

        private IQueryProvider FindProvider(Expression expression)
        {
            ConstantExpression expression2 = TypedSubtreeFinder.Find(expression, typeof(IQueryProvider)) as ConstantExpression;
            if (expression2 == null)
            {
                expression2 = TypedSubtreeFinder.Find(expression, typeof(IQueryable)) as ConstantExpression;
            }
            if (expression2 != null)
            {
                IQueryProvider provider = expression2.Value as IQueryProvider;
                if (provider == null)
                {
                    IQueryable queryable = expression2.Value as IQueryable;
                    if (queryable != null)
                    {
                        provider = queryable.Provider;
                    }
                }
                return provider;
            }
            return null;
        }

        private LambdaExpression Parameterize(Expression query, out object[] arguments)
        {
            IQueryProvider provider = this.FindProvider(query);
            if (provider == null)
            {
                throw new ArgumentException("Cannot deduce query provider from query");
            }
            IEntityProvider ep = provider as IEntityProvider;
            Func<Expression, bool> fnCanBeEvaluated = (ep != null) ? new Func<Expression, bool>(ep.CanBeEvaluatedLocally) : null;
            List<ParameterExpression> parameters = new List<ParameterExpression>();
            List<object> values = new List<object>();
            Expression expression = PartialEvaluator.Eval(query, fnCanBeEvaluated, delegate (ConstantExpression c) {
                bool flag = c.Value is IQueryable;
                if (!((flag || (ep == null)) || ep.CanBeParameter(c)))
                {
                    return c;
                }
                ParameterExpression item = Expression.Parameter(c.Type, "p" + parameters.Count);
                parameters.Add(item);
                values.Add(c.Value);
                if (flag)
                {
                    return c;
                }
                return item;
            });
            if (expression.Type != typeof(object))
            {
                expression = Expression.Convert(expression, typeof(object));
            }
            arguments = values.ToArray();
            if (arguments.Length < 5)
            {
                return Expression.Lambda(expression, parameters.ToArray());
            }
            arguments = new object[] { arguments };
            return ExplicitToObjectArray.Rewrite(expression, parameters);
        }

        public int Count
        {
            get
            {
                return this.cache.Count;
            }
        }

        private class ExplicitToObjectArray : King.Framework.Linq.ExpressionVisitor
        {
            private ParameterExpression array = Expression.Parameter(typeof(object[]), "array");
            private IList<ParameterExpression> parameters;

            private ExplicitToObjectArray(IList<ParameterExpression> parameters)
            {
                this.parameters = parameters;
            }

            internal static LambdaExpression Rewrite(Expression body, IList<ParameterExpression> parameters)
            {
                QueryCache.ExplicitToObjectArray array = new QueryCache.ExplicitToObjectArray(parameters);
                return Expression.Lambda(array.Visit(body), new ParameterExpression[] { array.array });
            }

            protected override Expression VisitParameter(ParameterExpression p)
            {
                int num = 0;
                int count = this.parameters.Count;
                while (num < count)
                {
                    if (this.parameters[num] == p)
                    {
                        return Expression.Convert(Expression.ArrayIndex(this.array, Expression.Constant(num)), p.Type);
                    }
                    num++;
                }
                return p;
            }
        }
    }
}
