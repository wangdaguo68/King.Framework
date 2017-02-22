namespace King.Framework.Linq.Data.Common
{
    using System;

    public enum JoinType
    {
        CrossJoin,
        InnerJoin,
        CrossApply,
        OuterApply,
        LeftOuter,
        SingletonLeftOuter
    }
}
