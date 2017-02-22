namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    public abstract class QueryMapper
    {
        protected QueryMapper()
        {
        }

        public virtual Expression ApplyMapping(Expression expression)
        {
            return QueryBinder.Bind(this, expression);
        }

        public abstract Expression GetDeleteExpression(MappingEntity entity, Expression instance, LambdaExpression deleteCheck);
        public abstract EntityExpression GetEntityExpression(Expression root, MappingEntity entity);
        public abstract Expression GetInsertExpression(MappingEntity entity, Expression instance, LambdaExpression selector);
        public abstract Expression GetInsertOrUpdateExpression(MappingEntity entity, Expression instance, LambdaExpression updateCheck, LambdaExpression resultSelector);
        public abstract Expression GetMemberExpression(Expression root, MappingEntity entity, MemberInfo member);
        public abstract ProjectionExpression GetQueryExpression(MappingEntity entity);
        public abstract Expression GetUpdateExpression(MappingEntity entity, Expression instance, LambdaExpression updateCheck, LambdaExpression selector, Expression @else);
        public abstract bool HasIncludedMembers(EntityExpression entity);
        public abstract EntityExpression IncludeMembers(EntityExpression entity, Func<MemberInfo, bool> fnIsIncluded);
        public virtual Expression Translate(Expression expression)
        {
            expression = QueryBinder.Bind(this, expression);
            expression = AggregateRewriter.Rewrite(this.Translator.Linguist.Language, expression);
            expression = UnusedColumnRemover.Remove(expression);
            expression = RedundantColumnRemover.Remove(expression);
            expression = RedundantSubqueryRemover.Remove(expression);
            expression = RedundantJoinRemover.Remove(expression);
            Expression expression2 = RelationshipBinder.Bind(this, expression);
            if (expression2 != expression)
            {
                expression = expression2;
                expression = RedundantColumnRemover.Remove(expression);
                expression = RedundantJoinRemover.Remove(expression);
            }
            expression = ComparisonRewriter.Rewrite(this.Mapping, expression);
            return expression;
        }

        public abstract QueryMapping Mapping { get; }

        public abstract QueryTranslator Translator { get; }
    }
}
