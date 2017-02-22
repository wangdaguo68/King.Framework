namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Linq.Expressions;

    public abstract class DbExpression : Expression
    {
        private readonly ExpressionType _nodeType;
        private readonly System.Type _type;

        protected DbExpression(DbExpressionType eType, System.Type type)
        {
            this._nodeType = (ExpressionType)eType;
            this._type = type;
        }

        public override string ToString()
        {
            return DbExpressionWriter.WriteToString(this);
        }

        public override ExpressionType NodeType
        {
            get
            {
                return this._nodeType;
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
