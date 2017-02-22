namespace King.Framework.Mvc
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class SelectControlData
    {
        public string icon { get; set; }

        public string iconSkin { get; set; }

        public int id { get; set; }

        public string name { get; set; }

        public bool open { get; set; }

        public int? pId { get; set; }

        public List<NameValues> userlist { get; set; }
    }
}

