namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [KingTable]
    public class SysPathRuleItem
    {
        [KingColumn]
        public int ItemType { get; set; }

        [KingColumn]
        public int ObjectId { get; set; }

        [KingColumn]
        public int PathRuleId { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public int PathRuleItemId { get; set; }
    }
}

