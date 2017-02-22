namespace King.Framework.LiteQueryDef.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class LiteQuery
    {
        private bool _distinct;
        private readonly List<LiteField> _groupBys;
        public readonly List<LiteOrderField> _orderFields;
        public readonly List<LiteField> SelectFields;
        public LiteTable TableSource;

        public LiteQuery()
        {
            this.SelectFields = new List<LiteField>(20);
            this._groupBys = new List<LiteField>(4);
            this._orderFields = new List<LiteOrderField>(4);
            this._distinct = false;
        }

        public LiteQuery(string tableName, string tableAlias)
        {
            this.SelectFields = new List<LiteField>(20);
            this._groupBys = new List<LiteField>(4);
            this._orderFields = new List<LiteOrderField>(4);
            this._distinct = false;
            this.TableSource = new LiteTable(tableName, tableAlias);
        }

        public void AddField(string fieldAlias)
        {
            this.SelectFields.Add(LiteField.NewConstField("", fieldAlias));
        }

        public void AddField(LiteQuery subQuery, string fieldAlias)
        {
            this.SelectFields.Add(LiteField.NewSubQueryField(subQuery, fieldAlias));
        }

        public void AddField(string tableAlias, string fieldName)
        {
            this.SelectFields.Add(LiteField.NewEntityField(tableAlias, fieldName));
        }

        public void AddField(string tableAlias, string fieldName, string fieldAlias)
        {
            this.SelectFields.Add(LiteField.NewEntityField(tableAlias, fieldName, fieldAlias));
        }

        public void AddFields(string tableAlias, params string[] fieldArray)
        {
            if (fieldArray != null)
            {
                foreach (string str in fieldArray)
                {
                    this.SelectFields.Add(LiteField.NewEntityField(tableAlias, str));
                }
            }
        }

        public LiteQuery CrossJoin(string tableName, string tableAlias)
        {
            LiteJoinTable item = new LiteJoinTable(this, tableName, tableAlias, JoinTypeEnum.CrossJoin);
            this.TableSource.JoinTables.Add(item);
            return this;
        }

        public LiteQuery GroupBy(params LiteField[] fields)
        {
            this.GroupByList.AddRange(fields);
            return this;
        }

        public LiteQuery GroupBy(string tableAlias, string fieldName)
        {
            LiteField item = LiteField.NewEntityField(tableAlias, fieldName);
            this.GroupByList.Add(item);
            return this;
        }

        public LiteJoinTable InnerJoin(string tableName, string tableAlias)
        {
            LiteJoinTable item = new LiteJoinTable(this, tableName, tableAlias, JoinTypeEnum.InnerJoin);
            this.TableSource.JoinTables.Add(item);
            return item;
        }

        public LiteJoinTable LeftOuterJoin(string tableName, string tableAlias)
        {
            LiteJoinTable item = new LiteJoinTable(this, tableName, tableAlias, JoinTypeEnum.LeftOuterJoin);
            this.TableSource.JoinTables.Add(item);
            return item;
        }

        public static LiteQuery New(string tableName, string tableAlias)
        {
            return new LiteQuery(tableName, tableAlias);
        }

        public LiteQuery OrderBy(string tableAlias, string fieldName, OrderMethod orderMethod = OrderMethod.ASC)
        {
            LiteOrderField item = new LiteOrderField(tableAlias, fieldName, orderMethod);
            this.Orders.Add(item);
            return this;
        }

        public LiteJoinTable RightOuterJoin(string tableName, string tableAlias)
        {
            LiteJoinTable item = new LiteJoinTable(this, tableName, tableAlias, JoinTypeEnum.RightOuterJoin);
            this.TableSource.JoinTables.Add(item);
            return item;
        }

        public LiteQuery Select(params LiteField[] fields)
        {
            this.SelectFields.AddRange(fields);
            return this;
        }

        public LiteQuery Select(string tableAlias, params string[] fieldArray)
        {
            this.AddFields(tableAlias, fieldArray);
            return this;
        }

        public LiteQuery Skip(int skipNum)
        {
            this.SkipNumber = new int?(skipNum);
            return this;
        }

        public LiteQuery Take(int takeNum)
        {
            this.TakeNumber = new int?(takeNum);
            return this;
        }

        public LiteQuery Where(LiteFilter filter)
        {
            this.Filter = filter;
            return this;
        }

        public bool Distinct
        {
            get
            {
                return this._distinct;
            }
            set
            {
                this._distinct = value;
            }
        }

        public LiteFilter Filter { get; set; }

        public List<LiteField> GroupByList
        {
            get
            {
                return this._groupBys;
            }
        }

        public List<LiteOrderField> Orders
        {
            get
            {
                return this._orderFields;
            }
        }

        public int? SkipNumber { get; set; }

        public int? TakeNumber { get; set; }
    }
}
