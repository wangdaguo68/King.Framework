using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using King.Framework.DAL.CSharpTypes;
using King.Framework.DAL.DataAccessLib;

namespace King.Framework.DAL
{
    public class Column
    {
        private readonly string _columnName;
        private readonly Type _dataType;
        private readonly bool _isIdentity;
        private readonly bool _isPrimaryKey;
        private readonly int _keyOrder;
        private int? _length;
        private readonly System.Reflection.PropertyInfo _propertyInfo;
        private readonly Type _simpleDataType;

        internal Column(System.Reflection.PropertyInfo propertyInfo, string columnName, Type dataType, int? length) : this(propertyInfo, columnName, dataType, length, false, 0, false, false)
        {
        }

        internal Column(System.Reflection.PropertyInfo propertyInfo, string columnName, Type dataType, int? length, bool isPrimaryKey, int keyOrder, bool isIdentity, bool isTableVersion)
        {
            this._simpleDataType = null;
            this._dataType = null;
            this._isPrimaryKey = false;
            this._keyOrder = 0;
            this._isIdentity = false;
            this._propertyInfo = propertyInfo;
            this._columnName = columnName;
            this._dataType = dataType;
            this._simpleDataType = this.GetSimpleType(dataType);
            this._isPrimaryKey = isPrimaryKey;
            this._keyOrder = keyOrder;
            this._isIdentity = isIdentity;
            this._length = length;
        }

        public string GetOracleTypeString()
        {
            Type dataType = this.DataType;
            if (dataType.IsEnum)
            {
                return "number(10)";
            }
            if (BaseDataAccess.IsNullableEnum(dataType))
            {
                return "number(10)";
            }
            if (dataType == typeof(string))
            {
                return "NVARCHAR2(256)";
            }
            if (dataType == typeof(int))
            {
                return "number(10)";
            }
            if (dataType == typeof(int?))
            {
                return "number(10)";
            }
            if (dataType == typeof(long))
            {
                return "number(19)";
            }
            if (dataType == typeof(long?))
            {
                return "number(19)";
            }
            if (dataType != typeof(decimal))
            {
                if (dataType == typeof(decimal?))
                {
                    return "number(18,3)";
                }
                if (dataType == typeof(DateTime))
                {
                    return "TIMESTAMP(6)";
                }
                if (dataType == typeof(DateTime?))
                {
                    return "TIMESTAMP(6)";
                }
                if (dataType == typeof(bool))
                {
                    return "number(1)";
                }
                if (dataType == typeof(bool?))
                {
                    return "number(1)";
                }
                if (dataType == typeof(Guid))
                {
                    return "NVARCHAR2(256)";
                }
                if (dataType == typeof(Guid?))
                {
                    return "NVARCHAR2(256)";
                }
                if (dataType == typeof(byte[]))
                {
                    return "BLOB";
                }
                if (dataType == typeof(byte))
                {
                    return "number(10)";
                }
                if (dataType == typeof(byte?))
                {
                    return "number(10)";
                }
                if (dataType == typeof(short))
                {
                    return "number(10)";
                }
                if (dataType == typeof(short?))
                {
                    return "number(10)";
                }
                if ((dataType != typeof(double)) && (dataType != typeof(double?)))
                {
                    throw new ApplicationException(string.Format("不支持的数据类型:{0}", dataType.ToString()));
                }
            }
            return "number(18,3)";
        }

        private Type GetSimpleType(Type type)
        {
            if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() != typeof(Nullable<>))
                {
                    throw new ApplicationException("不支持Nullable<>之外的泛型:" + type.ToString());
                }
                return type.GetGenericArguments()[0];
            }
            return type;
        }

        public string GetSqlTypeString()
        {
            Type dataType = this.DataType;
            if (dataType.IsEnum)
            {
                return "int";
            }
            if (BaseDataAccess.IsNullableEnum(dataType))
            {
                return "int";
            }
            if (dataType == typeof(string))
            {
                return "nvarchar(256)";
            }
            if (dataType == typeof(int))
            {
                return "int";
            }
            if (dataType == typeof(int?))
            {
                return "int";
            }
            if (dataType == typeof(long))
            {
                return "bigint";
            }
            if (dataType == typeof(long?))
            {
                return "bigint";
            }
            if (dataType == typeof(decimal))
            {
                return "decimal";
            }
            if (dataType == typeof(decimal?))
            {
                return "decimal";
            }
            if (dataType == typeof(DateTime))
            {
                return "date";
            }
            if (dataType == typeof(DateTime?))
            {
                return "date";
            }
            if (dataType == typeof(bool))
            {
                return "bit";
            }
            if (dataType == typeof(bool?))
            {
                return "bit";
            }
            if (dataType == typeof(Guid))
            {
                return "uniqueidentifier";
            }
            if (dataType == typeof(Guid?))
            {
                return "uniqueidentifier";
            }
            if (dataType == typeof(byte[]))
            {
                return "image";
            }
            if (dataType == typeof(byte))
            {
                return "tinyint";
            }
            if (dataType == typeof(byte?))
            {
                return "tinyint";
            }
            if (dataType == typeof(short))
            {
                return "smallint";
            }
            if (dataType == typeof(short?))
            {
                return "smallint";
            }
            if ((dataType != typeof(double)) && (dataType != typeof(double?)))
            {
                throw new ApplicationException(string.Format("不支持的数据类型:{0}", dataType.ToString()));
            }
            return "float";
        }

        public override string ToString()
        {
            return string.Format("{0}-{1}-{2}-{3}", new object[] { this._columnName, this._dataType, this._isPrimaryKey, this._keyOrder });
        }

        public string ColumnName
        {
            get
            {
                return this._columnName;
            }
        }

        public Type DataType
        {
            get
            {
                return this._dataType;
            }
        }

        public Func<object, object> GetFunction { get; internal set; }

        public bool IsIdentity
        {
            get
            {
                return this._isIdentity;
            }
        }

        public bool IsPrimaryKey
        {
            get
            {
                return this._isPrimaryKey;
            }
        }

        public bool IsTableVersion { get; private set; }

        public int KeyOrder
        {
            get
            {
                return this._keyOrder;
            }
        }

        public int? Length
        {
            get
            {
                return this._length;
            }
        }

        public System.Reflection.PropertyInfo PropertyInfo
        {
            get
            {
                return this._propertyInfo;
            }
        }

        public Action<object, object> SetAction { get; internal set; }

        public Type SimpleDataType
        {
            get
            {
                return this._simpleDataType;
            }
        }

        public ISharpType TypeHelper { get; internal set; }
    }
}
