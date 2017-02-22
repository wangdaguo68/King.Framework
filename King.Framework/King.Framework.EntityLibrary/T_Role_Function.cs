namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [Serializable, KingTable]
    public class T_Role_Function
    {
        [KingColumn]
        public DateTime? CreateTime { get; set; }

        [KingColumn]
        public long? CreateUserID { get; set; }

        [KingColumn]
        public long? Function_ID { get; set; }

        [KingColumn]
        public long? OwnerId { get; set; }

        [KingColumn(IsPrimaryKey=true, KeyOrder=1)]
        public long? Role_Function_ID { get; set; }

        [KingColumn]
        public long? Role_ID { get; set; }

        [KingColumn]
        public int? State { get; set; }

        [KingColumn]
        public DateTime? UpdateTime { get; set; }

        [KingColumn]
        public long? UpdateUserID { get; set; }
    }
}

