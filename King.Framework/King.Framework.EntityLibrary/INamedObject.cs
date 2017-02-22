namespace King.Framework.EntityLibrary
{
    using System;

    public interface INamedObject
    {
        string DisplayText { get; set; }

        long Id { get; set; }

        string Name { get; set; }
    }
}

