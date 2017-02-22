namespace King.Framework.LiteQueryDef.Visitors
{
    using King.Framework.LiteQueryDef.CSharpTypes;
    using King.Framework.LiteQueryDef.Internal;
    using System;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    public class OracleVisitor : ILiteVisitor
    {
        private readonly LiteQuery _query;
        private static readonly string ROWNUM = "ROW____NUM";
        private StringBuilder sb = new StringBuilder(0x400);

        public OracleVisitor(LiteQuery query)
        {
            this._query = query;
        }

        private string GetFullTextValueString(string str, FullTextTypeEnum type)
        {
            string input = string.Format("\"{0}\"", str.Trim());
            Regex regex = new Regex(@"\s+");
            string replacement = string.Format("\" {0} \"", type.ToString());
            return regex.Replace(input, replacement);
        }

        [Obsolete]
        private void old_VisitQuery(LiteQuery q)
        {
            int? skipNumber;
            this.sb.Append(" WITH a AS ( SELECT main.*, ROWNUM AS ").Append(ROWNUM);
            this.sb.Append(" FROM (SELECT ");
            this.VisitSelectFields(q);
            this.sb.Append(" FROM ");
            this.VisitTableSource(q.TableSource);
            if ((q.Filter != null) && (q.Filter.GetRealNodeCount() > 0))
            {
                this.sb.Append(" WHERE ");
                this.VisitFilter(q.Filter);
            }
            if (q.GroupByList.Count > 0)
            {
                this.sb.Append(" GROUP BY ");
                this.VisitGroupByList(q);
            }
            if (q.Orders.Count > 0)
            {
                this.sb.Append(" ORDER BY ");
                this.VisitOrderList(q);
            }
            this.sb.Append(") main ) SELECT ");
            int num = -1;
            foreach (LiteField field in q.SelectFields)
            {
                num++;
                if (num > 0)
                {
                    this.sb.Append(",");
                }
                this.sb.Append("a.");
                this.sb.Append(field.FieldAlias);
                this.sb.Append(" ");
            }
            this.sb.Append(" FROM a WHERE a.").Append(ROWNUM).Append(" ");
            if (q.TakeNumber.HasValue && (q.TakeNumber.Value > 0))
            {
                this.sb.Append(" BETWEEN ");
                skipNumber = q.SkipNumber;
                this.sb.Append((int)((skipNumber.HasValue ? skipNumber.GetValueOrDefault() : 0) + 1));
                this.sb.Append(" AND ");
                skipNumber = q.SkipNumber;
                skipNumber = q.TakeNumber;
                this.sb.Append((int)((skipNumber.HasValue ? skipNumber.GetValueOrDefault() : 0) + (skipNumber.HasValue ? skipNumber.GetValueOrDefault() : 0)));
            }
            else
            {
                this.sb.Append(" > ");
                skipNumber = q.SkipNumber;
                this.sb.Append(skipNumber.HasValue ? skipNumber.GetValueOrDefault() : 0);
            }
        }

        private void ViewQuery_Null_Null(LiteQuery q)
        {
            this.sb.Append("SELECT ");
            this.VisitSelectFields(q);
            this.sb.Append(" FROM ");
            this.VisitTableSource(q.TableSource);
            if ((q.Filter != null) && (q.Filter.GetRealNodeCount() > 0))
            {
                this.sb.Append(" WHERE ");
                this.VisitFilter(q.Filter);
            }
            if (q.GroupByList.Count > 0)
            {
                this.sb.Append(" GROUP BY ");
                this.VisitGroupByList(q);
            }
            if (q.Orders.Count > 0)
            {
                this.sb.Append(" ORDER BY ");
                this.VisitOrderList(q);
            }
        }

        public string Visit()
        {
            this.VisitQuery(this._query);
            return this.sb.ToString();
        }

        private void VisitConditionFilter(LiteFilter filter)
        {
            switch (filter.compareType)
            {
                case CompareTypeEnum.LessThan:
                    this.VisitTwoVarFilter(filter, "< ");
                    break;

                case CompareTypeEnum.LessThanOrEqual:
                    this.VisitTwoVarFilter(filter, "<= ");
                    break;

                case CompareTypeEnum.Equal:
                    this.VisitTwoVarFilter(filter, "= ");
                    break;

                case CompareTypeEnum.GreaterThanOrEqual:
                    this.VisitTwoVarFilter(filter, ">= ");
                    break;

                case CompareTypeEnum.GreaterThan:
                    this.VisitTwoVarFilter(filter, "> ");
                    break;

                case CompareTypeEnum.EndWith:
                    this.VisitFilterVariable(filter.LeftVar);
                    if (filter.RightVar.VariableType != VariableTypeEnum.Const)
                    {
                        throw new ApplicationException("EndWith只支持常量");
                    }
                    this.sb.AppendFormat(" LIKE '%{0}' ESCAPE '/' ", filter.RightVar.ConstValue.ToString().Replace("'", "''").Replace("%", "/%").Replace("_", "/_"));
                    break;

                case CompareTypeEnum.StartWith:
                    this.VisitFilterVariable(filter.LeftVar);
                    if (filter.RightVar.VariableType != VariableTypeEnum.Const)
                    {
                        throw new ApplicationException("StartWith只支持常量");
                    }
                    this.sb.AppendFormat(" LIKE '{0}%' ESCAPE '/' ", filter.RightVar.ConstValue.ToString().Replace("'", "''").Replace("%", "/%").Replace("_", "/_"));
                    break;

                case CompareTypeEnum.Contains:
                    this.VisitFilterVariable(filter.LeftVar);
                    if (filter.RightVar.VariableType != VariableTypeEnum.Const)
                    {
                        throw new ApplicationException("Contains只支持常量");
                    }
                    this.sb.AppendFormat(" LIKE '%{0}%' ESCAPE '/' ", filter.RightVar.ConstValue.ToString().Replace("'", "''").Replace("%", "/%").Replace("_", "/_"));
                    break;

                case CompareTypeEnum.NotEqual:
                    this.VisitTwoVarFilter(filter, "<> ");
                    break;

                case CompareTypeEnum.FullText:
                    throw new ApplicationException("Oracle中暂不支持全文检索");

                case CompareTypeEnum.IN:
                    this.VisitTwoVarFilter(filter, "IN ");
                    break;

                case CompareTypeEnum.IsNull:
                    this.VisitFilterVariable(filter.LeftVar);
                    this.sb.Append(" IS NULL ");
                    break;

                case CompareTypeEnum.IsNotNull:
                    this.VisitFilterVariable(filter.LeftVar);
                    this.sb.Append(" IS NOT NULL ");
                    break;
            }
        }

        private void VisitFieldVariable(LiteVariable var)
        {
            Func<string, string> selector = null;
            if (var.IsFullTextSearch)
            {
                if (selector == null)
                {
                    selector = i => string.Format("{0}.{1}", var.TableAlias, i);
                }
                this.sb.Append(string.Join(",", var.FieldNameList.Select<string, string>(selector)));
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(var.TableAlias))
                {
                    this.sb.Append(" ");
                    this.sb.Append(var.TableAlias);
                    this.sb.Append(".");
                }
                this.sb.Append(var.FieldName);
                this.sb.Append(" ");
            }
        }

        private void VisitFilter(LiteFilter filter)
        {
            if ((filter.filterType == FilterTypeEnum.AND) || (filter.filterType == FilterTypeEnum.OR))
            {
                int realChildCount = filter.GetRealChildCount();
                if (realChildCount >= 2)
                {
                    string str = (filter.filterType == FilterTypeEnum.AND) ? " AND " : " OR ";
                    int num2 = -1;
                    foreach (LiteFilter filter2 in filter.ChildFilters)
                    {
                        if (filter2.GetRealNodeCount() > 0)
                        {
                            num2++;
                            if (num2 > 0)
                            {
                                this.sb.Append(str);
                            }
                            this.sb.Append("(");
                            this.VisitFilter(filter2);
                            this.sb.Append(")");
                        }
                    }
                }
                else if (realChildCount == 1)
                {
                    foreach (LiteFilter filter2 in filter.ChildFilters)
                    {
                        if (filter2.GetRealNodeCount() > 0)
                        {
                            this.VisitFilter(filter2);
                        }
                    }
                }
            }
            else
            {
                this.VisitConditionFilter(filter);
            }
        }

        public void VisitFilterField(FilterField field)
        {
            this.sb.Append(" ");
            this.sb.Append(field.TableAlias);
            this.sb.Append(".");
            this.sb.Append(field.FieldName);
            this.sb.Append(" ");
        }

        private void VisitFilterVariable(LiteVariable var)
        {
            if (var.VariableType == VariableTypeEnum.Const)
            {
                if (var.ConstValue == null)
                {
                    throw new ApplicationException("Variable的ConstValue==null");
                }
                string str = SharpTypeHelper.FormatOracle(var.ConstValue);
                this.sb.Append(str);
            }
            else if (var.VariableType == VariableTypeEnum.Array)
            {
                this.sb.Append("(");
                this.sb.Append(string.Join(",", var.ConstListValue.ToArray()));
                this.sb.Append(")");
            }
            else if (var.VariableType == VariableTypeEnum.SubQuery)
            {
                this.sb.Append("(");
                this.VisitSubQuery(var.SubQuery);
                this.sb.Append(")");
            }
            else if (var.VariableType == VariableTypeEnum.Field)
            {
                this.VisitFieldVariable(var);
            }
        }

        private void VisitGroupByList(LiteQuery q)
        {
            int num = -1;
            foreach (LiteField field in q.GroupByList)
            {
                num++;
                if (num > 0)
                {
                    this.sb.Append(", ");
                }
                this.sb.Append(" ");
                this.sb.Append(field.TableAlias);
                this.sb.Append(".");
                this.sb.Append(field.FieldName);
                this.sb.Append(" ");
            }
        }

        public void VisitJoinTable(LiteJoinTable jtable)
        {
            switch (jtable.JoinType)
            {
                case JoinTypeEnum.LeftOuterJoin:
                    this.sb.Append(" LEFT OUTER JOIN ");
                    break;

                case JoinTypeEnum.RightOuterJoin:
                    this.sb.Append(" RIGHT OUTER JOIN ");
                    break;

                case JoinTypeEnum.InnerJoin:
                    this.sb.Append(" INNER JOIN ");
                    break;

                default:
                    this.sb.Append(" CROSS JOIN ");
                    break;
            }
            this.sb.Append(" ");
            this.sb.Append(jtable.tableName);
            this.sb.Append(" ");
            this.sb.Append(jtable.tableAlias);
            this.sb.Append(" ");
            if (jtable.filter != null)
            {
                this.sb.Append(" ON ");
                this.VisitFilter(jtable.filter);
            }
        }

        private void VisitOrderField(LiteOrderField field)
        {
            switch (field.OrderMethod)
            {
                case OrderMethod.ASC:
                case OrderMethod.DESC:
                    this.sb.Append(" ");
                    this.sb.Append(field.TableAlias);
                    this.sb.Append(".");
                    this.sb.Append(field.FieldName);
                    this.sb.Append(" ");
                    this.sb.Append(field.OrderMethod.ToString());
                    break;

                case OrderMethod.ASC_PY:
                    this.sb.AppendFormat("nlssort({0}.{1},'NLS_SORT=SCHINESE_PINYIN_M') asc", field.TableAlias, field.FieldName);
                    break;

                case OrderMethod.DESC_PY:
                    this.sb.AppendFormat("nlssort({0}.{1},'NLS_SORT=SCHINESE_PINYIN_M') desc", field.TableAlias, field.FieldName);
                    break;

                default:
                    throw new Exception("排序方式无法解析");
            }
        }

        private void VisitOrderList(LiteQuery q)
        {
            int num = -1;
            foreach (LiteOrderField field in q.Orders)
            {
                num++;
                if (num > 0)
                {
                    this.sb.Append(", ");
                }
                this.VisitOrderField(field);
            }
        }

        private void VisitQuery(LiteQuery q)
        {
            int? nullable;
            if (q.SkipNumber.HasValue && (((nullable = q.SkipNumber).GetValueOrDefault() > 0) && nullable.HasValue))
            {
                if (q.TakeNumber.HasValue && (((nullable = q.TakeNumber).GetValueOrDefault() > 0) && nullable.HasValue))
                {
                    this.VisitQuery_Skip_Take(q);
                }
                else
                {
                    this.VisitQuery_Skip_Null(q);
                }
            }
            else if (!q.SkipNumber.HasValue || (q.SkipNumber.Value <= 0))
            {
                if (q.TakeNumber.HasValue && (((nullable = q.TakeNumber).GetValueOrDefault() > 0) && nullable.HasValue))
                {
                    this.VisitQuery_Null_Take(q);
                }
                else
                {
                    this.ViewQuery_Null_Null(q);
                }
            }
        }

        private void VisitQuery_Null_Take(LiteQuery q)
        {
            this.sb.Append("SELECT ROWNUM as ").Append(ROWNUM);
            foreach (LiteField field in q.SelectFields)
            {
                this.sb.Append(", a.");
                this.sb.Append(field.FieldAlias);
            }
            this.sb.Append(" FROM (");
            this.ViewQuery_Null_Null(q);
            this.sb.Append(") a ");
            if (q.TakeNumber.HasValue && (q.TakeNumber.Value > 0))
            {
                this.sb.Append("WHERE ROWNUM <= ");
                int? skipNumber = q.SkipNumber;
                skipNumber = q.TakeNumber;
                this.sb.Append((int)((skipNumber.HasValue ? skipNumber.GetValueOrDefault() : 0) + (skipNumber.HasValue ? skipNumber.GetValueOrDefault() : 0)));
            }
        }

        private void VisitQuery_Skip_Null(LiteQuery q)
        {
            this.sb.Append("SELECT ");
            int num = -1;
            foreach (LiteField field in q.SelectFields)
            {
                num++;
                if (num > 0)
                {
                    this.sb.Append(",");
                }
                this.sb.Append(" a.");
                this.sb.Append(field.FieldAlias);
            }
            this.sb.Append(" FROM (");
            this.VisitQuery_Null_Take(q);
            this.sb.Append(") a WHERE a.").Append(ROWNUM).Append(" > ");
            int? skipNumber = q.SkipNumber;
            this.sb.Append(skipNumber.HasValue ? skipNumber.GetValueOrDefault() : 0);
        }

        private void VisitQuery_Skip_Take(LiteQuery q)
        {
            this.sb.Append("SELECT ");
            int num = -1;
            foreach (LiteField field in q.SelectFields)
            {
                num++;
                if (num > 0)
                {
                    this.sb.Append(",");
                }
                this.sb.Append(" a.");
                this.sb.Append(field.FieldAlias);
            }
            this.sb.Append(" FROM (");
            this.VisitQuery_Null_Take(q);
            this.sb.Append(") a WHERE a.").Append(ROWNUM).Append(" > ");
            int? skipNumber = q.SkipNumber;
            this.sb.Append(skipNumber.HasValue ? skipNumber.GetValueOrDefault() : 0);
        }

        private void VisitSelectField(LiteField field)
        {
            if (field.FieldType == LiteFieldTypeEnum.EntityField)
            {
                if (!string.IsNullOrWhiteSpace(field.TableAlias))
                {
                    this.sb.Append(" ");
                    this.sb.Append(field.TableAlias);
                    this.sb.Append(".");
                }
                this.sb.Append(field.FieldName);
                if (field.FieldName != field.FieldAlias)
                {
                    this.sb.Append(" AS ");
                    this.sb.Append(field.FieldAlias);
                }
            }
            else if (field.FieldType == LiteFieldTypeEnum.Function)
            {
                this.sb.Append(field.FunctionName.ToString());
                this.sb.Append("(");
                if (!string.IsNullOrWhiteSpace(field.TableAlias))
                {
                    this.sb.Append(" ");
                    this.sb.Append(field.TableAlias);
                    this.sb.Append(".");
                }
                this.sb.Append(field.FieldName);
                this.sb.Append(" ) AS  ");
                this.sb.Append(field.FieldAlias);
                this.sb.Append(" ");
            }
            else if (field.FieldType == LiteFieldTypeEnum.ConstField)
            {
                string str = SharpTypeHelper.FormatOracle(field.ConstValue);
                this.sb.Append(str);
                this.sb.Append(" AS  ");
                this.sb.Append(field.FieldAlias);
            }
            else
            {
                this.sb.Append("(");
                this.VisitSubQuery(field.SubQuery);
                this.sb.Append(") AS ");
                this.sb.Append(field.FieldAlias);
            }
        }

        public void VisitSelectFields(LiteQuery q)
        {
            int num = -1;
            foreach (LiteField field in q.SelectFields)
            {
                num++;
                if (num > 0)
                {
                    this.sb.Append(",");
                }
                this.VisitSelectField(field);
            }
        }

        private void VisitSubQuery(LiteQuery q)
        {
            this.sb.Append(" SELECT ");
            this.VisitSelectFields(q);
            this.sb.Append(" FROM");
            this.VisitTableSource(q.TableSource);
            if ((q.Filter != null) && (q.Filter.GetRealNodeCount() > 0))
            {
                this.sb.Append(" WHERE ");
                this.VisitFilter(q.Filter);
            }
            if (q.GroupByList.Count > 0)
            {
                this.sb.Append(" GROUP BY ");
                this.VisitGroupByList(q);
            }
            if (q.Orders.Count > 0)
            {
                this.sb.Append(" ORDER BY ");
                this.VisitOrderList(q);
            }
        }

        public void VisitTableSource(LiteTable source)
        {
            this.sb.Append(" ");
            this.sb.Append(source.TableName);
            this.sb.Append(" ");
            if (!string.IsNullOrWhiteSpace(source.TableAlias))
            {
                this.sb.Append(" ");
                this.sb.Append(source.TableAlias);
                this.sb.Append(" ");
            }
            if (source.JoinTables.Count > 0)
            {
                foreach (LiteJoinTable table in source.JoinTables)
                {
                    this.VisitJoinTable(table);
                }
            }
        }

        private void VisitTwoVarFilter(LiteFilter filter, string compareOP)
        {
            this.VisitFilterVariable(filter.LeftVar);
            this.sb.Append(compareOP);
            this.VisitFilterVariable(filter.RightVar);
        }
    }
}
