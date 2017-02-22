namespace King.Framework.Linq.Data.Mapping
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple=true)]
    public class ExtensionTableAttribute : TableBaseAttribute
    {
        public string KeyColumns { get; set; }

        public string RelatedAlias { get; set; }

        public string RelatedKeyColumns { get; set; }
    }
}
