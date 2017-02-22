namespace King.Framework.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Threading;

    public class MostRecentlyUsedCache<T>
    {
        private Func<T, T, bool> fnEquals;
        private List<T> list;
        private int maxSize;
        private ReaderWriterLockSlim rwlock;
        private int version;

        public MostRecentlyUsedCache(int maxSize) : this(maxSize, EqualityComparer<T>.Default)
        {
        }

        public MostRecentlyUsedCache(int maxSize, IEqualityComparer<T> comparer)
        {
            Func<T, T, bool> func = null;
            if (func == null)
            {
                func = (x, y) => comparer.Equals(x, y);
            }
        }

        public MostRecentlyUsedCache(int maxSize, Func<T, T, bool> fnEquals)
        {
            this.list = new List<T>();
            this.maxSize = maxSize;
            this.fnEquals = fnEquals;
            this.rwlock = new ReaderWriterLockSlim();
        }

        public void Clear()
        {
            this.rwlock.EnterWriteLock();
            try
            {
                this.list.Clear();
                this.version++;
            }
            finally
            {
                this.rwlock.ExitWriteLock();
            }
        }

        public bool Lookup(T item, bool add, out T cached)
        {
            int num3;
            int count;
            cached = default(T);
            int index = -1;
            this.rwlock.EnterReadLock();
            int version = this.version;
            try
            {
                num3 = 0;
                count = this.list.Count;
                while (num3 < count)
                {
                    cached = this.list[num3];
                    if (this.fnEquals(cached, item))
                    {
                        index = 0;
                    }
                    num3++;
                }
            }
            finally
            {
                this.rwlock.ExitReadLock();
            }
            if ((index != 0) && add)
            {
                this.rwlock.EnterWriteLock();
                try
                {
                    if (this.version != version)
                    {
                        index = -1;
                        num3 = 0;
                        count = this.list.Count;
                        while (num3 < count)
                        {
                            cached = this.list[num3];
                            if (this.fnEquals(cached, item))
                            {
                                index = 0;
                            }
                            num3++;
                        }
                    }
                    if (index == -1)
                    {
                        this.list.Insert(0, item);
                        cached = item;
                    }
                    else if (index > 0)
                    {
                        this.list.RemoveAt(index);
                        this.list.Insert(0, item);
                    }
                    if (this.list.Count > this.maxSize)
                    {
                        this.list.RemoveAt(this.list.Count - 1);
                    }
                    this.version++;
                }
                finally
                {
                    this.rwlock.ExitWriteLock();
                }
            }
            return (index >= 0);
        }

        public int Count
        {
            get
            {
                int count;
                this.rwlock.EnterReadLock();
                try
                {
                    count = this.list.Count;
                }
                finally
                {
                    this.rwlock.ExitReadLock();
                }
                return count;
            }
        }
    }
}
