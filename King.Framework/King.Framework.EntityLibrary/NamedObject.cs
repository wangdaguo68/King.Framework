namespace King.Framework.EntityLibrary
{
    using System;
    using System.Runtime.CompilerServices;

    public class NamedObject
    {
        public NamedObject()
        {
        }

        public NamedObject(INamedObject obj)
        {
            this.Id = obj.Id;
            this.Name = obj.Name;
            this.DisplayText = obj.DisplayText;
        }

        public string DisplayText { get; set; }

        public long Id { get; set; }

        public string Name { get; set; }
    }
}

