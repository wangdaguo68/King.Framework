namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [KingTable]
    public class T_Configuration_Group
    {
        [KingColumn(IsPrimaryKey=true)]
        public int Configuration_Group_Id { get; set; }

        [KingColumn(MaxLength=0xff)]
        public string Configuration_Group_Title { get; set; }

        [KingColumn(MaxLength=-1)]
        public string Description { get; set; }

        [KingColumn]
        public int Is_Visible { get; set; }

        [KingColumn]
        public int Sort_Order { get; set; }

        [KingColumn]
        public int? State { get; set; }
    }
}

