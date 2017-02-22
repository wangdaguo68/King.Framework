namespace King.Framework.LiteQueryDef.Internal
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class FilterField
    {
        public readonly Type _dataType;
        public readonly string FieldName;
        public readonly List<string> FieldNameList;
        public FullTextTypeEnum FullTextType;
        public readonly string TableAlias;

        public FilterField(string tableAlias, string fieldName) : this(tableAlias, fieldName, null)
        {
        }

        public FilterField(string tableAlias, string fieldName, Type dataType)
        {
            this.TableAlias = tableAlias;
            this._dataType = dataType;
            this.FieldName = fieldName;
            this.FieldNameList = new List<string>();
            this.FieldNameList.Add(fieldName);
        }

        public LiteFilter Contains(object constValue)
        {
            return LiteFilter.Contains(this, constValue);
        }

        public LiteFilter EndWith(object constValue)
        {
            return LiteFilter.EndWith(this, constValue);
        }

        public LiteFilter Equal(int constValue)
        {
            return LiteFilter.EqualConst(this, constValue);
        }

        public LiteFilter Equal(long constValue)
        {
            return LiteFilter.EqualConst(this, constValue);
        }

        public LiteFilter Equal(object constValue)
        {
            return LiteFilter.EqualConst(this, constValue);
        }

        public LiteFilter Equal(string constValue)
        {
            return LiteFilter.EqualConst(this, constValue);
        }

        public LiteFilter Equal(string tableAlias, string fieldName)
        {
            return LiteFilter.EqualField(this, New(tableAlias, fieldName));
        }

        public LiteFilter FullTextContains(object constValue)
        {
            return LiteFilter.FullTextContains(this, constValue);
        }

        public LiteFilter GreaterThan(object constValue)
        {
            return LiteFilter.GreaterThan(this, constValue);
        }

        public LiteFilter GreaterThanOrEqual(object constValue)
        {
            return LiteFilter.GreaterThanOrEqual(this, constValue);
        }

        public LiteFilter IN(LiteQuery subQuery)
        {
            return LiteFilter.FieldIn(this, subQuery);
        }

        public LiteFilter IN(ArrayList constArray)
        {
            return LiteFilter.FieldIn(this, constArray);
        }

        public LiteFilter IN(object value1, params object[] otherValues)
        {
            ArrayList arrayConst = new ArrayList();
            arrayConst.Add(value1);
            arrayConst.AddRange(otherValues);
            return LiteFilter.FieldIn(this, arrayConst);
        }

        public LiteFilter IsNotNull()
        {
            return LiteFilter.IsNotNull(this);
        }

        public LiteFilter IsNull()
        {
            return LiteFilter.IsNull(this);
        }

        public LiteFilter LessThan(object constValue)
        {
            return LiteFilter.LessThan(this, constValue);
        }

        public LiteFilter LessThanOrEqual(object constValue)
        {
            return LiteFilter.LessThanOrEqual(this, constValue);
        }

        public static FilterField New(string tableAlias, string fieldName)
        {
            return new FilterField(tableAlias, fieldName);
        }

        public static FilterField New(string tableAlias, string fieldName, Type dataType)
        {
            return new FilterField(tableAlias, fieldName, dataType);
        }

        public static FilterField New(string tableAlias, int fullTextType, string fieldName, params string[] otherFieldList)
        {
            FilterField field = new FilterField(tableAlias, fieldName)
            {
                FullTextType = (FullTextTypeEnum)fullTextType
            };
            foreach (string str in otherFieldList)
            {
                field.FieldNameList.Add(str);
            }
            return field;
        }

        public LiteFilter NotEqual(object constValue)
        {
            return LiteFilter.NotEqual(this, constValue);
        }

        public LiteFilter StartWith(object constValue)
        {
            return LiteFilter.StartWith(this, constValue);
        }
    }
}
