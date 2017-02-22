namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Linq.Expressions;

    public class VariableExpression : Expression
    {
        private readonly ExpressionType _nodeType = ((ExpressionType) 0x402);
        private readonly System.Type _type;
        private string name;
        private King.Framework.Linq.Data.Common.QueryType queryType;

        public VariableExpression(string name, System.Type type, King.Framework.Linq.Data.Common.QueryType queryType)
        {
            this._type = type;
            this.name = name;
            this.queryType = queryType;
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public override ExpressionType NodeType
        {
            get
            {
                return this._nodeType;
            }
        }

        public King.Framework.Linq.Data.Common.QueryType QueryType
        {
            get
            {
                return this.queryType;
            }
        }

        public override System.Type Type
        {
            get
            {
                return this._type;
            }
        }
    }
}
