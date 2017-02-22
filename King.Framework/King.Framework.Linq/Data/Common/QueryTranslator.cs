namespace King.Framework.Linq.Data.Common
{
    using King.Framework.Linq;
    using System;
    using System.Linq.Expressions;

    public class QueryTranslator
    {
        private QueryLinguist linguist;
        private QueryMapper mapper;
        private QueryPolice police;

        public QueryTranslator(QueryLanguage language, QueryMapping mapping, QueryPolicy policy)
        {
            this.linguist = language.CreateLinguist(this);
            this.mapper = mapping.CreateMapper(this);
            this.police = policy.CreatePolice(this);
        }

        public virtual Expression Translate(Expression expression)
        {
            QueryMapping mapping = this.mapper.Mapping;
            expression = PartialEvaluator.Eval(expression, new Func<Expression, bool>(mapping.CanBeEvaluatedLocally));
            expression = this.mapper.Translate(expression);
            expression = this.police.Translate(expression);
            expression = this.linguist.Translate(expression);
            return expression;
        }

        public QueryLinguist Linguist
        {
            get
            {
                return this.linguist;
            }
        }

        public QueryMapper Mapper
        {
            get
            {
                return this.mapper;
            }
        }

        public QueryPolice Police
        {
            get
            {
                return this.police;
            }
        }
    }
}
