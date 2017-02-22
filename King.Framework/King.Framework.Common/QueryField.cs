namespace King.Framework.Common
{
    using System;
    using System.Runtime.CompilerServices;

    [Serializable]
    public class QueryField
    {
        public long? ControlId { get; set; }

        public string FieldName { get; set; }
    }
}
