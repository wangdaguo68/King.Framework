namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [Serializable, KingTable]
    public class SysServiceCallLog
    {
        [KingColumn]
        public int? InvokeState { get; set; }

        [KingColumn]
        public string LogDesc { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public int LogId { get; set; }

        [KingColumn]
        public DateTime? LogTime { get; set; }

        [KingColumn]
        public int? ServiceCallId { get; set; }
    }
}

