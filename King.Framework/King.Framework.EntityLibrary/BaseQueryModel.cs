namespace King.Framework.EntityLibrary
{
    using System;
    using System.Runtime.CompilerServices;

    public class BaseQueryModel
    {
        public string _gridName { get; set; }

        public string orderBy { get; set; }

        public OrderMethodEnum orderMethod { get; set; }

        public int pageIndex { get; set; }

        public int pageSize { get; set; }
    }
}

