namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [Serializable, KingTable]
    public class T_Company
    {
        [KingColumn]
        public string Company_Address { get; set; }

        [KingColumn(MaxLength=100)]
        public string Company_Code { get; set; }

        [KingColumn(IsPrimaryKey=true, KeyOrder=1)]
        public int Company_Id { get; set; }

        [KingColumn(MaxLength=100)]
        public string Company_Name { get; set; }

        [KingColumn]
        public int? Company_Type { get; set; }
    }
}

