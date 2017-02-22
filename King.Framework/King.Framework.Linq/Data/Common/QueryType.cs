namespace King.Framework.Linq.Data.Common
{
    using System;

    public abstract class QueryType
    {
        protected QueryType()
        {
        }

        public abstract int Length { get; }

        public abstract bool NotNull { get; }

        public abstract short Precision { get; }

        public abstract short Scale { get; }
    }
}
