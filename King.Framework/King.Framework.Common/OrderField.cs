namespace King.Framework.Common
{
    using King.Framework.EntityLibrary;
    using System;
    using System.Runtime.CompilerServices;

    [Serializable]
    public class OrderField
    {
        public long FieldId { get; set; }

        public OrderMethodEnum OrderMethod { get; set; }

        public long? RelationId { get; set; }
    }
}
