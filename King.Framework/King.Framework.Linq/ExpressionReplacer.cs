namespace King.Framework.Linq
{
    using System;
    using System.Linq.Expressions;

    public class ExpressionReplacer : King.Framework.Linq.ExpressionVisitor
    {
        private Expression replaceWith;
        private Expression searchFor;

        private ExpressionReplacer(Expression searchFor, Expression replaceWith)
        {
            this.searchFor = searchFor;
            this.replaceWith = replaceWith;
        }

        public static Expression Replace(Expression expression, Expression searchFor, Expression replaceWith)
        {
            return new ExpressionReplacer(searchFor, replaceWith).Visit(expression);
        }

        public static Expression ReplaceAll(Expression expression, Expression[] searchFor, Expression[] replaceWith)
        {
            int index = 0;
            int length = searchFor.Length;
            while (index < length)
            {
                expression = Replace(expression, searchFor[index], replaceWith[index]);
                index++;
            }
            return expression;
        }

        protected override Expression Visit(Expression exp)
        {
            if (exp == this.searchFor)
            {
                return this.replaceWith;
            }
            return base.Visit(exp);
        }
    }
}
