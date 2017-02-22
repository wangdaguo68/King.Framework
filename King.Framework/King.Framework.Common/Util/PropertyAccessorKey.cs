namespace King.Framework.Common.Util
{
    using System;
    using System.Runtime.CompilerServices;

    public class PropertyAccessorKey
    {
        public PropertyAccessorKey(Type targetType, string propertyName)
        {
            Guard.ArgumentNotNullOrEmpty(targetType, "targetType");
            Guard.ArgumentNotNullOrEmpty(propertyName, "propertyName");
            this.TargetType = targetType;
            this.PropertyName = propertyName;
        }

        public override bool Equals(object obj)
        {
            return (this.GetHashCode() == obj.GetHashCode());
        }

        public override int GetHashCode()
        {
            return (this.TargetType.GetHashCode() ^ this.PropertyName.GetHashCode());
        }

        public string PropertyName { get; private set; }

        public Type TargetType { get; private set; }
    }
}
