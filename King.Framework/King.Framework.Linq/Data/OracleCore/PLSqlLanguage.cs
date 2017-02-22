namespace King.Framework.Linq.Data.OracleCore
{
    using King.Framework.Linq;
    using King.Framework.Linq.Data;
    using King.Framework.Linq.Data.Common;
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading;

    public class PLSqlLanguage : QueryLanguage
    {
        private static PLSqlLanguage _default;
        private static readonly char[] splitChars = new char[] { '.' };
        private DbTypeSystem typeSystem = new DbTypeSystem();

        public override QueryLinguist CreateLinguist(QueryTranslator translator)
        {
            return new PLSqlLinguist(this, translator);
        }

        public override Expression GetGeneratedIdExpression(MemberInfo member)
        {
            return new FunctionExpression(King.Framework.Linq.TypeHelper.GetMemberType(member), "NEXTID.CURRVAL", null);
        }

        public override string Quote(string name)
        {
            name = name.ToUpper();
            if (name.StartsWith("\"") && name.EndsWith("\""))
            {
                return name;
            }
            if (name.IndexOf('.') > 0)
            {
                name = name.Split(splitChars, StringSplitOptions.RemoveEmptyEntries).Last<string>();
                return ("\"" + name + "\"");
            }
            return ("\"" + name + "\"");
        }

        public override bool AllowDistinctInAggregates
        {
            get
            {
                return true;
            }
        }

        public override bool AllowsMultipleCommands
        {
            get
            {
                return false;
            }
        }

        public override bool AllowSubqueryInSelectWithoutFrom
        {
            get
            {
                return true;
            }
        }

        public static PLSqlLanguage Default
        {
            get
            {
                if (_default == null)
                {
                    Interlocked.CompareExchange<PLSqlLanguage>(ref _default, new PLSqlLanguage(), null);
                }
                return _default;
            }
        }

        public override QueryTypeSystem TypeSystem
        {
            get
            {
                return this.typeSystem;
            }
        }

        private class PLSqlLinguist : QueryLinguist
        {
            public PLSqlLinguist(PLSqlLanguage language, QueryTranslator translator) : base(language, translator)
            {
            }

            public override string Format(Expression expression)
            {
                return PLSqlFormatter.Format(expression, base.Language);
            }

            public override Expression Translate(Expression expression)
            {
                expression = OrderByRewriter.Rewrite(base.Language, expression);
                expression = base.Translate(expression);
                expression = SkipToRowNumberRewriter.Rewrite(base.Language, expression);
                expression = OrderByRewriter.Rewrite(base.Language, expression);
                return expression;
            }
        }
    }
}
