namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using King.Framework.Interfaces;
    using System;
    using System.Runtime.CompilerServices;

    [Serializable, KingTable]
    public class T_Department : IDepartment
    {
        public override bool Equals(object obj)
        {
            return (this.GetHashCode() == obj.GetHashCode());
        }

        public override int GetHashCode()
        {
            return (base.GetType().GetHashCode() ^ this.Department_ID.GetHashCode());
        }

        public override string ToString()
        {
            return (this.Department_Name ?? "");
        }

        [KingColumn(MaxLength=20)]
        public string Department_Code { get; set; }

        [KingColumn(IsPrimaryKey=true, KeyOrder=1)]
        public int Department_ID { get; set; }

        [KingColumn(MaxLength=50)]
        public string Department_Name { get; set; }

        [KingColumn]
        public int? Manager_ID { get; set; }

        [KingColumn]
        public int? OrderIndex { get; set; }

        [KingColumn]
        public int? Parent_ID { get; set; }

        [KingColumn]
        public string Remark { get; set; }

        [KingColumn]
        public int? State { get; set; }

        [KingColumn(MaxLength=100)]
        public string SystemLevelCode { get; set; }

        [KingColumn]
        public int? TableVersion { get; set; }
    }
}

