namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Linq.Expressions;

    public class QueryLinguist
    {
        private QueryLanguage language;
        private QueryTranslator translator;

        public QueryLinguist(QueryLanguage language, QueryTranslator translator)
        {
            this.language = language;
            this.translator = translator;
        }

        public virtual string Format(Expression expression)
        {
            return SqlFormatter.Format(expression);
        }

        public virtual Expression Parameterize(Expression expression)
        {
            return Parameterizer.Parameterize(this.language, expression);
        }

        public virtual Expression Translate(Expression expression)
        {
            expression = UnusedColumnRemover.Remove(expression);
            expression = RedundantColumnRemover.Remove(expression);
            expression = RedundantSubqueryRemover.Remove(expression);
            Expression expression2 = CrossJoinRewriter.Rewrite(CrossApplyRewriter.Rewrite(this.language, expression));
            if (expression2 != expression)
            {
                expression = expression2;
                expression = UnusedColumnRemover.Remove(expression);
                expression = RedundantSubqueryRemover.Remove(expression);
                expression = RedundantJoinRemover.Remove(expression);
                expression = RedundantColumnRemover.Remove(expression);
            }
            return expression;
        }

        public QueryLanguage Language
        {
            get
            {
                return this.language;
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
