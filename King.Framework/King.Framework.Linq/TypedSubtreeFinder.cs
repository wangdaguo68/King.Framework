namespace King.Framework.Linq
{
    using System;
    using System.Linq.Expressions;

    public class TypedSubtreeFinder : King.Framework.Linq.ExpressionVisitor
    {
        private Expression root;
        private Type type;

        private TypedSubtreeFinder(Type type)
        {
            this.type = type;
        }

        public static Expression Find(Expression expression, Type type)
        {
            TypedSubtreeFinder finder = new TypedSubtreeFinder(type);
            finder.Visit(expression);
            return finder.root;
        }

        protected override Expression Visit(Expression exp)
        {
            Expression expression = base.Visit(exp);
            if (((this.root == null) && (expression != null)) && this.type.IsAssignableFrom(expression.Type))
            {
                this.root = expression;
            }
            return expression;
        }
    }
}
