namespace King.Framework.Manager
{
    using King.Framework.Common;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class ViewField
    {
        public override bool Equals(object obj)
        {
            return (base.GetHashCode() == obj.GetHashCode());
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("Name:{0} FormatString:{1} FormatType:{2}", this.Name, this.FormatString, this.FormatType);
        }

        public string FormatString { get; set; }

        public King.Framework.Common.FormatType FormatType { get; set; }

        public string Name { get; set; }

        public class ViewFieldComparer<TSource> : IEqualityComparer<TSource> where TSource: class
        {
            public bool Equals(TSource x, TSource y)
            {
                return (x.GetHashCode() == y.GetHashCode());
            }

            public int GetHashCode(TSource obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
