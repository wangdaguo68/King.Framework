namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [Serializable, KingTable]
    public class SysService
    {
        [KingColumn]
        public string AshxAddr { get; set; }

        [KingColumn]
        public string AshxParameter { get; set; }

        [KingColumn]
        public string CallBackAshxAddr { get; set; }

        [KingColumn]
        public string CallBackAshxParameter { get; set; }

        [KingColumn]
        public string ClassName { get; set; }

        [KingColumn]
        public string DLLName { get; set; }

        [KingColumn]
        public int? ErrorTimes { get; set; }

        [KingColumn]
        public string NameSpace { get; set; }

        [KingColumn]
        public string OwnerApp { get; set; }

        [KingColumn]
        public string ProcessName { get; set; }

        [KingColumn]
        public string ServiceDesc { get; set; }

        [KingColumn]
        public string ServiceFlag { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public int ServiceId { get; set; }

        [KingColumn]
        public string ServicePath { get; set; }

        [KingColumn]
        public int? ServicePriority { get; set; }

        [KingColumn]
        public int? ServiceType { get; set; }

        [KingColumn]
        public int? State { get; set; }
    }
}

