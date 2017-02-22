namespace King.Framework.Common.Util
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static class Guard
    {
        public static void ArgumentNotNullOrEmpty(object value, string name)
        {
            if (null == value)
            {
                throw new ArgumentNullException(name);
            }
            if ((value is string) && string.Empty.Equals(value))
            {
                throw new ArgumentNullException(name);
            }
        }

        public static void SureNotNull<T>(this T? value, string errorMessage = null) where T: struct
        {
            if (!value.HasValue)
            {
                throw new ArgumentNullException(errorMessage);
            }
        }

        public static void SureNotNullOrEmpty(this string value, string name = null)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(name);
            }
        }
    }
}
