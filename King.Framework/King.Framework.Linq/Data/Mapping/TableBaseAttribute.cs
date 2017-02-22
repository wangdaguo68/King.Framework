namespace King.Framework.Linq.Data.Mapping
{
    using System;
    using System.Runtime.CompilerServices;

    public abstract class TableBaseAttribute : MappingAttribute
    {
        protected TableBaseAttribute()
        {
        }

        public string Alias { get; set; }

        public string Name { get; set; }
    }
}
