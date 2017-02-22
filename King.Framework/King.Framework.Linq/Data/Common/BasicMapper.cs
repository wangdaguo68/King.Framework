namespace King.Framework.Linq.Data.Common
{
    using King.Framework.Linq;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public class BasicMapper : QueryMapper
    {
        private BasicMapping mapping;
        private QueryTranslator translator;

        public BasicMapper(BasicMapping mapping, QueryTranslator translator)
        {
            this.mapping = mapping;
            this.translator = translator;
        }

        protected virtual ConstructorBindResult BindConstructor(ConstructorInfo cons, IList<EntityAssignment> assignments)
        {
            ParameterInfo[] parameters = cons.GetParameters();
            Expression[] arguments = new Expression[parameters.Length];
            MemberInfo[] members = new MemberInfo[parameters.Length];
            HashSet<EntityAssignment> source = new HashSet<EntityAssignment>(assignments);
            HashSet<EntityAssignment> other = new HashSet<EntityAssignment>();
            int index = 0;
            int length = parameters.Length;
            while (index < length)
            {
                Func<EntityAssignment, bool> predicate = null;
                ParameterInfo p = parameters[index];
                EntityAssignment item = source.FirstOrDefault<EntityAssignment>(a => (p.Name == a.Member.Name) && p.ParameterType.IsAssignableFrom(a.Expression.Type));
                if (item == null)
                {
                    if (predicate == null)
                    {
                        predicate = a => (string.Compare(p.Name, a.Member.Name, true) == 0) && p.ParameterType.IsAssignableFrom(a.Expression.Type);
                    }
                    item = source.FirstOrDefault<EntityAssignment>(predicate);
                }
                if (item != null)
                {
                    arguments[index] = item.Expression;
                    if (members != null)
                    {
                        members[index] = item.Member;
                    }
                    other.Add(item);
                }
                else
                {
                    MemberInfo[] member = cons.DeclaringType.GetMember(p.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if ((member != null) && (member.Length > 0))
                    {
                        arguments[index] = Expression.Constant(King.Framework.Linq.TypeHelper.GetDefault(p.ParameterType), p.ParameterType);
                        members[index] = member[0];
                    }
                    else
                    {
                        return null;
                    }
                }
                index++;
            }
            source.ExceptWith(other);
            return new ConstructorBindResult(Expression.New(cons, arguments, members), source);
        }

        protected virtual Expression BuildEntityExpression(MappingEntity entity, IList<EntityAssignment> assignments)
        {
            NewExpression expression;
            Expression expression2;
            Func<ConstructorInfo, ConstructorBindResult> selector = null;
            EntityAssignment[] readonlyMembers = (from b in assignments
                                                  where King.Framework.Linq.TypeHelper.IsReadOnly(b.Member)
                                                  select b).ToArray<EntityAssignment>();
            ConstructorInfo[] constructors = entity.EntityType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            bool flag = constructors.Any<ConstructorInfo>(c => c.GetParameters().Length == 0);
            if ((readonlyMembers.Length > 0) || !flag)
            {
                if (selector == null)
                {
                    selector = c => this.BindConstructor(c, readonlyMembers);
                }
                List<ConstructorBindResult> list = (from cbr in constructors.Select<ConstructorInfo, ConstructorBindResult>(selector)
                                                    where (cbr != null) && (cbr.Remaining.Count == 0)
                                                    select cbr).ToList<ConstructorBindResult>();
                if (list.Count == 0)
                {
                    throw new InvalidOperationException(string.Format("Cannot construct type '{0}' with all mapped includedMembers.", entity.ElementType));
                }
                if (readonlyMembers.Length == assignments.Count)
                {
                    return list[0].Expression;
                }
                ConstructorBindResult result = this.BindConstructor(list[0].Expression.Constructor, assignments);
                expression = result.Expression;
                assignments = result.Remaining;
            }
            else
            {
                expression = Expression.New(entity.EntityType);
            }
            if (assignments.Count > 0)
            {
                if (entity.ElementType.IsInterface)
                {
                    assignments = this.MapAssignments(assignments, entity.EntityType).ToList<EntityAssignment>();
                }
                expression2 = Expression.MemberInit(expression, (MemberBinding[])(from a in assignments select Expression.Bind(a.Member, a.Expression)).ToArray<MemberAssignment>());
            }
            else
            {
                expression2 = expression;
            }
            if (entity.ElementType != entity.EntityType)
            {
                expression2 = Expression.Convert(expression2, entity.ElementType);
            }
            return expression2;
        }

        private IEnumerable<EntityAssignment> GetAssignments(Expression newOrMemberInit)
        {
            Func<int, bool> predicate = null;
            Func<int, EntityAssignment> selector = null;
            List<EntityAssignment> list = new List<EntityAssignment>();
            MemberInitExpression expression = newOrMemberInit as MemberInitExpression;
            if (expression != null)
            {
                list.AddRange(from a in expression.Bindings.OfType<MemberAssignment>() select new EntityAssignment(a.Member, a.Expression));
                newOrMemberInit = expression.NewExpression;
            }
            NewExpression nex = newOrMemberInit as NewExpression;
            if ((nex != null) && (nex.Members != null))
            {
                if (predicate == null)
                {
                    predicate = i => nex.Members[i] != null;
                }
                if (selector == null)
                {
                    selector = i => new EntityAssignment(nex.Members[i], nex.Arguments[i]);
                }
                list.AddRange(Enumerable.Range(0, nex.Arguments.Count).Where<int>(predicate).Select<int, EntityAssignment>(selector));
            }
            return list;
        }

        private IEnumerable<ColumnAssignment> GetColumnAssignments(Expression table, Expression instance, MappingEntity entity, Func<MappingEntity, MemberInfo, bool> fnIncludeColumn)
        {
            foreach (MemberInfo iteratorVariable0 in this.mapping.GetMappedMembers(entity))
            {
                if (!this.mapping.IsColumn(entity, iteratorVariable0) ? true : !fnIncludeColumn(entity, iteratorVariable0))
                {
                    continue;
                }
                yield return new ColumnAssignment((ColumnExpression)this.GetMemberExpression(table, entity, iteratorVariable0), Expression.MakeMemberAccess(instance, iteratorVariable0));
            }
        }

        public virtual QueryType GetColumnType(MappingEntity entity, MemberInfo member)
        {
            string columnDbType = this.mapping.GetColumnDbType(entity, member);
            if (columnDbType != null)
            {
                return this.translator.Linguist.Language.TypeSystem.Parse(columnDbType);
            }
            return this.translator.Linguist.Language.TypeSystem.GetColumnType(King.Framework.Linq.TypeHelper.GetMemberType(member));
        }

        public override Expression GetDeleteExpression(MappingEntity entity, Expression instance, LambdaExpression deleteCheck)
        {
            TableExpression root = new TableExpression(new TableAlias(), entity, this.mapping.GetTableName(entity));
            Expression expression2 = null;
            if (instance != null)
            {
                expression2 = this.GetIdentityCheck(root, entity, instance);
            }
            if (deleteCheck != null)
            {
                Expression entityExpression = this.GetEntityExpression(root, entity);
                Expression expression4 = DbExpressionReplacer.Replace(deleteCheck.Body, deleteCheck.Parameters[0], entityExpression);
                expression2 = (expression2 != null) ? expression2.And(expression4) : expression4;
            }
            return new DeleteCommand(root, expression2);
        }

        protected virtual Expression GetEntityExistsTest(MappingEntity entity, Expression instance)
        {
            ProjectionExpression queryExpression = this.GetQueryExpression(entity);
            Expression where = this.GetIdentityCheck(queryExpression.Select, entity, instance);
            return new ExistsExpression(new SelectExpression(new TableAlias(), null, queryExpression.Select, where));
        }

        public override EntityExpression GetEntityExpression(Expression root, MappingEntity entity)
        {
            List<EntityAssignment> assignments = new List<EntityAssignment>();
            foreach (MemberInfo info in this.mapping.GetMappedMembers(entity))
            {
                if (!this.mapping.IsAssociationRelationship(entity, info))
                {
                    Expression expression = this.GetMemberExpression(root, entity, info);
                    if (expression != null)
                    {
                        assignments.Add(new EntityAssignment(info, expression));
                    }
                }
            }
            return new EntityExpression(entity, this.BuildEntityExpression(entity, assignments));
        }

        protected virtual Expression GetEntityStateTest(MappingEntity entity, Expression instance, LambdaExpression updateCheck)
        {
            ProjectionExpression queryExpression = this.GetQueryExpression(entity);
            Expression expression2 = this.GetIdentityCheck(queryExpression.Select, entity, instance);
            Expression expression3 = DbExpressionReplacer.Replace(updateCheck.Body, updateCheck.Parameters[0], queryExpression.Projector);
            expression2 = expression2.And(expression3);
            return new ExistsExpression(new SelectExpression(new TableAlias(), null, queryExpression.Select, expression2));
        }

        protected virtual DeclarationCommand GetGeneratedIdCommand(MappingEntity entity, List<MemberInfo> members, Dictionary<MemberInfo, Expression> map)
        {
            List<ColumnDeclaration> columns = new List<ColumnDeclaration>();
            List<VariableDeclaration> variables = new List<VariableDeclaration>();
            TableAlias alias = new TableAlias();
            foreach (MemberInfo info in members)
            {
                Expression generatedIdExpression = this.translator.Linguist.Language.GetGeneratedIdExpression(info);
                string name = info.Name;
                QueryType columnType = this.GetColumnType(entity, info);
                columns.Add(new ColumnDeclaration(info.Name, generatedIdExpression, columnType));
                variables.Add(new VariableDeclaration(info.Name, columnType, new ColumnExpression(generatedIdExpression.Type, columnType, alias, info.Name)));
                if (map != null)
                {
                    VariableExpression expression2 = new VariableExpression(info.Name, King.Framework.Linq.TypeHelper.GetMemberType(info), columnType);
                    map.Add(info, expression2);
                }
            }
            return new DeclarationCommand(variables, new SelectExpression(alias, columns, null, null));
        }

        protected virtual Expression GetIdentityCheck(Expression root, MappingEntity entity, Expression instance)
        {
            return (from m in this.mapping.GetMappedMembers(entity)
                    where this.mapping.IsPrimaryKey(entity, m)
                    select this.GetMemberExpression(root, entity, m).Equal(Expression.MakeMemberAccess(instance, m))).Aggregate<Expression>((x, y) => x.And(y));
        }

        public override Expression GetInsertExpression(MappingEntity entity, Expression instance, LambdaExpression selector)
        {
            TableAlias alias = new TableAlias();
            TableExpression table = new TableExpression(alias, entity, this.mapping.GetTableName(entity));
            IEnumerable<ColumnAssignment> assignments = this.GetColumnAssignments(table, instance, entity, (e, m) => !this.mapping.IsGenerated(e, m));
            if (selector != null)
            {
                return new BlockCommand(new Expression[] { new InsertCommand(table, assignments), this.GetInsertResult(entity, instance, selector, null) });
            }
            return new InsertCommand(table, assignments);
        }

        public override Expression GetInsertOrUpdateExpression(MappingEntity entity, Expression instance, LambdaExpression updateCheck, LambdaExpression resultSelector)
        {
            Expression expression;
            if (updateCheck != null)
            {
                expression = this.GetInsertExpression(entity, instance, resultSelector);
                return new IFCommand(this.GetEntityExistsTest(entity, instance), this.GetUpdateExpression(entity, instance, updateCheck, resultSelector, null), expression);
            }
            expression = this.GetInsertExpression(entity, instance, resultSelector);
            return this.GetUpdateExpression(entity, instance, updateCheck, resultSelector, expression);
        }

        protected virtual Expression GetInsertResult(MappingEntity entity, Expression instance, LambdaExpression selector, Dictionary<MemberInfo, Expression> map)
        {
            Expression expression2;
            Func<MemberInfo, bool> func = null;
            Func<MemberInfo, int, Expression> func2 = null;
            TableAlias alias = new TableAlias();
            TableExpression tex = new TableExpression(alias, entity, this.mapping.GetTableName(entity));
            LambdaExpression aggregator = Aggregator.GetAggregator(selector.Body.Type, typeof(IEnumerable<>).MakeGenericType(new Type[] { selector.Body.Type }));
            DeclarationCommand command = null;
            List<MemberInfo> source = (from m in this.mapping.GetMappedMembers(entity)
                                       where this.mapping.IsPrimaryKey(entity, m) && this.mapping.IsGenerated(entity, m)
                                       select m).ToList<MemberInfo>();
            if (source.Count > 0)
            {
                if (map != null)
                {
                }
                if (!((func == null) && source.Any<MemberInfo>((func = m => map.ContainsKey(m)))))
                {
                    Dictionary<MemberInfo, Expression> dictionary = new Dictionary<MemberInfo, Expression>();
                    command = this.GetGeneratedIdCommand(entity, source.ToList<MemberInfo>(), dictionary);
                    map = dictionary;
                }
                MemberExpression body = selector.Body as MemberExpression;
                if (((body != null) && this.mapping.IsPrimaryKey(entity, body.Member)) && this.mapping.IsGenerated(entity, body.Member))
                {
                    if (command != null)
                    {
                        return new ProjectionExpression(command.Source, new ColumnExpression(body.Type, command.Variables[0].QueryType, command.Source.Alias, command.Source.Columns[0].Name), aggregator);
                    }
                    TableAlias alias2 = new TableAlias();
                    QueryType columnType = this.GetColumnType(entity, body.Member);
                    return new ProjectionExpression(new SelectExpression(alias2, new ColumnDeclaration[] { new ColumnDeclaration("", map[body.Member], columnType) }, null, null), new ColumnExpression(King.Framework.Linq.TypeHelper.GetMemberType(body.Member), columnType, alias2, ""), aggregator);
                }
                if (func2 == null)
                {
                    func2 = (m, i) => this.GetMemberExpression(tex, entity, m).Equal(map[m]);
                }
                expression2 = source.Select<MemberInfo, Expression>(func2).Aggregate<Expression>((x, y) => x.And(y));
            }
            else
            {
                expression2 = this.GetIdentityCheck(tex, entity, instance);
            }
            Expression entityExpression = this.GetEntityExpression(tex, entity);
            Expression expression = DbExpressionReplacer.Replace(selector.Body, selector.Parameters[0], entityExpression);
            TableAlias newAlias = new TableAlias();
            ProjectedColumns columns = ColumnProjector.ProjectColumns(this.translator.Linguist.Language, expression, null, newAlias, new TableAlias[] { alias });
            ProjectionExpression expression6 = new ProjectionExpression(new SelectExpression(newAlias, columns.Columns, tex, expression2), columns.Projector, aggregator);
            if (command != null)
            {
                return new BlockCommand(new Expression[] { command, expression6 });
            }
            return expression6;
        }

        public override Expression GetMemberExpression(Expression root, MappingEntity entity, MemberInfo member)
        {
            if (this.mapping.IsAssociationRelationship(entity, member))
            {
                MappingEntity relatedEntity = this.mapping.GetRelatedEntity(entity, member);
                ProjectionExpression queryExpression = this.GetQueryExpression(relatedEntity);
                List<MemberInfo> list = this.mapping.GetAssociationKeyMembers(entity, member).ToList<MemberInfo>();
                List<MemberInfo> list2 = this.mapping.GetAssociationRelatedKeyMembers(entity, member).ToList<MemberInfo>();
                Expression expression2 = null;
                int num = 0;
                int count = list2.Count;
                while (num < count)
                {
                    Expression expression3 = this.GetMemberExpression(queryExpression.Projector, relatedEntity, list2[num]).Equal(this.GetMemberExpression(root, entity, list[num]));
                    expression2 = (expression2 != null) ? expression2.And(expression3) : expression3;
                    num++;
                }
                TableAlias newAlias = new TableAlias();
                ProjectedColumns columns = ColumnProjector.ProjectColumns(this.translator.Linguist.Language, queryExpression.Projector, null, newAlias, new TableAlias[] { queryExpression.Select.Alias });
                LambdaExpression aggregator = Aggregator.GetAggregator(King.Framework.Linq.TypeHelper.GetMemberType(member), typeof(IEnumerable<>).MakeGenericType(new Type[] { columns.Projector.Type }));
                ProjectionExpression expression = new ProjectionExpression(new SelectExpression(newAlias, columns.Columns, queryExpression.Select, expression2), columns.Projector, aggregator);
                return this.translator.Police.ApplyPolicy(expression, member);
            }
            AliasedExpression expression6 = root as AliasedExpression;
            if ((expression6 != null) && this.mapping.IsColumn(entity, member))
            {
                return new ColumnExpression(King.Framework.Linq.TypeHelper.GetMemberType(member), this.GetColumnType(entity, member), expression6.Alias, this.mapping.GetColumnName(entity, member));
            }
            return QueryBinder.BindMember(root, member);
        }

        public override ProjectionExpression GetQueryExpression(MappingEntity entity)
        {
            TableAlias alias = new TableAlias();
            TableAlias newAlias = new TableAlias();
            TableExpression root = new TableExpression(alias, entity, this.mapping.GetTableName(entity));
            Expression entityExpression = this.GetEntityExpression(root, entity);
            ProjectedColumns columns = ColumnProjector.ProjectColumns(this.translator.Linguist.Language, entityExpression, null, newAlias, new TableAlias[] { alias });
            ProjectionExpression expression = new ProjectionExpression(new SelectExpression(newAlias, columns.Columns, root, null), columns.Projector);
            return (ProjectionExpression)this.Translator.Police.ApplyPolicy(expression, entity.ElementType);
        }

        public override Expression GetUpdateExpression(MappingEntity entity, Expression instance, LambdaExpression updateCheck, LambdaExpression selector, Expression @else)
        {
            TableAlias alias = new TableAlias();
            TableExpression root = new TableExpression(alias, entity, this.mapping.GetTableName(entity));
            Expression expression2 = this.GetIdentityCheck(root, entity, instance);
            if (updateCheck != null)
            {
                Expression entityExpression = this.GetEntityExpression(root, entity);
                Expression expression4 = DbExpressionReplacer.Replace(updateCheck.Body, updateCheck.Parameters[0], entityExpression);
                expression2 = expression2.And(expression4);
            }
            IEnumerable<ColumnAssignment> assignments = this.GetColumnAssignments(root, instance, entity, (e, m) => this.mapping.IsUpdatable(e, m));
            Expression command = new UpdateCommand(root, expression2, assignments);
            if (selector != null)
            {
                return new BlockCommand(new Expression[] { command, new IFCommand(this.translator.Linguist.Language.GetRowsAffectedExpression(command).GreaterThan(Expression.Constant(0)), this.GetUpdateResult(entity, instance, selector), @else) });
            }
            if (@else != null)
            {
                return new BlockCommand(new Expression[] { command, new IFCommand(this.translator.Linguist.Language.GetRowsAffectedExpression(command).LessThanOrEqual(Expression.Constant(0)), @else, null) });
            }
            return command;
        }

        protected virtual Expression GetUpdateResult(MappingEntity entity, Expression instance, LambdaExpression selector)
        {
            ProjectionExpression queryExpression = this.GetQueryExpression(entity);
            Expression where = this.GetIdentityCheck(queryExpression.Select, entity, instance);
            Expression expression = DbExpressionReplacer.Replace(selector.Body, selector.Parameters[0], queryExpression.Projector);
            TableAlias newAlias = new TableAlias();
            ProjectedColumns columns = ColumnProjector.ProjectColumns(this.translator.Linguist.Language, expression, null, newAlias, new TableAlias[] { queryExpression.Select.Alias });
            return new ProjectionExpression(new SelectExpression(newAlias, columns.Columns, queryExpression.Select, where), columns.Projector, Aggregator.GetAggregator(selector.Body.Type, typeof(IEnumerable<>).MakeGenericType(new Type[] { selector.Body.Type })));
        }

        public override bool HasIncludedMembers(EntityExpression entity)
        {
            QueryPolicy policy = this.translator.Police.Policy;
            foreach (MemberInfo info in this.mapping.GetMappedMembers(entity.Entity))
            {
                if (policy.IsIncluded(info))
                {
                    return true;
                }
            }
            return false;
        }

        public override EntityExpression IncludeMembers(EntityExpression entity, Func<MemberInfo, bool> fnIsIncluded)
        {
            Dictionary<string, EntityAssignment> dictionary = this.GetAssignments(entity.Expression).ToDictionary<EntityAssignment, string>(ma => ma.Member.Name);
            bool flag = false;
            foreach (MemberInfo info in this.mapping.GetMappedMembers(entity.Entity))
            {
                EntityAssignment assignment;
                if (!(!(!dictionary.TryGetValue(info.Name, out assignment) || this.IsNullRelationshipAssignment(entity.Entity, assignment)) ? true : !fnIsIncluded(info)))
                {
                    assignment = new EntityAssignment(info, this.GetMemberExpression(entity.Expression, entity.Entity, info));
                    dictionary[info.Name] = assignment;
                    flag = true;
                }
            }
            if (flag)
            {
                return new EntityExpression(entity.Entity, this.BuildEntityExpression(entity.Entity, dictionary.Values.ToList<EntityAssignment>()));
            }
            return entity;
        }

        private bool IsNullRelationshipAssignment(MappingEntity entity, EntityAssignment assignment)
        {
            if (this.mapping.IsRelationship(entity, assignment.Member))
            {
                ConstantExpression expression = assignment.Expression as ConstantExpression;
                if ((expression != null) && (expression.Value == null))
                {
                    return true;
                }
            }
            return false;
        }

        private IEnumerable<EntityAssignment> MapAssignments(IEnumerable<EntityAssignment> assignments, Type entityType)
        {
            foreach (EntityAssignment iteratorVariable0 in assignments)
            {
                MemberInfo[] member = entityType.GetMember(iteratorVariable0.Member.Name, BindingFlags.Public | BindingFlags.Instance);
                if ((member != null) && (member.Length > 0))
                {
                    yield return new EntityAssignment(member[0], iteratorVariable0.Expression);
                    continue;
                }
                yield return iteratorVariable0;
            }
        }

        public override QueryMapping Mapping
        {
            get
            {
                return this.mapping;
            }
        }

        public override QueryTranslator Translator
        {
            get
            {
                return this.translator;
            }
        }

        
}

public class ConstructorBindResult
{
    public ConstructorBindResult(NewExpression expression, IEnumerable<EntityAssignment> remaining)
    {
        this.Expression = expression;
        this.Remaining = remaining.ToReadOnly<EntityAssignment>();
    }

    public NewExpression Expression { get; private set; }

    public ReadOnlyCollection<EntityAssignment> Remaining { get; private set; }
}

public class EntityAssignment
{
    public EntityAssignment(MemberInfo member, System.Linq.Expressions.Expression expression)
    {
        this.Member = member;
        Debug.Assert(expression != null);
        this.Expression = expression;
    }

    public System.Linq.Expressions.Expression Expression { get; private set; }

    public MemberInfo Member { get; private set; }
}
    }

