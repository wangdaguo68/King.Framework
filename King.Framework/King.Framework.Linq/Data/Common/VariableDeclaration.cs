namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Linq.Expressions;

    public class VariableDeclaration
    {
        private System.Linq.Expressions.Expression expression;
        private string name;
        private King.Framework.Linq.Data.Common.QueryType type;

        public VariableDeclaration(string name, King.Framework.Linq.Data.Common.QueryType type, System.Linq.Expressions.Expression expression)
        {
            this.name = name;
            this.type = type;
            this.expression = expression;
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
                return this.type;
            }
        }
    }
}
