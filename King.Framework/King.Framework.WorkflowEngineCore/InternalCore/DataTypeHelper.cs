namespace King.Framework.WorkflowEngineCore.InternalCore
{
    using King.Framework.EntityLibrary;
    using System;
    using System.Collections.Generic;

    internal static class DataTypeHelper
    {
        private static readonly Dictionary<int, BaseDataTypeHelper> _helpers = new Dictionary<int, BaseDataTypeHelper>(30);

        static DataTypeHelper()
        {
            Add(new KeyHelper());
            Add(new IntHelper());
            Add(new StringHelper());
            Add(new DateTimeHelper());
            Add(new DecimalHelper());
            Add(new FloatHelper());
            Add(new TextHelper());
            Add(new LongHelper());
            Add(new BinaryHelper());
            Add(new FileHelper());
            Add(new BoolHelper());
            Add(new RefHelper());
            Add(new EnumHelper());
            Add(new PasswordHelper());
        }

        private static void Add(BaseDataTypeHelper helper)
        {
            _helpers[(int) helper.DataType] = helper;
        }

        public static BaseDataTypeHelper GetHelper(DataTypeEnum data_type)
        {
            if (_helpers.ContainsKey((int) data_type))
            {
                return _helpers[(int) data_type];
            }
            return null;
        }

        private class BinaryHelper : BaseDataTypeHelper
        {
            public BinaryHelper() : base(DataTypeEnum.pbinary)
            {
            }

            public override object ConvertFromString(string s)
            {
                return Convert.FromBase64String(s);
            }

            public override Type TargetType
            {
                get
                {
                    return typeof(byte[]);
                }
            }
        }

        private class BoolHelper : BaseDataTypeHelper
        {
            public BoolHelper() : base(DataTypeEnum.pbool)
            {
            }

            public override object ConvertFromString(string s)
            {
                return Convert.ToBoolean(s);
            }

            public override Type TargetType
            {
                get
                {
                    return typeof(bool);
                }
            }
        }

        private class DateTimeHelper : BaseDataTypeHelper
        {
            public DateTimeHelper() : base(DataTypeEnum.pdatetime)
            {
            }

            public override object ConvertFromString(string s)
            {
                return Convert.ToDateTime(s);
            }

            public override Type TargetType
            {
                get
                {
                    return typeof(DateTime);
                }
            }
        }

        private class DecimalHelper : BaseDataTypeHelper
        {
            public DecimalHelper() : base(DataTypeEnum.pdecimal)
            {
            }

            public override object ConvertFromString(string s)
            {
                return Convert.ToDecimal(s);
            }

            public override Type TargetType
            {
                get
                {
                    return typeof(decimal);
                }
            }
        }

        private class EnumHelper : BaseDataTypeHelper
        {
            public EnumHelper() : base(DataTypeEnum.penum)
            {
            }

            public override object ConvertFromString(string s)
            {
                return Convert.ToInt32(s);
            }

            public override Type TargetType
            {
                get
                {
                    return typeof(int);
                }
            }
        }

        private class FileHelper : BaseDataTypeHelper
        {
            public FileHelper() : base(DataTypeEnum.pfile)
            {
            }

            public override object ConvertFromString(string s)
            {
                return Convert.ToInt32(s);
            }

            public override Type TargetType
            {
                get
                {
                    return typeof(int);
                }
            }
        }

        private class FloatHelper : BaseDataTypeHelper
        {
            public FloatHelper() : base(DataTypeEnum.pfloat)
            {
            }

            public override object ConvertFromString(string s)
            {
                return Convert.ToDouble(s);
            }

            public override Type TargetType
            {
                get
                {
                    return typeof(double);
                }
            }
        }

        private class IntHelper : BaseDataTypeHelper
        {
            public IntHelper() : base(DataTypeEnum.pint)
            {
            }

            public override object ConvertFromString(string s)
            {
                return Convert.ToInt32(s);
            }

            public override Type TargetType
            {
                get
                {
                    return typeof(int);
                }
            }
        }

        private class KeyHelper : BaseDataTypeHelper
        {
            public KeyHelper() : base(DataTypeEnum.pkey)
            {
            }

            public override object ConvertFromString(string s)
            {
                return Convert.ToInt32(s);
            }

            public override Type TargetType
            {
                get
                {
                    return typeof(int);
                }
            }
        }

        private class LongHelper : BaseDataTypeHelper
        {
            public LongHelper() : base(DataTypeEnum.plong)
            {
            }

            public override object ConvertFromString(string s)
            {
                return Convert.ToInt64(s);
            }

            public override Type TargetType
            {
                get
                {
                    return typeof(long);
                }
            }
        }

        private class PasswordHelper : BaseDataTypeHelper
        {
            public PasswordHelper() : base(DataTypeEnum.ppassword)
            {
            }

            public override object ConvertFromString(string s)
            {
                return s;
            }

            public override Type TargetType
            {
                get
                {
                    return typeof(string);
                }
            }
        }

        private class RefHelper : BaseDataTypeHelper
        {
            public RefHelper() : base(DataTypeEnum.pref)
            {
            }

            public override object ConvertFromString(string s)
            {
                return Convert.ToInt32(s);
            }

            public override Type TargetType
            {
                get
                {
                    return typeof(int);
                }
            }
        }

        private class StringHelper : BaseDataTypeHelper
        {
            public StringHelper() : base(DataTypeEnum.pstring)
            {
            }

            public override object ConvertFromString(string s)
            {
                return s;
            }

            public override Type TargetType
            {
                get
                {
                    return typeof(string);
                }
            }
        }

        private class TextHelper : BaseDataTypeHelper
        {
            public TextHelper() : base(DataTypeEnum.ptext)
            {
            }

            public override object ConvertFromString(string s)
            {
                return s;
            }

            public override Type TargetType
            {
                get
                {
                    return typeof(string);
                }
            }
        }
    }
}

