namespace King.Framework.Linq
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class CompoundKey : IEquatable<CompoundKey>, IEnumerable<object>, IEnumerable
    {
        private int hc;
        private object[] values;

        public CompoundKey(params object[] values)
        {
            this.values = values;
            int index = 0;
            int length = values.Length;
            while (index < length)
            {
                object obj2 = values[index];
                if (obj2 != null)
                {
                    this.hc ^= obj2.GetHashCode() + index;
                }
                index++;
            }
        }

        public bool Equals(CompoundKey other)
        {
            if ((other == null) || (other.values.Length != this.values.Length))
            {
                return false;
            }
            int index = 0;
            int length = other.values.Length;
            while (index < length)
            {
                if (!object.Equals(this.values[index], other.values[index]))
                {
                    return false;
                }
                index++;
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public IEnumerator<object> GetEnumerator()
        {
            return ((IEnumerable<object>) this.values).GetEnumerator();
        }

        public override int GetHashCode()
        {
            return this.hc;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
