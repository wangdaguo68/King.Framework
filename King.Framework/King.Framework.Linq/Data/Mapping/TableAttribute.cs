namespace King.Framework.Linq.Data.Mapping
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple=false)]
    public class TableAttribute : TableBaseAttribute
    {
        public Type EntityType { get; set; }
    }
}
