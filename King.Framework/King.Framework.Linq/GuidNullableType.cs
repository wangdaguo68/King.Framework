namespace King.Framework.Linq
{
    using System;

    internal class GuidNullableType : ISharpType
    {
        public string FormatAsSql(object value)
        {
            return string.Format("'{0}'", value);
        }
    }
}
