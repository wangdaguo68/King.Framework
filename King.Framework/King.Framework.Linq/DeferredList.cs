namespace King.Framework.Linq
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;

    public class DeferredList<T> : IDeferredList<T>, IList<T>, IDeferredList, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable, IDeferLoadable
    {
        private IEnumerable<T> source;
        private List<T> values;

        public DeferredList(IEnumerable<T> source)
        {
            this.source = source;
        }

        public int Add(object value)
        {
            this.Check();
            this.values.Add((T)value);
            return 1;
        }

        public void Add(T item)
        {
            this.Check();
            this.values.Add(item);
        }

        private void Check()
        {
            if (!this.IsLoaded)
            {
                this.Load();
            }
        }

        public void Clear()
        {
            this.Check();
            this.values.Clear();
        }

        public bool Contains(T item)
        {
            this.Check();
            return this.values.Contains(item);
        }

        public bool Contains(object value)
        {
            this.Check();
            return this.values.Contains((T)value);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.Check();
            this.values.CopyTo(array, arrayIndex);
        }

        public void CopyTo(Array array, int index)
        {
            this.Check();
            this.values.CopyTo((T[])array, index);
        }

        public IEnumerator<T> GetEnumerator()
        {
            this.Check();
            return this.values.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            this.Check();
            return this.values.IndexOf(item);
        }

        public int IndexOf(object value)
        {
            this.Check();
            return this.values.IndexOf((T)value);
        }

        public void Insert(int index, T item)
        {
            this.Check();
            this.values.Insert(index, item);
        }

        public void Insert(int index, object value)
        {
            this.Check();
            this.values.Insert(index,(T) value);
        }

        public void Load()
        {
            this.values = new List<T>(this.source);
        }

        public bool Remove(T item)
        {
            this.Check();
            return this.values.Remove(item);
        }

        public void Remove(object value)
        {
            this.Check();
            this.values.Remove((T)value);
        }

        public void RemoveAt(int index)
        {
            this.Check();
            this.values.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public int Count
        {
            get
            {
                this.Check();
                return this.values.Count;
            }
        }

        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        public bool IsLoaded
        {
            get
            {
                return (this.values != null);
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        public T this[int index]
        {
            get
            {
                this.Check();
                return this.values[index];
            }
            set
            {
                this.Check();
                this.values[index] = value;
            }
        }

        public object SyncRoot
        {
            get
            {
                return null;
            }
        }

        object IList.this[int index]
        {
            get
            {
                this.Check();
                return ((IList) this.values)[index];
            }
            set
            {
                this.Check();
                ((IList) this.values)[index] = value;
            }
        }
    }
}
