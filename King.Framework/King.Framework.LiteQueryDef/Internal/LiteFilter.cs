namespace King.Framework.LiteQueryDef.Internal
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class LiteFilter
    {
        private int? _realChildCount;
        private int? _realNodeCount;
        public List<LiteFilter> ChildFilters;
        public readonly CompareTypeEnum compareType;
        public readonly FilterTypeEnum filterType;
        public readonly LiteVariable LeftVar;
        public readonly LiteVariable RightVar;

        private LiteFilter(FilterTypeEnum _filterType)
        {
            this.filterType = FilterTypeEnum.AND;
            if (_filterType == FilterTypeEnum.CONDITION)
            {
                throw new ApplicationException();
            }
            this.filterType = _filterType;
            this.ChildFilters = new List<LiteFilter>();
        }

        private LiteFilter(LiteVariable leftVar, CompareTypeEnum _compareType, LiteVariable rightVar)
        {
            this.filterType = FilterTypeEnum.AND;
            this.filterType = FilterTypeEnum.CONDITION;
            this.LeftVar = leftVar;
            this.compareType = _compareType;
            this.RightVar = rightVar;
        }

        public LiteFilter And(LiteFilter childFilter)
        {
            LiteFilter filter = AND();
            filter.ChildFilters.Add(this);
            filter.ChildFilters.Add(childFilter);
            return filter;
        }

        public static LiteFilter AND()
        {
            return new LiteFilter(FilterTypeEnum.AND);
        }

        public static LiteFilter CompareWith(FilterField field, CompareTypeEnum compareType, object constValue)
        {
            LiteVariable leftVar = LiteVariable.FromField(field);
            return new LiteFilter(leftVar, compareType, LiteVariable.FromConst(constValue));
        }

        private static LiteFilter CompareWith(object constValueLeft, CompareTypeEnum compareType, object constValueRight)
        {
            LiteVariable leftVar = LiteVariable.FromConst(constValueLeft);
            return new LiteFilter(leftVar, compareType, LiteVariable.FromConst(constValueRight));
        }

        public static LiteFilter Contains(FilterField field, object constValue)
        {
            return CompareWith(field, CompareTypeEnum.Contains, constValue);
        }

        public static LiteFilter EndWith(FilterField field, object constValue)
        {
            return CompareWith(field, CompareTypeEnum.EndWith, constValue);
        }

        public static LiteFilter EqualConst(FilterField field, object constValue)
        {
            return CompareWith(field, CompareTypeEnum.Equal, constValue);
        }

        public static LiteFilter EqualField(FilterField left, FilterField right)
        {
            LiteVariable leftVar = LiteVariable.FromField(left.TableAlias, left.FieldName);
            return new LiteFilter(leftVar, CompareTypeEnum.Equal, LiteVariable.FromField(right.TableAlias, right.FieldName));
        }

        public static LiteFilter False()
        {
            return CompareWith(1, CompareTypeEnum.Equal, 0);
        }

        public static LiteFilter FieldIn(FilterField field, LiteQuery subQuery)
        {
            LiteVariable leftVar = LiteVariable.FromField(field);
            return new LiteFilter(leftVar, CompareTypeEnum.IN, LiteVariable.FromSubQuery(subQuery));
        }

        public static LiteFilter FieldIn(FilterField field, ArrayList arrayConst)
        {
            LiteVariable leftVar = LiteVariable.FromField(field);
            return new LiteFilter(leftVar, CompareTypeEnum.IN, LiteVariable.FromConstArray(arrayConst));
        }

        public static LiteFilter FullTextContains(FilterField field, object constValue)
        {
            return CompareWith(field, CompareTypeEnum.FullText, constValue);
        }

        internal int GetRealChildCount()
        {
            if (!this._realChildCount.HasValue)
            {
                if (this.filterType == FilterTypeEnum.CONDITION)
                {
                    this._realChildCount = 0;
                }
                else
                {
                    int num = 0;
                    foreach (LiteFilter filter in this.ChildFilters)
                    {
                        num += filter.GetRealNodeCount();
                    }
                    this._realChildCount = new int?(num);
                }
            }
            return this._realChildCount.Value;
        }

        internal int GetRealNodeCount()
        {
            if (!this._realNodeCount.HasValue)
            {
                if (this.filterType == FilterTypeEnum.CONDITION)
                {
                    this._realNodeCount = 1;
                }
                else
                {
                    this._realNodeCount = 0;
                    foreach (LiteFilter filter in this.ChildFilters)
                    {
                        if (filter.GetRealNodeCount() > 0)
                        {
                            this._realNodeCount = 1;
                            break;
                        }
                    }
                }
            }
            return this._realNodeCount.Value;
        }

        public static LiteFilter GreaterThan(FilterField field, object constValue)
        {
            return CompareWith(field, CompareTypeEnum.GreaterThan, constValue);
        }

        public static LiteFilter GreaterThanOrEqual(FilterField field, object constValue)
        {
            return CompareWith(field, CompareTypeEnum.GreaterThanOrEqual, constValue);
        }

        public static LiteFilter IsNotNull(FilterField field)
        {
            return new LiteFilter(LiteVariable.FromField(field), CompareTypeEnum.IsNotNull, null);
        }

        public static LiteFilter IsNull(FilterField field)
        {
            return new LiteFilter(LiteVariable.FromField(field), CompareTypeEnum.IsNull, null);
        }

        public static LiteFilter LessThan(FilterField field, object constValue)
        {
            return CompareWith(field, CompareTypeEnum.LessThan, constValue);
        }

        public static LiteFilter LessThanOrEqual(FilterField field, object constValue)
        {
            return CompareWith(field, CompareTypeEnum.LessThanOrEqual, constValue);
        }

        public static LiteFilter NotEqual(FilterField field, object constValue)
        {
            return CompareWith(field, CompareTypeEnum.NotEqual, constValue);
        }

        public LiteFilter Or(LiteFilter childFilter)
        {
            LiteFilter filter = OR();
            filter.ChildFilters.Add(this);
            filter.ChildFilters.Add(childFilter);
            return filter;
        }

        public static LiteFilter OR()
        {
            return new LiteFilter(FilterTypeEnum.OR);
        }

        public static LiteFilter StartWith(FilterField field, object constValue)
        {
            return CompareWith(field, CompareTypeEnum.StartWith, constValue);
        }

        public static LiteFilter True()
        {
            return CompareWith(1, CompareTypeEnum.Equal, 1);
        }
    }
}
