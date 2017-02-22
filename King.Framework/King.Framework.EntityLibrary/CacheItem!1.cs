namespace King.Framework.EntityLibrary
{
    using System;

    public class CacheItem<T>
    {
        public readonly T CacheObject;
        public readonly DateTime ExpireTime;

        public CacheItem(T obj, DateTime expireTime)
        {
            this.CacheObject = obj;
            this.ExpireTime = expireTime;
        }
    }
}

