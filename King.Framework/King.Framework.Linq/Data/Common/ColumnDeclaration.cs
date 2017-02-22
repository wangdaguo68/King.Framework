namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Linq.Expressions;

    public class ColumnDeclaration
    {
        private System.Linq.Expressions.Expression expression;
        private string name;
        private King.Framework.Linq.Data.Common.QueryType queryType;

        public ColumnDeclaration(string name, System.Linq.Expressions.Expression expression, King.Framework.Linq.Data.Common.QueryType queryType)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            if (queryType == null)
            {
                throw new ArgumentNullException("queryType");
            }
            this.name = name;
            this.expression = expression;
            this.queryType = queryType;
        }

        public System.Linq.Expressions.Expression Expression
        {
            get
            {
                return this.expression;
            }
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
    }
}
