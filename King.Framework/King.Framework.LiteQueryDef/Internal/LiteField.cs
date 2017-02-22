namespace King.Framework.LiteQueryDef.Internal
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class LiteField
    {
        private LiteFieldTypeEnum _fieldType = LiteFieldTypeEnum.EntityField;

        private LiteField()
        {
        }

        public static LiteField AVG(string tableAlias, string fieldName, string fieldAlias)
        {
            return new LiteField { FieldType = LiteFieldTypeEnum.Function, FunctionName = StatisticFunctionsEnum.AVG, TableAlias = tableAlias, FieldName = fieldName, FieldAlias = string.IsNullOrEmpty(fieldAlias) ? fieldName : fieldAlias };
        }

        public static LiteField COUNT(string tableAlias, string fieldName, string fieldAlias = null)
        {
            return new LiteField { FieldType = LiteFieldTypeEnum.Function, FunctionName = StatisticFunctionsEnum.COUNT, TableAlias = tableAlias, FieldName = fieldName, FieldAlias = string.IsNullOrEmpty(fieldAlias) ? fieldName : fieldAlias };
        }

        public static LiteField MAX(string tableAlias, string fieldName, string fieldAlias)
        {
            return new LiteField { FieldType = LiteFieldTypeEnum.Function, FunctionName = StatisticFunctionsEnum.MAX, TableAlias = tableAlias, FieldName = fieldName, FieldAlias = string.IsNullOrEmpty(fieldAlias) ? fieldName : fieldAlias };
        }

        public static LiteField MIN(string tableAlias, string fieldName, string fieldAlias)
        {
            return new LiteField { FieldType = LiteFieldTypeEnum.Function, FunctionName = StatisticFunctionsEnum.MIN, TableAlias = tableAlias, FieldName = fieldName, FieldAlias = string.IsNullOrEmpty(fieldAlias) ? fieldName : fieldAlias };
        }

        public static LiteField NewConstField(object constValue, string fieldAlias)
        {
            return new LiteField { FieldType = LiteFieldTypeEnum.ConstField, ConstValue = constValue, DataType = (constValue == null) ? null : constValue.GetType(), FieldAlias = fieldAlias };
        }

        public static LiteField NewEntityField(string tableAlias, string fieldName)
        {
            return new LiteField { FieldType = LiteFieldTypeEnum.EntityField, TableAlias = tableAlias, FieldName = fieldName, FieldAlias = fieldName };
        }

        public static LiteField NewEntityField(string tableAlias, string fieldName, string fieldAlias)
        {
            return new LiteField { FieldType = LiteFieldTypeEnum.EntityField, TableAlias = tableAlias, FieldName = fieldName, FieldAlias = fieldAlias };
        }

        public static LiteField NewSubQueryField(LiteQuery subQuery, string fieldAlias)
        {
            return new LiteField { FieldType = LiteFieldTypeEnum.SubQuery, FieldAlias = fieldAlias, SubQuery = subQuery };
        }

        public static LiteField SUM(string tableAlias, string fieldName, string fieldAlias)
        {
            return new LiteField { FieldType = LiteFieldTypeEnum.Function, FunctionName = StatisticFunctionsEnum.SUM, TableAlias = tableAlias, FieldName = fieldName, FieldAlias = string.IsNullOrEmpty(fieldAlias) ? fieldName : fieldAlias };
        }

        public object ConstValue { get; private set; }

        public Type DataType { get; private set; }

        public string FieldAlias { get; private set; }

        public string FieldName { get; private set; }

        public LiteFieldTypeEnum FieldType
        {
            get
            {
                return this._fieldType;
            }
            private set
            {
                this._fieldType = value;
            }
        }

        public StatisticFunctionsEnum? FunctionName { get; private set; }

        public LiteQuery SubQuery { get; private set; }

        public string TableAlias { get; private set; }
    }
}
