namespace King.Framework.LiteQueryDef.Internal
{
    using System;

    public class LiteOrderField
    {
        public readonly string FieldName;
        public readonly King.Framework.LiteQueryDef.Internal.OrderMethod OrderMethod;
        public readonly string TableAlias;

        public LiteOrderField(string tableAlias, string fieldName) : this(tableAlias, fieldName, King.Framework.LiteQueryDef.Internal.OrderMethod.ASC)
        {
        }

        public LiteOrderField(string tableAlias, string fieldName, King.Framework.LiteQueryDef.Internal.OrderMethod orderMethod)
        {
            this.TableAlias = tableAlias;
            this.FieldName = fieldName;
            this.OrderMethod = orderMethod;
        }
    }
}
