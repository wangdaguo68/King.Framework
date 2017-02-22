namespace King.Framework.DAL
{
    using System;

    public enum KingOracleDbType
    {
        Array = 0x80,
        BFile = 0x65,
        BinaryDouble = 0x84,
        BinaryFloat = 0x85,
        Blob = 0x66,
        Byte = 0x67,
        Char = 0x68,
        Clob = 0x69,
        Date = 0x6a,
        Decimal = 0x6b,
        Double = 0x6c,
        Int16 = 0x6f,
        Int32 = 0x70,
        Int64 = 0x71,
        IntervalDS = 0x72,
        IntervalYM = 0x73,
        Long = 0x6d,
        LongRaw = 110,
        NChar = 0x75,
        NClob = 0x74,
        NVarchar2 = 0x77,
        Object = 0x81,
        Raw = 120,
        Ref = 130,
        RefCursor = 0x79,
        Single = 0x7a,
        TimeStamp = 0x7b,
        TimeStampLTZ = 0x7c,
        TimeStampTZ = 0x7d,
        Varchar2 = 0x7e,
        XmlType = 0x7f
    }
}
