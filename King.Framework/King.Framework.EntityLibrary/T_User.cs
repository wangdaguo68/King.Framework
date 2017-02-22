namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using King.Framework.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    [Serializable, KingTable]
    public class T_User : IUser
    {
        public override bool Equals(object obj)
        {
            return (this.GetHashCode() == obj.GetHashCode());
        }

        public override int GetHashCode()
        {
            return (base.GetType().GetHashCode() ^ this.User_ID.GetHashCode());
        }

        public override string ToString()
        {
            return (this.User_Name ?? "");
        }

        public int? _mobileState { get; set; }

        [KingColumn]
        public string Address { get; set; }

        [KingColumn]
        public string Address_En { get; set; }

        [KingColumn]
        public string Card_No { get; set; }

        [KingColumn]
        public int? CompanyID { get; set; }

        [KingColumn]
        public int? Department_ID { get; set; }

        [KingColumn]
        public string EnglishName { get; set; }

        [KingColumn(MaxLength=50)]
        public string LoginName { get; set; }

        [KingColumn]
        public string NameSpell { get; set; }

        [KingColumn]
        public int? OrderIndex { get; set; }

        [KingColumn]
        public string Pinyin_Abbr { get; set; }

        [KingColumn]
        public string Pinyin_Code { get; set; }

        [KingColumn]
        public string Position { get; set; }

        [KingColumn]
        public string Position_En { get; set; }

        [KingColumn]
        public string SpellInitial { get; set; }

        [KingColumn(ColumnName="State")]
        public int? State { get; set; }

        public virtual ICollection<T_Role> T_Roles { get; set; }

        [KingColumn]
        public int? TableVersion { get; set; }

        [KingColumn]
        public DateTime? UpdateTime { get; set; }

        [KingColumn(MaxLength=50)]
        public string User_Code { get; set; }

        [KingColumn(MaxLength=0x80)]
        public string User_EMail { get; set; }

        [KingColumn(IsPrimaryKey=true, KeyOrder=1)]
        public int User_ID { get; set; }

        [KingColumn(MaxLength=50, IsPrimaryKey=false)]
        public string User_Mobile { get; set; }

        [KingColumn(MaxLength=50)]
        public string User_Name { get; set; }

        [KingColumn]
        public string User_OfficePhone { get; set; }

        [KingColumn(MaxLength=0x80)]
        public string User_Password { get; set; }

        [KingColumn(ColumnName="User_Status")]
        public int? User_Status { get; set; }
    }
}

