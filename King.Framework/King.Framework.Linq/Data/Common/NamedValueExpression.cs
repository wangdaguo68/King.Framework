namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Linq.Expressions;

    public class NamedValueExpression : DbExpression
    {
        private string name;
        private King.Framework.Linq.Data.Common.QueryType queryType;
        private Expression value;

        public NamedValueExpression(string name, King.Framework.Linq.Data.Common.QueryType queryType, Expression value) : base(DbExpressionType.NamedValue, value.Type)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            this.name = name;
            this.queryType = queryType;
            this.value = value;
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public King.Framework.Linq.Data.Common.QueryType QueryType
        {
            get
            {
                return this.queryType;
            }
        }

        public Expression Value
        {
            get
            {
                return this.value;
            }
        }
    }
}
