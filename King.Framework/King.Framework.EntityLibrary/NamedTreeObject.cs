namespace King.Framework.EntityLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class NamedTreeObject : NamedObject
    {
        private List<NamedTreeObject> _children;

        public NamedTreeObject()
        {
        }

        public NamedTreeObject(INamedObject obj) : base(obj)
        {
        }

        public List<NamedTreeObject> Children
        {
            get
            {
                if (this._children == null)
                {
                    this._children = new List<NamedTreeObject>();
                }
                return this._children;
            }
        }

        public long ParentId { get; set; }
    }
}

