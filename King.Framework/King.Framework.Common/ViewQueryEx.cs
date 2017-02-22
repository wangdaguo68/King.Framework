namespace King.Framework.Common
{
    using King.Framework.LiteQueryDef.Internal;
    using System;
    using System.Runtime.CompilerServices;

    public class ViewQueryEx
    {
        public CompareTypeEnum CompareType { get; set; }

        public object ConditionValue { get; set; }

        public string FieldName { get; set; }

        public LiteQuery SubQuery { get; set; }

        public string TableAlias { get; set; }
    }
}
