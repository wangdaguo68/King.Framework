namespace King.Framework.DAL
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Class)]
    public class KingTableAttribute : Attribute
    {
        public KingTableAttribute()
        {
        }

        public KingTableAttribute(string name)
        {
            this.Name = name;
        }

        public bool IsInherited { get; set; }

        public string Name { get; set; }

        public string Schema { get; set; }
    }
}
