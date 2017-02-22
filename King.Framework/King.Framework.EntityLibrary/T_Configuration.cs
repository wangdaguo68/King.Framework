namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [Serializable, KingTable]
    public class T_Configuration
    {
        [KingColumn(MaxLength=-1)]
        public string Configuration_Description { get; set; }

        [KingColumn]
        public int Configuration_GroupId { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public int Configuration_Id { get; set; }

        [KingColumn(MaxLength=0xff)]
        public string Configuration_Key { get; set; }

        [KingColumn(MaxLength=0xff)]
        public string Configuration_Title { get; set; }

        [KingColumn(MaxLength=-1)]
        public string Configuration_Value { get; set; }

        [KingColumn]
        public DateTime Date_Added { get; set; }

        [KingColumn]
        public DateTime Last_Modified { get; set; }

        [KingColumn]
        public int Sort_Order { get; set; }

        [KingColumn]
        public int? State { get; set; }

        public Configuration_StateEnum? StateEnum { get; set; }
    }
}

