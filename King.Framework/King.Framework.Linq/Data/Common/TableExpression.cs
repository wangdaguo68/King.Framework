namespace King.Framework.Linq.Data.Common
{
    using System;

    public class TableExpression : AliasedExpression
    {
        private MappingEntity entity;
        private string name;

        public TableExpression(TableAlias alias, MappingEntity entity, string name) : base(DbExpressionType.Table, typeof(void), alias)
        {
            this.entity = entity;
            this.name = name;
        }

        public override string ToString()
        {
            return ("T(" + this.Name + ")");
        }

        public MappingEntity Entity
        {
            get
            {
                return this.entity;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }
    }
}
