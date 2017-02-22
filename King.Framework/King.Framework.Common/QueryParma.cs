namespace King.Framework.Common
{
    using King.Framework.LiteQueryDef.Internal;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    [Serializable]
    public class QueryParma
    {
        public override int GetHashCode()
        {
            return ((this.QueryField.FieldName.GetHashCode() ^ this.CompareTypeEnum.GetHashCode()) ^ this.Value.GetHashCode());
        }

        public King.Framework.LiteQueryDef.Internal.CompareTypeEnum CompareTypeEnum { get; set; }

        public FullTextTypeEnum FullTextType { get; set; }

        public King.Framework.Common.QueryField QueryField { get; set; }

        public List<King.Framework.Common.QueryField> QueryFieldList { get; set; }

        public object Value { get; set; }
    }
}
