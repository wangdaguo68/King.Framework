namespace King.Framework.EntityLibrary
{
    using System;
    using System.Collections.Generic;

    public static class DataTypeExtend
    {
        private static readonly List<DataTypeEnum> _list = new List<DataTypeEnum>(new DataTypeEnum[] { DataTypeEnum.pbool, DataTypeEnum.pdatetime, DataTypeEnum.pdecimal, DataTypeEnum.penum, DataTypeEnum.pfloat, DataTypeEnum.pint, DataTypeEnum.plong, DataTypeEnum.pstring, DataTypeEnum.ptext });

        public static List<DataTypeEnum> RowEditableDataTypes
        {
            get
            {
                return _list;
            }
        }
    }
}

