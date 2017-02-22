namespace King.Framework.Linq.Data
{
    using King.Framework.Linq;
    using King.Framework.Linq.Data.Common;
    using System;
    using System.Data;
    using System.Text;

    public class DbTypeSystem : QueryTypeSystem
    {
        public override QueryType GetColumnType(Type type)
        {
            bool isNotNull = type.IsValueType && !TypeHelper.IsNullableType(type);
            type = TypeHelper.GetNonNullableType(type);
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return this.NewType(SqlDbType.Bit, isNotNull, 0, 0, 0);

                case TypeCode.Char:
                    return this.NewType(SqlDbType.NChar, isNotNull, 1, 0, 0);

                case TypeCode.SByte:
                case TypeCode.Byte:
                    return this.NewType(SqlDbType.TinyInt, isNotNull, 0, 0, 0);

                case TypeCode.Int16:
                case TypeCode.UInt16:
                    return this.NewType(SqlDbType.SmallInt, isNotNull, 0, 0, 0);

                case TypeCode.Int32:
                case TypeCode.UInt32:
                    return this.NewType(SqlDbType.Int, isNotNull, 0, 0, 0);

                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return this.NewType(SqlDbType.BigInt, isNotNull, 0, 0, 0);

                case TypeCode.Single:
                case TypeCode.Double:
                    return this.NewType(SqlDbType.Float, isNotNull, 0, 0, 0);

                case TypeCode.Decimal:
                    return this.NewType(SqlDbType.Decimal, isNotNull, 0, 0x1d, 4);

                case TypeCode.DateTime:
                    return this.NewType(SqlDbType.DateTime, isNotNull, 0, 0, 0);

                case TypeCode.String:
                    return this.NewType(SqlDbType.NVarChar, isNotNull, this.StringDefaultSize, 0, 0);
            }
            if (type == typeof(byte[]))
            {
                return this.NewType(SqlDbType.VarBinary, isNotNull, this.BinaryDefaultSize, 0, 0);
            }
            if (type == typeof(Guid))
            {
                return this.NewType(SqlDbType.UniqueIdentifier, isNotNull, 0, 0, 0);
            }
            if (type == typeof(DateTimeOffset))
            {
                return this.NewType(SqlDbType.DateTimeOffset, isNotNull, 0, 0, 0);
            }
            if (type == typeof(TimeSpan))
            {
                return this.NewType(SqlDbType.Time, isNotNull, 0, 0, 0);
            }
            return null;
        }

        public static DbType GetDbType(SqlDbType dbType)
        {
            switch (dbType)
            {
                case SqlDbType.BigInt:
                    return DbType.Int64;

                case SqlDbType.Binary:
                    return DbType.Binary;

                case SqlDbType.Bit:
                    return DbType.Boolean;

                case SqlDbType.Char:
                    return DbType.AnsiStringFixedLength;

                case SqlDbType.DateTime:
                case SqlDbType.SmallDateTime:
                    return DbType.DateTime;

                case SqlDbType.Decimal:
                    return DbType.Decimal;

                case SqlDbType.Float:
                case SqlDbType.Real:
                    return DbType.Double;

                case SqlDbType.Image:
                    return DbType.Binary;

                case SqlDbType.Int:
                    return DbType.Int32;

                case SqlDbType.Money:
                case SqlDbType.SmallMoney:
                    return DbType.Currency;

                case SqlDbType.NChar:
                    return DbType.StringFixedLength;

                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                    return DbType.String;

                case SqlDbType.UniqueIdentifier:
                    return DbType.Guid;

                case SqlDbType.SmallInt:
                    return DbType.Int16;

                case SqlDbType.Text:
                    return DbType.AnsiString;

                case SqlDbType.Timestamp:
                    return DbType.Binary;

                case SqlDbType.TinyInt:
                    return DbType.SByte;

                case SqlDbType.VarBinary:
                    return DbType.Binary;

                case SqlDbType.VarChar:
                    return DbType.AnsiString;

                case SqlDbType.Variant:
                    return DbType.Object;

                case SqlDbType.Xml:
                    return DbType.String;

                case SqlDbType.Udt:
                    return DbType.Object;

                case SqlDbType.Date:
                    return DbType.Date;

                case SqlDbType.Time:
                    return DbType.Time;

                case SqlDbType.DateTime2:
                    return DbType.DateTime2;

                case SqlDbType.DateTimeOffset:
                    return DbType.DateTimeOffset;
            }
            throw new InvalidOperationException(string.Format("Unhandled sql type: {0}", dbType));
        }

        public virtual QueryType GetQueryType(string typeName, string[] args, bool isNotNull)
        {
            if (string.Compare(typeName, "rowversion", StringComparison.OrdinalIgnoreCase) == 0)
            {
                typeName = "Timestamp";
            }
            if (string.Compare(typeName, "numeric", StringComparison.OrdinalIgnoreCase) == 0)
            {
                typeName = "Decimal";
            }
            if (string.Compare(typeName, "sql_variant", StringComparison.OrdinalIgnoreCase) == 0)
            {
                typeName = "Variant";
            }
            SqlDbType sqlType = this.GetSqlType(typeName);
            int length = 0;
            short precision = 0;
            short scale = 0;
            switch (sqlType)
            {
                case SqlDbType.Binary:
                case SqlDbType.Char:
                case SqlDbType.Image:
                case SqlDbType.NChar:
                case SqlDbType.NVarChar:
                case SqlDbType.VarBinary:
                case SqlDbType.VarChar:
                    if ((args == null) || (args.Length < 1))
                    {
                        length = 80;
                    }
                    else if (string.Compare(args[0], "max", true) == 0)
                    {
                        length = 0x7fffffff;
                    }
                    else
                    {
                        length = int.Parse(args[0]);
                    }
                    goto Label_01E6;

                case SqlDbType.Decimal:
                    if ((args != null) && (args.Length >= 1))
                    {
                        precision = short.Parse(args[0]);
                    }
                    else
                    {
                        precision = 0x1d;
                    }
                    if ((args == null) || (args.Length < 2))
                    {
                        scale = 0;
                    }
                    else
                    {
                        scale = short.Parse(args[1]);
                    }
                    goto Label_01E6;

                case SqlDbType.Float:
                case SqlDbType.Real:
                    if ((args != null) && (args.Length >= 1))
                    {
                        precision = short.Parse(args[0]);
                    }
                    else
                    {
                        precision = 0x1d;
                    }
                    goto Label_01E6;

                case SqlDbType.Money:
                    if ((args != null) && (args.Length >= 1))
                    {
                        precision = short.Parse(args[0]);
                        break;
                    }
                    precision = 0x1d;
                    break;

                default:
                    goto Label_01E6;
            }
            if ((args == null) || (args.Length < 2))
            {
                scale = 4;
            }
            else
            {
                scale = short.Parse(args[1]);
            }
            Label_01E6:
            return this.NewType(sqlType, isNotNull, length, precision, scale);
        }

        public virtual SqlDbType GetSqlType(string typeName)
        {
            return (SqlDbType)Enum.Parse(typeof(SqlDbType), typeName, true);
        }

        public override string GetVariableDeclaration(QueryType type, bool suppressSize)
        {
            DbQueryType type2 = (DbQueryType)type;
            StringBuilder builder = new StringBuilder();
            builder.Append(type2.SqlDbType.ToString().ToUpper());
            if ((type2.Length > 0) && !suppressSize)
            {
                if (type2.Length == 0x7fffffff)
                {
                    builder.Append("(max)");
                }
                else
                {
                    builder.AppendFormat("({0})", type2.Length);
                }
            }
            else if (type2.Precision != 0)
            {
                if (type2.Scale != 0)
                {
                    builder.AppendFormat("({0},{1})", type2.Precision, type2.Scale);
                }
                else
                {
                    builder.AppendFormat("({0})", type2.Precision);
                }
            }
            return builder.ToString();
        }

        public static bool IsVariableLength(SqlDbType dbType)
        {
            switch (dbType)
            {
                case SqlDbType.Text:
                case SqlDbType.VarBinary:
                case SqlDbType.VarChar:
                case SqlDbType.Xml:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                case SqlDbType.Image:
                    return true;
            }
            return false;
        }

        public virtual QueryType NewType(SqlDbType type, bool isNotNull, int length, short precision, short scale)
        {
            return new DbQueryType(type, isNotNull, length, precision, scale);
        }

        public override QueryType Parse(string typeDeclaration)
        {
            string[] args = null;
            string typeName = null;
            string str2 = null;
            int index = typeDeclaration.IndexOf('(');
            if (index >= 0)
            {
                typeName = typeDeclaration.Substring(0, index).Trim();
                int length = typeDeclaration.IndexOf(')', index);
                if (length < index)
                {
                    length = typeDeclaration.Length;
                }
                args = typeDeclaration.Substring(index + 1, length - (index + 1)).Split(new char[] { ',' });
                str2 = typeDeclaration.Substring(length + 1);
            }
            else
            {
                int num3 = typeDeclaration.IndexOf(' ');
                if (num3 >= 0)
                {
                    typeName = typeDeclaration.Substring(0, num3);
                    str2 = typeDeclaration.Substring(num3 + 1).Trim();
                }
                else
                {
                    typeName = typeDeclaration;
                }
            }
            bool isNotNull = (str2 != null) ? str2.ToUpper().Contains("NOT NULL") : false;
            return this.GetQueryType(typeName, args, isNotNull);
        }

        public virtual int BinaryDefaultSize
        {
            get
            {
                return 0x7fffffff;
            }
        }

        public virtual int StringDefaultSize
        {
            get
            {
                return 0x7fffffff;
            }
        }
    }
}
