namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [KingTable("T_User_Role")]
    public class T_User_Role
    {
        [KingColumn]
        public DateTime? CreateTime { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public int Id { get; set; }

        [KingColumn]
        public int Role_Id { get; set; }

        [KingColumn]
        public int? State { get; set; }

        [KingColumn]
        public DateTime? UpdateTime { get; set; }

        [KingColumn]
        public int User_Id { get; set; }
    }
}

