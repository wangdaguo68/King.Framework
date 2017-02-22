namespace King.Framework.Linq.Data.SqlClient
{
    using King.Framework.Linq;
    using King.Framework.Linq.Data;
    using King.Framework.Linq.Data.Common;
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading;

    public class TSqlLanguage : QueryLanguage
    {
        private static TSqlLanguage _default;
        private static readonly char[] splitChars = new char[] { '.' };
        private DbTypeSystem typeSystem = new DbTypeSystem();

        public override QueryLinguist CreateLinguist(QueryTranslator translator)
        {
            return new TSqlLinguist(this, translator);
        }

        public override Expression GetGeneratedIdExpression(MemberInfo member)
        {
            return new FunctionExpression(TypeHelper.GetMemberType(member), "SCOPE_IDENTITY()", null);
        }

        public override string Quote(string name)
        {
            if (name.StartsWith("[") && name.EndsWith("]"))
            {
                return name;
            }
            if (name.IndexOf('.') > 0)
            {
                return ("[" + string.Join("].[", name.Split(splitChars, StringSplitOptions.RemoveEmptyEntries)) + "]");
            }
            return ("[" + name + "]");
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
                return true;
            }
        }

        public override bool AllowSubqueryInSelectWithoutFrom
        {
            get
            {
                return true;
            }
        }

        public static TSqlLanguage Default
        {
            get
            {
                if (_default == null)
                {
                    Interlocked.CompareExchange<TSqlLanguage>(ref _default, new TSqlLanguage(), null);
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

        private class TSqlLinguist : QueryLinguist
        {
            public TSqlLinguist(TSqlLanguage language, QueryTranslator translator) : base(language, translator)
            {
            }

            public override string Format(Expression expression)
            {
                return TSqlFormatter.Format(expression, base.Language);
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
