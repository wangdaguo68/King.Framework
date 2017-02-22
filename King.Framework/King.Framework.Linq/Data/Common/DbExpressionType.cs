namespace King.Framework.Linq.Data.Common
{
    using System;

    public enum DbExpressionType
    {
        Aggregate = 0x3ef,
        AggregateSubquery = 0x3f4,
        Batch = 0x3fd,
        Between = 0x3f6,
        Block = 0x3ff,
        ClientJoin = 0x3e9,
        Column = 0x3ea,
        Declaration = 0x401,
        Delete = 0x3fc,
        Entity = 0x3ed,
        Exists = 0x3f1,
        Function = 0x3fe,
        Grouping = 0x3f3,
        If = 0x400,
        In = 0x3f2,
        Insert = 0x3fa,
        IsNull = 0x3f5,
        Join = 0x3ee,
        NamedValue = 0x3f8,
        OuterJoined = 0x3f9,
        Projection = 0x3ec,
        RowCount = 0x3f7,
        Scalar = 0x3f0,
        Select = 0x3eb,
        Table = 0x3e8,
        Update = 0x3fb,
        Variable = 0x402
    }
}
