namespace King.Framework.Common.ServiceContract
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class WfEntity
    {
        public long EntityId { get; set; }

        public string EntityName { get; set; }

        public IEnumerable<WfField> FieldList { get; set; }

        public IEnumerable<WfPage> PageList { get; set; }

        public IEnumerable<WfRelation> RelationList { get; set; }
    }
}
