namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    public class QueryPolice
    {
        private QueryPolicy policy;
        private QueryTranslator translator;

        public QueryPolice(QueryPolicy policy, QueryTranslator translator)
        {
            this.policy = policy;
            this.translator = translator;
        }

        public virtual Expression ApplyPolicy(Expression expression, MemberInfo member)
        {
            return expression;
        }

        public virtual Expression BuildExecutionPlan(Expression query, Expression provider)
        {
            return ExecutionBuilder.Build(this.translator.Linguist, this.policy, query, provider);
        }

        public virtual Expression Translate(Expression expression)
        {
            Expression expression2 = RelationshipIncluder.Include(this.translator.Mapper, expression);
            if (expression2 != expression)
            {
                expression = expression2;
                expression = UnusedColumnRemover.Remove(expression);
                expression = RedundantColumnRemover.Remove(expression);
                expression = RedundantSubqueryRemover.Remove(expression);
                expression = RedundantJoinRemover.Remove(expression);
            }
            expression2 = SingletonProjectionRewriter.Rewrite(this.translator.Linguist.Language, expression);
            if (expression2 != expression)
            {
                expression = expression2;
                expression = UnusedColumnRemover.Remove(expression);
                expression = RedundantColumnRemover.Remove(expression);
                expression = RedundantSubqueryRemover.Remove(expression);
                expression = RedundantJoinRemover.Remove(expression);
            }
            expression2 = ClientJoinedProjectionRewriter.Rewrite(this.policy, this.translator.Linguist.Language, expression);
            if (expression2 != expression)
            {
                expression = expression2;
                expression = UnusedColumnRemover.Remove(expression);
                expression = RedundantColumnRemover.Remove(expression);
                expression = RedundantSubqueryRemover.Remove(expression);
                expression = RedundantJoinRemover.Remove(expression);
            }
            return expression;
        }

        public QueryPolicy Policy
        {
            get
            {
                return this.policy;
            }
        }

        public QueryTranslator Translator
        {
            get
            {
                return this.translator;
            }
        }
    }
}
