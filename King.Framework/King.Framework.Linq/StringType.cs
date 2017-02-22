namespace King.Framework.Linq
{
    using System;

    internal class StringType : ISharpType
    {
        public string FormatAsSql(object value)
        {
            return string.Format(" '{0}' ", value.ToString().Replace("'", "''"));
        }
    }
}
