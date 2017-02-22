namespace King.Framework.Linq
{
    using System;

    public interface IDeferLoadable
    {
        void Load();

        bool IsLoaded { get; }
    }
}
