namespace King.Framework.Common
{
    using System;
    using System.Runtime.CompilerServices;

    [Serializable]
    public class QueryExtendField
    {
        public long FieldId { get; set; }

        public long? RefRelationId { get; set; }
    }
}
