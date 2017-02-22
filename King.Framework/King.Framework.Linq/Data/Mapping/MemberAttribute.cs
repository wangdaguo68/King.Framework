namespace King.Framework.Linq.Data.Mapping
{
    using System;
    using System.Runtime.CompilerServices;

    public abstract class MemberAttribute : MappingAttribute
    {
        protected MemberAttribute()
        {
        }

        public string Member { get; set; }
    }
}
