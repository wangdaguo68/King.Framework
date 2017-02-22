namespace King.Framework.Common
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    [Serializable]
    public class QueryContext
    {
        public int LoginUserID { get; set; }

        public List<OrderField> OrderFields { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public List<MobileQueryParma> Params { get; set; }

        public string SystemKey { get; set; }

        public int TableVersion { get; set; }

        public long ViewId { get; set; }
    }
}
