namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [Serializable, KingTable]
    public class SysServiceCall
    {
        [KingColumn]
        public string CallBackAshxAddr { get; set; }

        [KingColumn]
        public string CallBackAshxParameter { get; set; }

        [KingColumn]
        public DateTime? CallTime { get; set; }

        [KingColumn]
        public int? InvokeErrorTimes { get; set; }

        [KingColumn]
        public string InvokeResult { get; set; }

        [KingColumn]
        public DateTime? InvokeTime { get; set; }

        [KingColumn]
        public string Parameter1 { get; set; }

        [KingColumn]
        public string Parameter10 { get; set; }

        [KingColumn]
        public string Parameter2 { get; set; }

        [KingColumn]
        public string Parameter3 { get; set; }

        [KingColumn]
        public string Parameter4 { get; set; }

        [KingColumn]
        public string Parameter5 { get; set; }

        [KingColumn]
        public string Parameter6 { get; set; }

        [KingColumn]
        public string Parameter7 { get; set; }

        [KingColumn]
        public string Parameter8 { get; set; }

        [KingColumn]
        public string Parameter9 { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public int ServiceCallId { get; set; }

        [KingColumn]
        public string ServiceFlag { get; set; }

        [KingColumn]
        public int? State { get; set; }
    }
}

