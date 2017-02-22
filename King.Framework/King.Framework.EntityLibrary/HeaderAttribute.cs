namespace King.Framework.EntityLibrary
{
    using System;
    using System.Runtime.CompilerServices;

    public class HeaderAttribute : Attribute
    {
        public string PrimaryKey { get; set; }

        public string XmlRootName { get; set; }
    }
}

