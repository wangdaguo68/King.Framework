namespace King.Framework.Linq.Data.Mapping
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple=true)]
    public class AssociationAttribute : MemberAttribute
    {
        public bool IsForeignKey { get; set; }

        public string KeyMembers { get; set; }

        public string Name { get; set; }

        public string RelatedEntityID { get; set; }

        public Type RelatedEntityType { get; set; }

        public string RelatedKeyMembers { get; set; }
    }
}
