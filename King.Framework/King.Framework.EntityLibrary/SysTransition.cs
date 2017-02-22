namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [KingTable]
    public class SysTransition
    {
        [KingColumn]
        public virtual int? Direction { get; set; }

        [KingColumn(MaxLength=-1)]
        public virtual string DisplayText { get; set; }

        [XmlIgnore]
        public virtual SysExpression Expression { get; set; }

        [KingColumn]
        public virtual long? ExpressionId { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysExpression> Expressions { get; set; }

        [XmlIgnore]
        public virtual SysActivity PostActivity { get; set; }

        [KingColumn]
        public virtual long? PostActivityId { get; set; }

        [XmlIgnore]
        public virtual SysActivity PreActivity { get; set; }

        [KingColumn]
        public virtual long? PreActivityId { get; set; }

        [XmlIgnore]
        public virtual SysProcess Process { get; set; }

        [KingColumn]
        public virtual long? ProcessId { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public virtual long TransitionId { get; set; }

        [XmlIgnore]
        public virtual ICollection<SysTransitionInstance> TransitionInstances { get; set; }

        [KingColumn(MaxLength=200)]
        public virtual string WwfTransitionId { get; set; }
    }
}

