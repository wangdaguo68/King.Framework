namespace King.Framework.LiteQueryDef.Internal
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class LiteVariable
    {
        public FullTextTypeEnum FullTextType;

        private LiteVariable()
        {
        }

        private LiteVariable(LiteQuery subQuery)
        {
            this.VariableType = VariableTypeEnum.SubQuery;
            this.SubQuery = subQuery;
        }

        private LiteVariable(ArrayList constArrayValue)
        {
            this.VariableType = VariableTypeEnum.Array;
            this.ConstListValue = constArrayValue;
        }

        private LiteVariable(VariableTypeEnum varType, object constValue)
        {
            this.VariableType = VariableTypeEnum.Const;
            this.ConstValue = constValue;
        }

        private LiteVariable(string tableAlias, string fieldName)
        {
            this.VariableType = VariableTypeEnum.Field;
            this.TableAlias = tableAlias;
            this.FieldName = fieldName;
        }

        public static LiteVariable FromConst(object constValue)
        {
            return new LiteVariable { VariableType = VariableTypeEnum.Const, ConstValue = constValue };
        }

        public static LiteVariable FromConstArray(ArrayList constArray)
        {
            return new LiteVariable { VariableType = VariableTypeEnum.Array, ConstListValue = constArray };
        }

        public static LiteVariable FromField(FilterField field)
        {
            LiteVariable variable = new LiteVariable
            {
                VariableType = VariableTypeEnum.Field,
                TableAlias = field.TableAlias,
                FieldName = field.FieldName,
                DataType = field._dataType
            };
            if (field.FieldNameList.Count > 1)
            {
                variable.FullTextType = field.FullTextType;
                variable.FieldNameList = field.FieldNameList;
                variable.IsFullTextSearch = true;
            }
            return variable;
        }

        public static LiteVariable FromField(string tableAlias, string fieldName)
        {
            return new LiteVariable { VariableType = VariableTypeEnum.Field, TableAlias = tableAlias, FieldName = fieldName };
        }

        public static LiteVariable FromField(string tableAlias, string fieldName, Type dataType)
        {
            return new LiteVariable { VariableType = VariableTypeEnum.Field, TableAlias = tableAlias, FieldName = fieldName, DataType = dataType };
        }

        public static LiteVariable FromSubQuery(LiteQuery subQuery)
        {
            return new LiteVariable { VariableType = VariableTypeEnum.SubQuery, SubQuery = subQuery };
        }

        public ArrayList ConstListValue { get; private set; }

        public object ConstValue { get; private set; }

        public Type DataType { get; set; }

        public string FieldName { get; private set; }

        public List<string> FieldNameList { get; private set; }

        public bool IsFullTextSearch { get; private set; }

        public LiteQuery SubQuery { get; private set; }

        public string TableAlias { get; private set; }

        public VariableTypeEnum VariableType { get; private set; }
    }
}
