namespace King.Framework.Linq.Data.Common
{
    using System;

    public abstract class AliasedExpression : DbExpression
    {
        private TableAlias alias;

        protected AliasedExpression(DbExpressionType nodeType, Type type, TableAlias alias) : base(nodeType, type)
        {
            this.alias = alias;
        }

        public TableAlias Alias
        {
            get
            {
                return this.alias;
            }
        }
    }
}
