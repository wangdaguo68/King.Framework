namespace King.Framework.Mvc
{
    using System;
    using System.Runtime.CompilerServices;

    public class TreeData : ITreeData
    {
        public string iconSkin { get; set; }

        public int id { get; set; }

        public string name { get; set; }

        public bool open { get; set; }

        public int pId { get; set; }
    }
}

