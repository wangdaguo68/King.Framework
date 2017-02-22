namespace King.Framework.Mvc
{
    using System;

    public interface ITreeData
    {
        string iconSkin { get; set; }

        int id { get; set; }

        string name { get; set; }

        bool open { get; set; }

        int pId { get; set; }
    }
}

