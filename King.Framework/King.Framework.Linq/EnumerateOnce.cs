namespace King.Framework.Linq
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;

    public class EnumerateOnce<T> : IEnumerable<T>, IEnumerable
    {
        private IEnumerable<T> enumerable;

        public EnumerateOnce(IEnumerable<T> enumerable)
        {
            this.enumerable = enumerable;
        }

        public IEnumerator<T> GetEnumerator()
        {
            IEnumerable<T> enumerable = Interlocked.Exchange<IEnumerable<T>>(ref this.enumerable, null);
            if (enumerable == null)
            {
                throw new Exception("Enumerated more than once.");
            }
            return enumerable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
