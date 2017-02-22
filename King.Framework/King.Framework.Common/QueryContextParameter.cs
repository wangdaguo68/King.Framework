namespace King.Framework.Common
{
    using King.Framework.EntityLibrary;
    using System;
    using System.Runtime.CompilerServices;

    [Serializable]
    public class QueryContextParameter
    {
        public T_User User { get; set; }
    }
}
