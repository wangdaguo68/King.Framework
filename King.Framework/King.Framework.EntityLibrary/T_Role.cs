namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using King.Framework.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    [Serializable, KingTable]
    public class T_Role : IRole
    {
        public override bool Equals(object obj)
        {
            return (this.GetHashCode() == obj.GetHashCode());
        }

        public override int GetHashCode()
        {
            return (base.GetType().GetHashCode() ^ this.Role_ID.GetHashCode());
        }

        public override string ToString()
        {
            return (this.Role_Name ?? "");
        }

        [KingColumn]
        public bool? IsBuiltin { get; set; }

        [KingColumn(MaxLength=0x7d0)]
        public string Role_Comment { get; set; }

        [KingColumn(IsPrimaryKey=true, KeyOrder=1)]
        public int Role_ID { get; set; }

        [KingColumn(MaxLength=20)]
        public string Role_Name { get; set; }

        public int Role_Status { get; set; }

        [KingColumn]
        public int? State { get; set; }

        public virtual ICollection<T_User> T_Users { get; set; }
    }
}

