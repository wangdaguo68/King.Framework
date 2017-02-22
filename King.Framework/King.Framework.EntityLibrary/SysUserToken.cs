namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [KingTable]
    public class SysUserToken
    {
        [KingColumn]
        public DateTime ExpireTime { get; set; }

        [KingColumn(IsPrimaryKey=true, MaxLength=80)]
        public string TokenKey { get; set; }

        [KingColumn]
        public int UserId { get; set; }
    }
}

