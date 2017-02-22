namespace King.Framework.AspNet.Identity
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [Serializable, KingTable]
    internal class T_Company_AppDb
    {
        [KingColumn(IsPrimaryKey=true)]
        public int Appdb_Id { get; set; }

        [KingColumn]
        public int? AppId { get; set; }

        [KingColumn]
        public int? CompanyId { get; set; }

        [KingColumn]
        public string DbConnString { get; set; }

        [KingColumn]
        public string DbPrivider { get; set; }
    }
}

