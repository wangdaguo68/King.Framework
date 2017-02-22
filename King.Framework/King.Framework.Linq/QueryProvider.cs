namespace King.Framework.Linq
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public abstract class QueryProvider : IQueryProvider, IQueryText
    {
        internal readonly DataContext _context;

        protected QueryProvider(DataContext context)
        {
            this._context = context;
        }

        public abstract object Execute(Expression expression);
        public abstract string GetQueryText(Expression expression);
        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            IQueryable queryable;
            Type elementType = King.Framework.Linq.TypeHelper.GetElementType(expression.Type);
            try
            {
                queryable = (IQueryable) Activator.CreateInstance(typeof(Query<>).MakeGenericType(new Type[] { elementType }), new object[] { this, expression });
            }
            catch (TargetInvocationException exception)
            {
                throw exception.InnerException;
            }
            return queryable;
        }

        IQueryable<S> IQueryProvider.CreateQuery<S>(Expression expression)
        {
            return new Query<S>(this._context, expression);
        }

        object IQueryProvider.Execute(Expression expression)
        {
            return this.Execute(expression);
        }

        S IQueryProvider.Execute<S>(Expression expression)
        {
            return (S) this.Execute(expression);
        }

        public DataContext Context
        {
            get
            {
                return this._context;
            }
        }
    }
}
