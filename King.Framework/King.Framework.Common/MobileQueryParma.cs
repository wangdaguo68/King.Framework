namespace King.Framework.Common
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    [Serializable]
    public class MobileQueryParma
    {
        public int CompareTypeEnum { get; set; }

        public King.Framework.Common.QueryField QueryField { get; set; }

        public List<King.Framework.Common.QueryField> QueryFieldList { get; set; }

        public object Value { get; set; }
    }
}
