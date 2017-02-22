namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [KingTable]
    public class SysTransitionInstance
    {
        [XmlIgnore]
        public virtual SysActivityInstance PostActivityInstance { get; set; }

        [KingColumn]
        public virtual int? PostActivityInstanceId { get; set; }

        [XmlIgnore]
        public virtual SysActivityInstance PreActivityInstance { get; set; }

        [KingColumn]
        public virtual int? PreActivityInstanceId { get; set; }

        [KingColumn]
        public virtual int? ProcessInstanceId { get; set; }

        [XmlIgnore]
        public virtual SysTransition Transition { get; set; }

        [KingColumn]
        public virtual long? TransitionId { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public virtual int TransitionInstanceId { get; set; }
    }
}

