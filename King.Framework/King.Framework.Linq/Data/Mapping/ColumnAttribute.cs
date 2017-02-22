namespace King.Framework.Linq.Data.Mapping
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple=true)]
    public class ColumnAttribute : MemberAttribute
    {
        public string Alias { get; set; }

        public string DbType { get; set; }

        public bool IsComputed { get; set; }

        public bool IsGenerated { get; set; }

        public bool IsPrimaryKey { get; set; }

        public string Name { get; set; }
    }
}
