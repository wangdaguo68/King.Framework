namespace King.Framework.Linq
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public class Query<T> : IOrderedQueryable<T>, IQueryable<T>, IEnumerable<T>, IOrderedQueryable, IQueryable, IEnumerable
    {
        private System.Linq.Expressions.Expression expression;
        private IQueryProvider provider;

        internal Query(DataContext _context) : this(_context._provider)
        {
        }

        private Query(IQueryProvider provider) : this(provider, (Type) null)
        {
        }

        internal Query(DataContext _context, System.Linq.Expressions.Expression expression) : this(_context._provider, expression)
        {
        }

        public Query(IQueryProvider provider, System.Linq.Expressions.Expression expression)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("Provider");
            }
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
            {
                throw new ArgumentOutOfRangeException("expression");
            }
            this.provider = provider;
            this.expression = expression;
        }

        protected Query(IQueryProvider provider, Type staticType)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("Provider");
            }
            this.provider = provider;
            this.expression = (staticType != null) ? System.Linq.Expressions.Expression.Constant(this, staticType) : System.Linq.Expressions.Expression.Constant(this);
        }

        private static Expression<Func<T1, bool>> FuncToExpression<T1>(Func<T1, bool> f)
        {
            ParameterExpression expression2;
            return System.Linq.Expressions.Expression.Lambda<Func<T1, bool>>(System.Linq.Expressions.Expression.Invoke(System.Linq.Expressions.Expression.Constant(f), new System.Linq.Expressions.Expression[] { expression2 = System.Linq.Expressions.Expression.Parameter(typeof(T1), "x") }), new ParameterExpression[] { expression2 });
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>) this.provider.Execute(this.expression)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) this.provider.Execute(this.expression)).GetEnumerator();
        }

        public override string ToString()
        {
            if ((this.expression.NodeType == ExpressionType.Constant) && (((ConstantExpression) this.expression).Value == this))
            {
                return ("Query(" + typeof(T) + ")");
            }
            return this.expression.ToString();
        }

        public Type ElementType
        {
            get
            {
                return typeof(T);
            }
        }

        public System.Linq.Expressions.Expression Expression
        {
            get
            {
                return this.expression;
            }
        }

        public IQueryProvider Provider
        {
            get
            {
                return this.provider;
            }
        }

        public string QueryText
        {
            get
            {
                IQueryText provider = this.provider as IQueryText;
                if (provider != null)
                {
                    return provider.GetQueryText(this.expression);
                }
                return "";
            }
        }
    }
}
