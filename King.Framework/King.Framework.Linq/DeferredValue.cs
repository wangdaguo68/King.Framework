namespace King.Framework.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct DeferredValue<T> : IDeferLoadable
    {
        private IEnumerable<T> source;
        private bool loaded;
        private T value;
        public DeferredValue(T value)
        {
            this.value = value;
            this.source = null;
            this.loaded = true;
        }

        public DeferredValue(IEnumerable<T> source)
        {
            this.source = source;
            this.loaded = false;
            this.value = default(T);
        }

        public void Load()
        {
            if (this.source != null)
            {
                this.value = this.source.SingleOrDefault<T>();
                this.loaded = true;
            }
        }

        public bool IsLoaded
        {
            get
            {
                return this.loaded;
            }
        }
        public bool IsAssigned
        {
            get
            {
                return (this.loaded && (this.source == null));
            }
        }
        private void Check()
        {
            if (!this.IsLoaded)
            {
                this.Load();
            }
        }

        public T Value
        {
            get
            {
                this.Check();
                return this.value;
            }
            set
            {
                this.value = value;
                this.loaded = true;
                this.source = null;
            }
        }
    }
}
