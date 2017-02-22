namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Diagnostics;

    public abstract class SubqueryExpression : DbExpression
    {
        private SelectExpression select;

        protected SubqueryExpression(DbExpressionType eType, Type type, SelectExpression select) : base(eType, type)
        {
            Debug.Assert(((eType == DbExpressionType.Scalar) || (eType == DbExpressionType.Exists)) || (eType == DbExpressionType.In));
            this.select = select;
        }

        public SelectExpression Select
        {
            get
            {
                return this.select;
            }
        }
    }
}
