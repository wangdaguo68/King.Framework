using System.Configuration;

namespace King.Framework.Manager
{
    using King.Framework.Common;
    using King.Framework.EntityLibrary;
    using King.Framework.Interfaces;
    using King.Framework.LiteQueryDef.Internal;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Runtime.CompilerServices;

    public abstract class ViewQueryBase
    {
        protected Dictionary<string, Dictionary<bool, string>> boolDict;
        protected Dictionary<string, Dictionary<int, string>> enumDict;

        public ViewQueryBase(BizDataContext context)
        {
            this.enumDict = new Dictionary<string, Dictionary<int, string>>();
            this.boolDict = new Dictionary<string, Dictionary<bool, string>>();
            this.DataHelper = context;
            this.ViewQueryExs = new List<ViewQueryEx>();
            this.ViewFieldFormatDict = new Dictionary<string, string>();
        }

        public ViewQueryBase(BizDataContext context, IUserIdentity currentUser)
        {
            this.enumDict = new Dictionary<string, Dictionary<int, string>>();
            this.boolDict = new Dictionary<string, Dictionary<bool, string>>();
            this.DataHelper = context;
            this.CurrentUser = currentUser;
            this.CurrentDepartment = context.FindById<T_Department>(new object[] { currentUser.Department_ID });
            this.ViewQueryExs = new List<ViewQueryEx>();
            this.ViewFieldFormatDict = new Dictionary<string, string>();
        }

        protected void AddExtendFilter(LiteQuery q)
        {
            foreach (ViewQueryEx ex in this.ViewQueryExs)
            {
                q.Filter = q.Filter.And(ex.GetFilter());
            }
        }

        protected void AddExtendFilterAndOrder(LiteQuery q)
        {
            this.AddExtendFilter(q);
            this.AddExtendOrder(q);
        }

        protected void AddExtendOrder(LiteQuery q)
        {
            if (!(string.IsNullOrEmpty(this.OrderTableAlias) || string.IsNullOrEmpty(this.OrderFieldName)))
            {
                q.Orders.Clear();
                q.Orders.Add(new LiteOrderField(this.OrderTableAlias, this.OrderFieldName, this.OrderMethod));
            }
            if (q.Orders.Count == 0)
            {
                q.Orders.Add(new LiteOrderField("a", this.KeyName, King.Framework.LiteQueryDef.Internal.OrderMethod.DESC));
            }
        }

        protected void AddFilterForPrevilege(LiteQuery q)
        {
        }

        protected void AddFilterForShared(LiteQuery q)
        {
        }

        protected void ConvertBoolDisplayText(DataRow row, string fieldAlias, string displayFieldAlias)
        {
            if (row[fieldAlias] != DBNull.Value)
            {
                if (!this.boolDict.ContainsKey(fieldAlias))
                {
                    throw new ApplicationException(string.Format("Bool字段[{0}]未设置显示值", fieldAlias));
                }
                bool flag = row[fieldAlias].ToBool();
                row[displayFieldAlias] = this.boolDict[fieldAlias][flag];
            }
        }

        protected void ConvertEnumDisplayText(DataRow row, string fieldAlias, string displayFieldAlias)
        {
            if (row[fieldAlias] != DBNull.Value)
            {
                if (!this.enumDict.ContainsKey(fieldAlias))
                {
                    throw new ApplicationException(string.Format("枚举字段[{0}]未设置显示值", fieldAlias));
                }
                int num = row[fieldAlias].ToInt();
                row[displayFieldAlias] = this.enumDict[fieldAlias][num];
            }
        }

        protected void ConvertMultiRefDisplayText(DataRow row, string fieldAlias, string displayFieldAlias)
        {
            string str = Convert.ToString(row[fieldAlias]);
            if (!string.IsNullOrEmpty(str))
            {
                string[] source = str.Split(new char[] { ',' });
                if (source.Count<string>() == 2)
                {
                    long entityId = source[0].ToLong();
                    int objectId = source[1].ToInt();
                    string displayValue = new OrgManager(this.DataHelper).GetDisplayValue(entityId, objectId);
                    row[displayFieldAlias] = displayValue;
                }
            }
        }

        protected LiteFilter GetFilterForMultiColumns(string tableAlias, string fields, object o)
        {
            List<string> list = (from p in fields.Split(new char[] { ',' })
                where !string.IsNullOrEmpty(p)
                select p).ToList<string>();
            LiteFilter filter = LiteFilter.False();
            foreach (string str in list)
            {
                filter = filter.Or(FilterField.New(tableAlias, str).Contains(o));
            }
            return filter;
        }

        protected LiteFilter GetFilterForMultiValues(string tableAlias, string fieldName, object o)
        {
            List<int> list = (from p in Convert.ToString(o).Split(new char[] { ',' })
                select p.ToIntNullable() into p
                where p.HasValue
                select p.Value).ToList<int>();
            LiteFilter filter = LiteFilter.False();
            foreach (int num in list)
            {
                filter = filter.Or(FilterField.New(tableAlias, fieldName).Equal(num));
            }
            return filter;
        }

        public LiteQuery GetMMSubQuery(string innnerTableName, string leftFieldName, string rightFieldName, object rightValue)
        {
            string tableAlias = string.Empty;
            LiteQuery query = LiteQuery.New(innnerTableName, tableAlias);
            query.AddField(tableAlias, leftFieldName);
            query.Filter = FilterField.New(tableAlias, rightFieldName).Equal(rightValue);
            return query;
        }

        protected LiteQuery GetRefSubQuery(string refTableName, string refKeyName, string refDisplayName, string foreignTableAlias, string foreignName)
        {
            string tableAlias = string.Empty;
            LiteQuery query = LiteQuery.New(refTableName, tableAlias);
            query.AddField(tableAlias, refDisplayName);
            query.Filter = FilterField.New(tableAlias, refKeyName).Equal(foreignTableAlias, foreignName);
            return query;
        }

        protected EntityPrivilegeEnum GetUserMaxQueryPrivilege()
        {
            return EntityPrivilegeEnum.AllRights;
        }

        public abstract DataTable Query();
        public abstract int QueryCount();
        public virtual DataTable QuerySum()
        {
            return new DataTable();
        }

        public virtual void SetQueryConditionValue(string name, object value)
        {
            this.SetPropertyValue(name, value, null);
        }



        public T_Department CurrentDepartment { get; set; }

        public IUserIdentity CurrentUser { get; set; }

        public BizDataContext DataHelper { get; set; }

        public long EntityId { get; set; }

        public string EntityName { get; set; }

        public string KeyName { get; set; }

        public EntityPrivilegeEnum MaxQueryPrivilege { get; set; }

        public string OrderFieldName { get; set; }

        public King.Framework.LiteQueryDef.Internal.OrderMethod OrderMethod { get; set; }

        public string OrderTableAlias { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public Dictionary<string, string> ViewFieldFormatDict { get; set; }

        public List<ViewQueryEx> ViewQueryExs { get; set; }
    }
}
