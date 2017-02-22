namespace King.Framework.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class ScopedDictionary<TKey, TValue>
    {
        private Dictionary<TKey, TValue> map;
        private ScopedDictionary<TKey, TValue> previous;

        public ScopedDictionary(ScopedDictionary<TKey, TValue> previous)
        {
            this.previous = previous;
            this.map = new Dictionary<TKey, TValue>();
        }

        public ScopedDictionary(ScopedDictionary<TKey, TValue> previous, IEnumerable<KeyValuePair<TKey, TValue>> pairs) : this(previous)
        {
            foreach (KeyValuePair<TKey, TValue> pair in pairs)
            {
                this.map.Add(pair.Key, pair.Value);
            }
        }

        public void Add(TKey key, TValue value)
        {
            this.map.Add(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            for (ScopedDictionary<TKey, TValue> dictionary = (ScopedDictionary<TKey, TValue>) this; dictionary != null; dictionary = dictionary.previous)
            {
                if (dictionary.map.ContainsKey(key))
                {
                    return true;
                }
            }
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            for (ScopedDictionary<TKey, TValue> dictionary = (ScopedDictionary<TKey, TValue>) this; dictionary != null; dictionary = dictionary.previous)
            {
                if (dictionary.map.TryGetValue(key, out value))
                {
                    return true;
                }
            }
            value = default(TValue);
            return false;
        }
    }
}
