namespace King.Framework.Linq
{
    using System;

    internal interface ISharpType
    {
        string FormatAsSql(object value);
    }
}
