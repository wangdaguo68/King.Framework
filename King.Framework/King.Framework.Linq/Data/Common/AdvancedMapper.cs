namespace King.Framework.Linq.Data.Common
{
    using King.Framework.Linq;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public class AdvancedMapper : BasicMapper
    {
        protected AdvancedMapping mapping;

        public AdvancedMapper(AdvancedMapping mapping, QueryTranslator translator) : base(mapping, translator)
        {
            this.mapping = mapping;
        }

        private IEnumerable<ColumnAssignment> GetColumnAssignments(Expression table, Expression instance, MappingEntity entity, Func<MappingEntity, MemberInfo, bool> fnIncludeColumn, Dictionary<MemberInfo, Expression> map)
        {
            foreach (MemberInfo iteratorVariable0 in this.mapping.GetMappedMembers(entity))
            {
                if (!(!this.mapping.IsColumn(entity, iteratorVariable0) ? true : !fnIncludeColumn(entity, iteratorVariable0)))
                {
                    yield return new ColumnAssignment((ColumnExpression)this.GetMemberExpression(table, entity, iteratorVariable0), this.GetMemberAccess(instance, iteratorVariable0, map));
                    continue;
                }
                if (this.mapping.IsNestedEntity(entity, iteratorVariable0))
                {
                    IEnumerable<ColumnAssignment> iteratorVariable1 = this.GetColumnAssignments(table, Expression.MakeMemberAccess(instance, iteratorVariable0), this.mapping.GetRelatedEntity(entity, iteratorVariable0), fnIncludeColumn, map);
                    foreach (ColumnAssignment iteratorVariable2 in iteratorVariable1)
                    {
                        yield return iteratorVariable2;
                    }
                }
            }
        }

        private void GetColumns(MappingEntity entity, Dictionary<string, TableAlias> aliases, List<ColumnDeclaration> columns)
        {
            foreach (MemberInfo info in this.mapping.GetMappedMembers(entity))
            {
                if (!this.mapping.IsAssociationRelationship(entity, info))
                {
                    if (this.mapping.IsNestedEntity(entity, info))
                    {
                        this.GetColumns(this.mapping.GetRelatedEntity(entity, info), aliases, columns);
                    }
                    else if (this.mapping.IsColumn(entity, info))
                    {
                        TableAlias alias;
                        string columnName = this.mapping.GetColumnName(entity, info);
                        string key = this.mapping.GetAlias(entity, info);
                        aliases.TryGetValue(key, out alias);
                        QueryType columnType = this.GetColumnType(entity, info);
                        ColumnExpression expression = new ColumnExpression(King.Framework.Linq.TypeHelper.GetMemberType(info), columnType, alias, columnName);
                        ColumnDeclaration item = new ColumnDeclaration(columnName, expression, columnType);
                        columns.Add(item);
                    }
                }
            }
        }

        public override Expression GetDeleteExpression(MappingEntity entity, Expression instance, LambdaExpression deleteCheck)
        {
            if (this.mapping.GetTables(entity).Count < 2)
            {
                return base.GetDeleteExpression(entity, instance, deleteCheck);
            }
            List<Expression> commands = new List<Expression>();
            foreach (MappingTable table in this.GetDependencyOrderedTables(entity).Reverse<MappingTable>())
            {
                TableExpression root = new TableExpression(new TableAlias(), entity, this.mapping.GetTableName(table));
                Expression where = this.GetIdentityCheck(root, entity, instance);
                commands.Add(new DeleteCommand(root, where));
            }
            Expression ifTrue = new BlockCommand(commands);
            if (deleteCheck != null)
            {
                return new IFCommand(this.GetEntityStateTest(entity, instance, deleteCheck), ifTrue, null);
            }
            return ifTrue;
        }

        public virtual IEnumerable<MappingTable> GetDependencyOrderedTables(MappingEntity entity)
        {
            ILookup<string, MappingTable> lookup = this.mapping.GetTables(entity).ToLookup<MappingTable, string>(t => this.mapping.GetAlias(t));
            return this.mapping.GetTables(entity).Sort<MappingTable>(t => (this.mapping.IsExtensionTable(t) ? lookup[this.mapping.GetExtensionRelatedAlias(t)] : null));
        }

        private Dictionary<string, List<MemberInfo>> GetDependentGeneratedColumns(MappingEntity entity)
        {
            return (from xt in this.mapping.GetTables(entity)
                    where this.mapping.IsExtensionTable(xt)
                    group xt by this.mapping.GetExtensionRelatedAlias(xt)).
                    ToDictionary(g => g.Key,g =>((from xt in g
                                                 select this.mapping.GetExtensionRelatedMembers(xt)) as IEnumerable<MemberInfo>).Distinct().ToList());
        }

        private CommandExpression GetDependentGeneratedVariableDeclaration(MappingEntity entity, MappingTable table, List<MemberInfo> members, Expression instance, Dictionary<MemberInfo, Expression> map)
        {
            Func<MemberInfo, int, Expression> selector = null;
            DeclarationCommand command = null;
            List<MemberInfo> second = (from m in this.mapping.GetMappedMembers(entity)
                                       where this.mapping.IsPrimaryKey(entity, m) && this.mapping.IsGenerated(entity, m)
                                       select m).ToList<MemberInfo>();
            if (second.Count > 0)
            {
                command = this.GetGeneratedIdCommand(entity, members, map);
                if (members.Count == second.Count)
                {
                    return command;
                }
            }
            members = members.Except<MemberInfo>(second).ToList<MemberInfo>();
            TableAlias alias = new TableAlias();
            TableExpression tex = new TableExpression(alias, entity, this.mapping.GetTableName(table));
            Expression where = null;
            if (second.Count > 0)
            {
                if (selector == null)
                {
                    selector = (m, i) => this.GetMemberExpression(tex, entity, m).Equal(map[m]);
                }
                where = second.Select<MemberInfo, Expression>(selector).Aggregate<Expression>((x, y) => x.And(y));
            }
            else
            {
                where = this.GetIdentityCheck(tex, entity, instance);
            }
            TableAlias alias2 = new TableAlias();
            List<ColumnDeclaration> columns = new List<ColumnDeclaration>();
            List<VariableDeclaration> variables = new List<VariableDeclaration>();
            foreach (MemberInfo info in members)
            {
                ColumnExpression expression = (ColumnExpression)this.GetMemberExpression(tex, entity, info);
                columns.Add(new ColumnDeclaration(this.mapping.GetColumnName(entity, info), expression, expression.QueryType));
                ColumnExpression expression3 = new ColumnExpression(expression.Type, expression.QueryType, alias2, expression.Name);
                variables.Add(new VariableDeclaration(info.Name, expression.QueryType, expression3));
                map.Add(info, new VariableExpression(info.Name, expression.Type, expression.QueryType));
            }
            DeclarationCommand command2 = new DeclarationCommand(variables, new SelectExpression(alias2, columns, tex, where));
            if (command != null)
            {
                return new BlockCommand(new Expression[] { command, command2 });
            }
            return command2;
        }

        public override EntityExpression GetEntityExpression(Expression root, MappingEntity entity)
        {
            List<EntityAssignment> assignments = new List<EntityAssignment>();
            foreach (MemberInfo info in this.mapping.GetMappedMembers(entity))
            {
                if (!this.mapping.IsAssociationRelationship(entity, info))
                {
                    Expression entityExpression;
                    if (this.mapping.IsNestedEntity(entity, info))
                    {
                        entityExpression = this.GetEntityExpression(root, this.mapping.GetRelatedEntity(entity, info));
                    }
                    else
                    {
                        entityExpression = this.GetMemberExpression(root, entity, info);
                    }
                    if (entityExpression != null)
                    {
                        assignments.Add(new EntityAssignment(info, entityExpression));
                    }
                }
            }
            return new EntityExpression(entity, this.BuildEntityExpression(entity, assignments));
        }

        private Expression GetIdentityCheck(TableExpression root, MappingEntity entity, Expression instance, MappingTable table)
        {
            if (this.mapping.IsExtensionTable(table))
            {
                string[] strArray = this.mapping.GetExtensionKeyColumnNames(table).ToArray<string>();
                MemberInfo[] infoArray = this.mapping.GetExtensionRelatedMembers(table).ToArray<MemberInfo>();
                Expression expression = null;
                int index = 0;
                int length = strArray.Length;
                while (index < length)
                {
                    MemberInfo mi = infoArray[index];
                    ColumnExpression expression2 = new ColumnExpression(King.Framework.Linq.TypeHelper.GetMemberType(mi), this.GetColumnType(entity, mi), root.Alias, strArray[length]);
                    Expression expression3 = this.GetMemberExpression(instance, entity, mi);
                    Expression expression4 = expression2.Equal(expression3);
                    expression = (expression != null) ? expression.And(expression4) : expression;
                    index++;
                }
                return expression;
            }
            return base.GetIdentityCheck(root, entity, instance);
        }

        public override Expression GetInsertExpression(MappingEntity entity, Expression instance, LambdaExpression selector)
        {
            if (this.mapping.GetTables(entity).Count < 2)
            {
                return base.GetInsertExpression(entity, instance, selector);
            }
            List<Expression> commands = new List<Expression>();
            Dictionary<string, List<MemberInfo>> dependentGeneratedColumns = this.GetDependentGeneratedColumns(entity);
            Dictionary<MemberInfo, Expression> map = new Dictionary<MemberInfo, Expression>();
            using (IEnumerator<MappingTable> enumerator = this.GetDependencyOrderedTables(entity).GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    List<MemberInfo> list3;
                    Func<MappingEntity, MemberInfo, bool> fnIncludeColumn = null;
                    MappingTable table = enumerator.Current;
                    TableAlias alias = new TableAlias();
                    TableExpression expression = new TableExpression(alias, entity, this.mapping.GetTableName(table));
                    if (fnIncludeColumn == null)
                    {
                        fnIncludeColumn = (e, m) => (this.mapping.GetAlias(e, m) == this.mapping.GetAlias(table)) && !this.mapping.IsGenerated(e, m);
                    }
                    IEnumerable<ColumnAssignment> assignments = this.GetColumnAssignments(expression, instance, entity, fnIncludeColumn, map).Concat<ColumnAssignment>(this.GetRelatedColumnAssignments(expression, entity, table, map));
                    commands.Add(new InsertCommand(expression, assignments));
                    if (dependentGeneratedColumns.TryGetValue(this.mapping.GetAlias(table), out list3))
                    {
                        CommandExpression item = this.GetDependentGeneratedVariableDeclaration(entity, table, list3, instance, map);
                        commands.Add(item);
                    }
                }
            }
            if (selector != null)
            {
                commands.Add(this.GetInsertResult(entity, instance, selector, map));
            }
            return new BlockCommand(commands);
        }

        private Expression GetMemberAccess(Expression instance, MemberInfo member, Dictionary<MemberInfo, Expression> map)
        {
            Expression expression;
            if (!((map != null) && map.TryGetValue(member, out expression)))
            {
                expression = Expression.MakeMemberAccess(instance, member);
            }
            return expression;
        }

        public override Expression GetMemberExpression(Expression root, MappingEntity entity, MemberInfo member)
        {
            if (this.mapping.IsNestedEntity(entity, member))
            {
                MappingEntity relatedEntity = this.mapping.GetRelatedEntity(entity, member);
                return this.GetEntityExpression(root, relatedEntity);
            }
            return base.GetMemberExpression(root, entity, member);
        }

        public override ProjectionExpression GetQueryExpression(MappingEntity entity)
        {
            IList<MappingTable> tables = this.mapping.GetTables(entity);
            if (tables.Count <= 1)
            {
                return base.GetQueryExpression(entity);
            }
            Dictionary<string, TableAlias> aliases = new Dictionary<string, TableAlias>();
            MappingTable table = tables.Single<MappingTable>(ta => !this.mapping.IsExtensionTable(ta));
            TableExpression expression = new TableExpression(new TableAlias(), entity, this.mapping.GetTableName(table));
            aliases.Add(this.mapping.GetAlias(table), expression.Alias);
            Expression left = expression;
            foreach (MappingTable table2 in from t in tables
                                            where this.mapping.IsExtensionTable(t)
                                            select t)
            {
                TableAlias alias2;
                TableAlias alias = new TableAlias();
                string key = this.mapping.GetAlias(table2);
                aliases.Add(key, alias);
                List<string> list2 = this.mapping.GetExtensionKeyColumnNames(table2).ToList<string>();
                List<MemberInfo> list3 = this.mapping.GetExtensionRelatedMembers(table2).ToList<MemberInfo>();
                string extensionRelatedAlias = this.mapping.GetExtensionRelatedAlias(table2);
                aliases.TryGetValue(extensionRelatedAlias, out alias2);
                TableExpression right = new TableExpression(alias, entity, this.mapping.GetTableName(table2));
                Expression expression4 = null;
                int num = 0;
                int count = list2.Count;
                while (num < count)
                {
                    Type memberType = King.Framework.Linq.TypeHelper.GetMemberType(list3[num]);
                    QueryType columnType = this.GetColumnType(entity, list3[num]);
                    ColumnExpression expression5 = new ColumnExpression(memberType, columnType, alias2, this.mapping.GetColumnName(entity, list3[num]));
                    Expression expression7 = new ColumnExpression(memberType, columnType, alias, list2[num]).Equal(expression5);
                    expression4 = (expression4 != null) ? expression4.And(expression7) : expression7;
                    num++;
                }
                left = new JoinExpression(JoinType.SingletonLeftOuter, left, right, expression4);
            }
            List<ColumnDeclaration> list4 = new List<ColumnDeclaration>();
            this.GetColumns(entity, aliases, list4);
            SelectExpression root = new SelectExpression(new TableAlias(), list4, left, null);
            Expression entityExpression = this.GetEntityExpression(root, entity);
            TableAlias newAlias = new TableAlias();
            ProjectedColumns columns = ColumnProjector.ProjectColumns(this.Translator.Linguist.Language, entityExpression, null, newAlias, new TableAlias[] { root.Alias });
            ProjectionExpression expression10 = new ProjectionExpression(new SelectExpression(newAlias, columns.Columns, root, null), columns.Projector);
            return (ProjectionExpression)this.Translator.Police.ApplyPolicy(expression10, entity.ElementType);
        }

        private IEnumerable<ColumnAssignment> GetRelatedColumnAssignments(Expression expr, MappingEntity entity, MappingTable table, Dictionary<MemberInfo, Expression> map)
        {
            if (this.mapping.IsExtensionTable(table))
            {
                string[] iteratorVariable0 = this.mapping.GetExtensionKeyColumnNames(table).ToArray<string>();
                MemberInfo[] iteratorVariable1 = this.mapping.GetExtensionRelatedMembers(table).ToArray<MemberInfo>();
                int index = 0;
                int length = iteratorVariable0.Length;
                while (index < length)
                {
                    MemberInfo member = iteratorVariable1[index];
                    Expression expression = map[member];
                    yield return new ColumnAssignment((ColumnExpression)this.GetMemberExpression(expr, entity, member), expression);
                    index++;
                }
            }
        }

        public override Expression GetUpdateExpression(MappingEntity entity, Expression instance, LambdaExpression updateCheck, LambdaExpression selector, Expression @else)
        {
            if (this.mapping.GetTables(entity).Count < 2)
            {
                return base.GetUpdateExpression(entity, instance, updateCheck, selector, @else);
            }
            List<Expression> commands = new List<Expression>();
            using (IEnumerator<MappingTable> enumerator = this.GetDependencyOrderedTables(entity).GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Func<MappingEntity, MemberInfo, bool> fnIncludeColumn = null;
                    MappingTable table = enumerator.Current;
                    TableExpression expression = new TableExpression(new TableAlias(), entity, this.mapping.GetTableName(table));
                    if (fnIncludeColumn == null)
                    {
                        fnIncludeColumn = (e, m) => (this.mapping.GetAlias(e, m) == this.mapping.GetAlias(table)) && this.mapping.IsUpdatable(e, m);
                    }
                    IEnumerable<ColumnAssignment> assignments = this.GetColumnAssignments(expression, instance, entity, fnIncludeColumn, null);
                    Expression where = this.GetIdentityCheck(expression, entity, instance);
                    commands.Add(new UpdateCommand(expression, where, assignments));
                }
            }
            if (selector != null)
            {
                commands.Add(new IFCommand(this.Translator.Linguist.Language.GetRowsAffectedExpression(commands[commands.Count - 1]).GreaterThan(Expression.Constant(0)), this.GetUpdateResult(entity, instance, selector), @else));
            }
            else if (@else != null)
            {
                commands.Add(new IFCommand(this.Translator.Linguist.Language.GetRowsAffectedExpression(commands[commands.Count - 1]).LessThanOrEqual(Expression.Constant(0)), @else, null));
            }
            Expression ifTrue = new BlockCommand(commands);
            if (updateCheck != null)
            {
                return new IFCommand(this.GetEntityStateTest(entity, instance, updateCheck), ifTrue, null);
            }
            return ifTrue;
        }




    }
}
