namespace King.Framework.Linq
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class Grouping<TKey, TElement> : IGrouping<TKey, TElement>, IEnumerable<TElement>, IEnumerable
    {
        private IEnumerable<TElement> group;
        private TKey key;

        public Grouping(TKey key, IEnumerable<TElement> group)
        {
            this.key = key;
            this.group = group;
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            if (!(this.group is List<TElement>))
            {
                this.group = this.group.ToList<TElement>();
            }
            return this.group.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.group.GetEnumerator();
        }

        public TKey Key
        {
            get
            {
                return this.key;
            }
        }
    }
}
