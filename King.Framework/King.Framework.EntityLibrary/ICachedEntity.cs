namespace King.Framework.EntityLibrary
{
    using System;

    public interface ICachedEntity
    {
        void InitNavigationList();
        void SetContext(CacheContext context);

        CacheContext MetaCache { get; }

        long PrimaryKeyValue { get; }
    }
}

