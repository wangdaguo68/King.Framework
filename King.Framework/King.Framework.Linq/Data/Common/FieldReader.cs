namespace King.Framework.Linq.Data.Common
{
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public abstract class FieldReader
    {
        private static MethodInfo _miReadNullableValue;
        private static MethodInfo _miReadValue;
        private static Dictionary<Type, MethodInfo> _readerMethods;
        private const TypeCode tcByteArray = (TypeCode.DateTime | TypeCode.SByte);
        private const TypeCode tcCharArray = (TypeCode.String | TypeCode.Char);
        private const TypeCode tcGuid = (TypeCode.DateTime | TypeCode.Char);
        private TypeCode[] typeCodes;

        protected abstract byte GetByte(int ordinal);
        protected abstract char GetChar(int ordinal);
        protected abstract DateTime GetDateTime(int ordinal);
        protected abstract decimal GetDecimal(int ordinal);
        protected abstract double GetDouble(int ordinal);
        protected abstract Type GetFieldType(int ordinal);
        protected abstract Guid GetGuid(int ordinal);
        protected abstract short GetInt16(int ordinal);
        protected abstract int GetInt32(int ordinal);
        protected abstract long GetInt64(int ordinal);
        public static MethodInfo GetReaderMethod(Type type)
        {
            MethodInfo info;
            if (_readerMethods == null)
            {
                List<MethodInfo> source = (from m in typeof(FieldReader).GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    where m.Name.StartsWith("Read")
                    select m).ToList<MethodInfo>();
                _readerMethods = source.ToDictionary<MethodInfo, Type>(m => m.ReturnType);
                _miReadValue = source.Single<MethodInfo>(m => m.Name == "ReadValue");
                _miReadNullableValue = source.Single<MethodInfo>(m => m.Name == "ReadNullableValue");
            }
            _readerMethods.TryGetValue(type, out info);
            if (info != null)
            {
                return info;
            }
            if (King.Framework.Linq.TypeHelper.IsNullableType(type))
            {
                return _miReadNullableValue.MakeGenericMethod(new Type[] { King.Framework.Linq.TypeHelper.GetNonNullableType(type) });
            }
            return _miReadValue.MakeGenericMethod(new Type[] { type });
        }

        protected abstract float GetSingle(int ordinal);
        protected abstract string GetString(int ordinal);
        private TypeCode GetTypeCode(int ordinal)
        {
            Type fieldType = this.GetFieldType(ordinal);
            TypeCode typeCode = Type.GetTypeCode(fieldType);
            if (typeCode == TypeCode.Object)
            {
                if (fieldType == typeof(Guid))
                {
                    return (TypeCode.DateTime | TypeCode.Char);
                }
                if (fieldType == typeof(byte[]))
                {
                    return (TypeCode.DateTime | TypeCode.SByte);
                }
                if (fieldType == typeof(char[]))
                {
                    typeCode = TypeCode.String | TypeCode.Char;
                }
            }
            return typeCode;
        }

        protected abstract T GetValue<T>(int ordinal);
        protected void Init()
        {
            this.typeCodes = new TypeCode[this.FieldCount];
        }

        protected abstract bool IsDBNull(int ordinal);
        public byte ReadByte(int ordinal)
        {
            bool flag;
            if (this.IsDBNull(ordinal))
            {
                return 0;
            }
        Label_00C7:
            flag = true;
            switch (this.typeCodes[ordinal])
            {
                case TypeCode.Byte:
                    return this.GetByte(ordinal);

                case TypeCode.Int16:
                    return (byte) this.GetInt16(ordinal);

                case TypeCode.Int32:
                    return (byte) this.GetInt32(ordinal);

                case TypeCode.Int64:
                    return (byte) this.GetInt64(ordinal);

                case TypeCode.Single:
                    return (byte) this.GetSingle(ordinal);

                case TypeCode.Double:
                    return (byte) this.GetDouble(ordinal);

                case TypeCode.Decimal:
                    return (byte) this.GetDecimal(ordinal);

                case TypeCode.Empty:
                    this.typeCodes[ordinal] = this.GetTypeCode(ordinal);
                    goto Label_00C7;
            }
            return this.GetValue<byte>(ordinal);
        }

        public byte[] ReadByteArray(int ordinal)
        {
            bool flag;
            if (this.IsDBNull(ordinal))
            {
                return null;
            }
        Label_005A:
            flag = true;
            TypeCode code = this.typeCodes[ordinal];
            if (code != TypeCode.Empty)
            {
                if (code != TypeCode.Byte)
                {
                    return this.GetValue<byte[]>(ordinal);
                }
            }
            else
            {
                this.typeCodes[ordinal] = this.GetTypeCode(ordinal);
                goto Label_005A;
            }
            return new byte[] { this.GetByte(ordinal) };
        }

        public char ReadChar(int ordinal)
        {
            bool flag;
            if (this.IsDBNull(ordinal))
            {
                return '\0';
            }
        Label_00C7:
            flag = true;
            switch (this.typeCodes[ordinal])
            {
                case TypeCode.Byte:
                    return (char) this.GetByte(ordinal);

                case TypeCode.Int16:
                    return (char) ((ushort) this.GetInt16(ordinal));

                case TypeCode.Int32:
                    return (char) this.GetInt32(ordinal);

                case TypeCode.Int64:
                    return (char) ((ushort) this.GetInt64(ordinal));

                case TypeCode.Single:
                    return (char) ((ushort) this.GetSingle(ordinal));

                case TypeCode.Double:
                    return (char) ((ushort) this.GetDouble(ordinal));

                case TypeCode.Decimal:
                    return (char) this.GetDecimal(ordinal);

                case TypeCode.Empty:
                    this.typeCodes[ordinal] = this.GetTypeCode(ordinal);
                    goto Label_00C7;
            }
            return this.GetValue<char>(ordinal);
        }

        public char[] ReadCharArray(int ordinal)
        {
            bool flag;
            if (this.IsDBNull(ordinal))
            {
                return null;
            }
        Label_005A:
            flag = true;
            TypeCode code = this.typeCodes[ordinal];
            if (code != TypeCode.Empty)
            {
                if (code != TypeCode.Char)
                {
                    return this.GetValue<char[]>(ordinal);
                }
            }
            else
            {
                this.typeCodes[ordinal] = this.GetTypeCode(ordinal);
                goto Label_005A;
            }
            return new char[] { this.GetChar(ordinal) };
        }

        public DateTime ReadDateTime(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return new DateTime();
            }
            while (true)
            {
                TypeCode code = this.typeCodes[ordinal];
                if (code != TypeCode.Empty)
                {
                    if (code == TypeCode.DateTime)
                    {
                        return this.GetDateTime(ordinal);
                    }
                    return this.GetValue<DateTime>(ordinal);
                }
                this.typeCodes[ordinal] = this.GetTypeCode(ordinal);
            }
        }

        public decimal ReadDecimal(int ordinal)
        {
            bool flag;
            if (this.IsDBNull(ordinal))
            {
                return 0M;
            }
        Label_00E2:
            flag = true;
            switch (this.typeCodes[ordinal])
            {
                case TypeCode.Byte:
                    return this.GetByte(ordinal);

                case TypeCode.Int16:
                    return this.GetInt16(ordinal);

                case TypeCode.Int32:
                    return this.GetInt32(ordinal);

                case TypeCode.Int64:
                    return this.GetInt64(ordinal);

                case TypeCode.Single:
                    return (decimal) this.GetSingle(ordinal);

                case TypeCode.Double:
                    return (decimal) this.GetDouble(ordinal);

                case TypeCode.Decimal:
                    return this.GetDecimal(ordinal);

                case TypeCode.Empty:
                    this.typeCodes[ordinal] = this.GetTypeCode(ordinal);
                    goto Label_00E2;
            }
            return this.GetValue<decimal>(ordinal);
        }

        public double ReadDouble(int ordinal)
        {
            bool flag;
            if (this.IsDBNull(ordinal))
            {
                return 0.0;
            }
        Label_00D0:
            flag = true;
            switch (this.typeCodes[ordinal])
            {
                case TypeCode.Byte:
                    return (double) this.GetByte(ordinal);

                case TypeCode.Int16:
                    return (double) this.GetInt16(ordinal);

                case TypeCode.Int32:
                    return (double) this.GetInt32(ordinal);

                case TypeCode.Int64:
                    return (double) this.GetInt64(ordinal);

                case TypeCode.Single:
                    return (double) this.GetSingle(ordinal);

                case TypeCode.Double:
                    return this.GetDouble(ordinal);

                case TypeCode.Decimal:
                    return (double) this.GetDecimal(ordinal);

                case TypeCode.Empty:
                    this.typeCodes[ordinal] = this.GetTypeCode(ordinal);
                    goto Label_00D0;
            }
            return this.GetValue<double>(ordinal);
        }

        public Guid ReadGuid(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return new Guid();
            }
            while (true)
            {
                TypeCode code = this.typeCodes[ordinal];
                if (code != TypeCode.Empty)
                {
                    if (code == (TypeCode.DateTime | TypeCode.Char))
                    {
                        return this.GetGuid(ordinal);
                    }
                    return this.GetValue<Guid>(ordinal);
                }
                this.typeCodes[ordinal] = this.GetTypeCode(ordinal);
            }
        }

        public short ReadInt16(int ordinal)
        {
            bool flag;
            if (this.IsDBNull(ordinal))
            {
                return 0;
            }
        Label_00C6:
            flag = true;
            switch (this.typeCodes[ordinal])
            {
                case TypeCode.Byte:
                    return this.GetByte(ordinal);

                case TypeCode.Int16:
                    return this.GetInt16(ordinal);

                case TypeCode.Int32:
                    return (short) this.GetInt32(ordinal);

                case TypeCode.Int64:
                    return (short) this.GetInt64(ordinal);

                case TypeCode.Single:
                    return (short) this.GetSingle(ordinal);

                case TypeCode.Double:
                    return (short) this.GetDouble(ordinal);

                case TypeCode.Decimal:
                    return (short) this.GetDecimal(ordinal);

                case TypeCode.Empty:
                    this.typeCodes[ordinal] = this.GetTypeCode(ordinal);
                    goto Label_00C6;
            }
            return this.GetValue<short>(ordinal);
        }

        public int ReadInt32(int ordinal)
        {
            bool flag;
            if (this.IsDBNull(ordinal))
            {
                return 0;
            }
        Label_00C5:
            flag = true;
            switch (this.typeCodes[ordinal])
            {
                case TypeCode.Byte:
                    return this.GetByte(ordinal);

                case TypeCode.Int16:
                    return this.GetInt16(ordinal);

                case TypeCode.Int32:
                    return this.GetInt32(ordinal);

                case TypeCode.Int64:
                    return (int) this.GetInt64(ordinal);

                case TypeCode.Single:
                    return (int) this.GetSingle(ordinal);

                case TypeCode.Double:
                    return (int) this.GetDouble(ordinal);

                case TypeCode.Decimal:
                    return (int) this.GetDecimal(ordinal);

                case TypeCode.Empty:
                    this.typeCodes[ordinal] = this.GetTypeCode(ordinal);
                    goto Label_00C5;
            }
            return this.GetValue<int>(ordinal);
        }

        public long ReadInt64(int ordinal)
        {
            bool flag;
            if (this.IsDBNull(ordinal))
            {
                return 0L;
            }
        Label_00C8:
            flag = true;
            switch (this.typeCodes[ordinal])
            {
                case TypeCode.Byte:
                    return (long) this.GetByte(ordinal);

                case TypeCode.Int16:
                    return (long) this.GetInt16(ordinal);

                case TypeCode.Int32:
                    return (long) this.GetInt32(ordinal);

                case TypeCode.Int64:
                    return this.GetInt64(ordinal);

                case TypeCode.Single:
                    return (long) this.GetSingle(ordinal);

                case TypeCode.Double:
                    return (long) this.GetDouble(ordinal);

                case TypeCode.Decimal:
                    return (long) this.GetDecimal(ordinal);

                case TypeCode.Empty:
                    this.typeCodes[ordinal] = this.GetTypeCode(ordinal);
                    goto Label_00C8;
            }
            return this.GetValue<long>(ordinal);
        }

        public byte? ReadNullableByte(int ordinal)
        {
            bool flag;
            if (this.IsDBNull(ordinal))
            {
                return null;
            }
        Label_00FD:
            flag = true;
            switch (this.typeCodes[ordinal])
            {
                case TypeCode.Byte:
                    return new byte?(this.GetByte(ordinal));

                case TypeCode.Int16:
                    return new byte?((byte) this.GetInt16(ordinal));

                case TypeCode.Int32:
                    return new byte?((byte) this.GetInt32(ordinal));

                case TypeCode.Int64:
                    return new byte?((byte) this.GetInt64(ordinal));

                case TypeCode.Single:
                    return new byte?((byte) this.GetSingle(ordinal));

                case TypeCode.Double:
                    return new byte?((byte) this.GetDouble(ordinal));

                case TypeCode.Decimal:
                    return new byte?((byte) this.GetDecimal(ordinal));

                case TypeCode.Empty:
                    this.typeCodes[ordinal] = this.GetTypeCode(ordinal);
                    goto Label_00FD;
            }
            return new byte?(this.GetValue<byte>(ordinal));
        }

        public char? ReadNullableChar(int ordinal)
        {
            bool flag;
            if (this.IsDBNull(ordinal))
            {
                return null;
            }
        Label_00FD:
            flag = true;
            switch (this.typeCodes[ordinal])
            {
                case TypeCode.Byte:
                    return new char?((char) this.GetByte(ordinal));

                case TypeCode.Int16:
                    return new char?((char) ((ushort) this.GetInt16(ordinal)));

                case TypeCode.Int32:
                    return new char?((char) ((ushort) this.GetInt32(ordinal)));

                case TypeCode.Int64:
                    return new char?((char) ((ushort) this.GetInt64(ordinal)));

                case TypeCode.Single:
                    return new char?((char) ((ushort) this.GetSingle(ordinal)));

                case TypeCode.Double:
                    return new char?((char) ((ushort) this.GetDouble(ordinal)));

                case TypeCode.Decimal:
                    return new char?((char) this.GetDecimal(ordinal));

                case TypeCode.Empty:
                    this.typeCodes[ordinal] = this.GetTypeCode(ordinal);
                    goto Label_00FD;
            }
            return new char?(this.GetValue<char>(ordinal));
        }

        public DateTime? ReadNullableDateTime(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return null;
            }
            while (true)
            {
                TypeCode code = this.typeCodes[ordinal];
                if (code != TypeCode.Empty)
                {
                    if (code == TypeCode.DateTime)
                    {
                        return new DateTime?(this.GetDateTime(ordinal));
                    }
                    return new DateTime?(this.GetValue<DateTime>(ordinal));
                }
                this.typeCodes[ordinal] = this.GetTypeCode(ordinal);
            }
        }

        public decimal? ReadNullableDecimal(int ordinal)
        {
            bool flag;
            if (this.IsDBNull(ordinal))
            {
                return null;
            }
        Label_0116:
            flag = true;
            switch (this.typeCodes[ordinal])
            {
                case TypeCode.Byte:
                    return new decimal?(this.GetByte(ordinal));

                case TypeCode.Int16:
                    return new decimal?(this.GetInt16(ordinal));

                case TypeCode.Int32:
                    return new decimal?(this.GetInt32(ordinal));

                case TypeCode.Int64:
                    return new decimal?(this.GetInt64(ordinal));

                case TypeCode.Single:
                    return new decimal?((decimal) this.GetSingle(ordinal));

                case TypeCode.Double:
                    return new decimal?((decimal) this.GetDouble(ordinal));

                case TypeCode.Decimal:
                    return new decimal?(this.GetDecimal(ordinal));

                case TypeCode.Empty:
                    this.typeCodes[ordinal] = this.GetTypeCode(ordinal);
                    goto Label_0116;
            }
            return new decimal?(this.GetValue<decimal>(ordinal));
        }

        public double? ReadNullableDouble(int ordinal)
        {
            bool flag;
            if (this.IsDBNull(ordinal))
            {
                return null;
            }
        Label_00FE:
            flag = true;
            switch (this.typeCodes[ordinal])
            {
                case TypeCode.Byte:
                    return new double?((double) this.GetByte(ordinal));

                case TypeCode.Int16:
                    return new double?((double) this.GetInt16(ordinal));

                case TypeCode.Int32:
                    return new double?((double) this.GetInt32(ordinal));

                case TypeCode.Int64:
                    return new double?((double) this.GetInt64(ordinal));

                case TypeCode.Single:
                    return new double?((double) this.GetSingle(ordinal));

                case TypeCode.Double:
                    return new double?(this.GetDouble(ordinal));

                case TypeCode.Decimal:
                    return new double?((double) this.GetDecimal(ordinal));

                case TypeCode.Empty:
                    this.typeCodes[ordinal] = this.GetTypeCode(ordinal);
                    goto Label_00FE;
            }
            return new double?(this.GetValue<double>(ordinal));
        }

        public Guid? ReadNullableGuid(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return null;
            }
            while (true)
            {
                TypeCode code = this.typeCodes[ordinal];
                if (code != TypeCode.Empty)
                {
                    if (code == (TypeCode.DateTime | TypeCode.Char))
                    {
                        return new Guid?(this.GetGuid(ordinal));
                    }
                    return new Guid?(this.GetValue<Guid>(ordinal));
                }
                this.typeCodes[ordinal] = this.GetTypeCode(ordinal);
            }
        }

        public short? ReadNullableInt16(int ordinal)
        {
            bool flag;
            if (this.IsDBNull(ordinal))
            {
                return null;
            }
        Label_00FC:
            flag = true;
            switch (this.typeCodes[ordinal])
            {
                case TypeCode.Byte:
                    return new short?(this.GetByte(ordinal));

                case TypeCode.Int16:
                    return new short?(this.GetInt16(ordinal));

                case TypeCode.Int32:
                    return new short?((short) this.GetInt32(ordinal));

                case TypeCode.Int64:
                    return new short?((short) this.GetInt64(ordinal));

                case TypeCode.Single:
                    return new short?((short) this.GetSingle(ordinal));

                case TypeCode.Double:
                    return new short?((short) this.GetDouble(ordinal));

                case TypeCode.Decimal:
                    return new short?((short) this.GetDecimal(ordinal));

                case TypeCode.Empty:
                    this.typeCodes[ordinal] = this.GetTypeCode(ordinal);
                    goto Label_00FC;
            }
            return new short?(this.GetValue<short>(ordinal));
        }

        public int? ReadNullableInt32(int ordinal)
        {
            bool flag;
            if (this.IsDBNull(ordinal))
            {
                return null;
            }
        Label_00FB:
            flag = true;
            switch (this.typeCodes[ordinal])
            {
                case TypeCode.Byte:
                    return new int?(this.GetByte(ordinal));

                case TypeCode.Int16:
                    return new int?(this.GetInt16(ordinal));

                case TypeCode.Int32:
                    return new int?(this.GetInt32(ordinal));

                case TypeCode.Int64:
                    return new int?((int) this.GetInt64(ordinal));

                case TypeCode.Single:
                    return new int?((int) this.GetSingle(ordinal));

                case TypeCode.Double:
                    return new int?((int) this.GetDouble(ordinal));

                case TypeCode.Decimal:
                    return new int?((int) this.GetDecimal(ordinal));

                case TypeCode.Empty:
                    this.typeCodes[ordinal] = this.GetTypeCode(ordinal);
                    goto Label_00FB;
            }
            return new int?(this.GetValue<int>(ordinal));
        }

        public long? ReadNullableInt64(int ordinal)
        {
            bool flag;
            if (this.IsDBNull(ordinal))
            {
                return null;
            }
        Label_00FD:
            flag = true;
            switch (this.typeCodes[ordinal])
            {
                case TypeCode.Byte:
                    return new long?((long) this.GetByte(ordinal));

                case TypeCode.Int16:
                    return new long?((long) this.GetInt16(ordinal));

                case TypeCode.Int32:
                    return new long?((long) this.GetInt32(ordinal));

                case TypeCode.Int64:
                    return new long?(this.GetInt64(ordinal));

                case TypeCode.Single:
                    return new long?((long) this.GetSingle(ordinal));

                case TypeCode.Double:
                    return new long?((long) this.GetDouble(ordinal));

                case TypeCode.Decimal:
                    return new long?((long) this.GetDecimal(ordinal));

                case TypeCode.Empty:
                    this.typeCodes[ordinal] = this.GetTypeCode(ordinal);
                    goto Label_00FD;
            }
            return new long?(this.GetValue<long>(ordinal));
        }

        public float? ReadNullableSingle(int ordinal)
        {
            bool flag;
            if (this.IsDBNull(ordinal))
            {
                return null;
            }
        Label_00FE:
            flag = true;
            switch (this.typeCodes[ordinal])
            {
                case TypeCode.Byte:
                    return new float?((float) this.GetByte(ordinal));

                case TypeCode.Int16:
                    return new float?((float) this.GetInt16(ordinal));

                case TypeCode.Int32:
                    return new float?((float) this.GetInt32(ordinal));

                case TypeCode.Int64:
                    return new float?((float) this.GetInt64(ordinal));

                case TypeCode.Single:
                    return new float?(this.GetSingle(ordinal));

                case TypeCode.Double:
                    return new float?((float) this.GetDouble(ordinal));

                case TypeCode.Decimal:
                    return new float?((float) this.GetDecimal(ordinal));

                case TypeCode.Empty:
                    this.typeCodes[ordinal] = this.GetTypeCode(ordinal);
                    goto Label_00FE;
            }
            return new float?(this.GetValue<float>(ordinal));
        }

        public T? ReadNullableValue<T>(int ordinal) where T: struct
        {
            if (this.IsDBNull(ordinal))
            {
                return null;
            }
            return new T?(this.GetValue<T>(ordinal));
        }

        public float ReadSingle(int ordinal)
        {
            bool flag;
            if (this.IsDBNull(ordinal))
            {
                return 0f;
            }
        Label_00CC:
            flag = true;
            switch (this.typeCodes[ordinal])
            {
                case TypeCode.Byte:
                    return (float) this.GetByte(ordinal);

                case TypeCode.Int16:
                    return (float) this.GetInt16(ordinal);

                case TypeCode.Int32:
                    return (float) this.GetInt32(ordinal);

                case TypeCode.Int64:
                    return (float) this.GetInt64(ordinal);

                case TypeCode.Single:
                    return this.GetSingle(ordinal);

                case TypeCode.Double:
                    return (float) this.GetDouble(ordinal);

                case TypeCode.Decimal:
                    return (float) this.GetDecimal(ordinal);

                case TypeCode.Empty:
                    this.typeCodes[ordinal] = this.GetTypeCode(ordinal);
                    goto Label_00CC;
            }
            return this.GetValue<float>(ordinal);
        }

        public string ReadString(int ordinal)
        {
            bool flag;
            if (this.IsDBNull(ordinal))
            {
                return null;
            }
        Label_0174:
            flag = true;
            switch (this.typeCodes[ordinal])
            {
                case TypeCode.Empty:
                    this.typeCodes[ordinal] = Type.GetTypeCode(this.GetFieldType(ordinal));
                    goto Label_0174;

                case TypeCode.Byte:
                    return this.GetByte(ordinal).ToString();

                case TypeCode.Int16:
                    return this.GetInt16(ordinal).ToString();

                case TypeCode.Int32:
                    return this.GetInt32(ordinal).ToString();

                case TypeCode.Int64:
                    return this.GetInt64(ordinal).ToString();

                case TypeCode.Single:
                    return this.GetSingle(ordinal).ToString();

                case TypeCode.Double:
                    return this.GetDouble(ordinal).ToString();

                case TypeCode.Decimal:
                    return this.GetDecimal(ordinal).ToString();

                case TypeCode.DateTime:
                    return this.GetDateTime(ordinal).ToString();

                case TypeCode.String:
                    return this.GetString(ordinal);

                case (TypeCode.DateTime | TypeCode.Char):
                    return this.GetGuid(ordinal).ToString();
            }
            return this.GetValue<string>(ordinal);
        }

        public T ReadValue<T>(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return default(T);
            }
            return this.GetValue<T>(ordinal);
        }

        protected abstract int FieldCount { get; }
    }
}
