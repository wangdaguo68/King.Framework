namespace King.Framework.Common.ServiceContract
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class WfRelation
    {
        public IEnumerable<WfField> FieldList { get; set; }

        public long ParentEntityId { get; set; }

        public string ParentEntityName { get; set; }

        public long RelationId { get; set; }

        public string RelationName { get; set; }
    }
}
