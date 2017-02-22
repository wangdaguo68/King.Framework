namespace King.Framework.Linq.Data.Common
{
    using System;

    public abstract class MappingEntity
    {
        protected MappingEntity()
        {
        }

        public abstract Type ElementType { get; }

        public abstract Type EntityType { get; }

        public abstract string TableId { get; }
    }
}
