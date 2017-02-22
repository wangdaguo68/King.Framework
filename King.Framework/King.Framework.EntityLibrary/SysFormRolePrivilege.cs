namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [KingTable]
    public class SysFormRolePrivilege
    {
        [KingColumn]
        public virtual DateTime? CreateTime { get; set; }

        [KingColumn]
        public virtual int? CreateUserId { get; set; }

        [KingColumn]
        public virtual int? DisplayPrivilege { get; set; }

        [XmlIgnore]
        public virtual SysForm Form { get; set; }

        [XmlIgnore]
        public virtual SysFormField FormField { get; set; }

        [KingColumn]
        public virtual long? FormFieldId { get; set; }

        [KingColumn]
        public virtual long? FormId { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public virtual long PrivilegeId { get; set; }

        [KingColumn]
        public virtual int? RoleId { get; set; }
    }
}

