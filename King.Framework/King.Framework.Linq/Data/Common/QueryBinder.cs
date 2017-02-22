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

    public class QueryBinder : DbExpressionVisitor
    {
        private IEntityTable batchUpd;
        private Expression currentGroupElement;
        internal readonly Dictionary<Expression, GroupByInfo> groupByMap;
        internal readonly QueryLanguage language;
        protected Dictionary<ParameterExpression, Expression> map;
        internal readonly QueryMapper mapper;
        protected Expression root;
        private List<OrderExpression> thenBys;

        private QueryBinder(QueryMapper mapper, Expression root)
        {
            this.mapper = mapper;
            this.language = mapper.Translator.Linguist.Language;
            this.map = new Dictionary<ParameterExpression, Expression>();
            this.groupByMap = new Dictionary<Expression, GroupByInfo>();
            this.root = root;
        }

        internal QueryBinder(QueryMapper mapper, Expression root, Dictionary<ParameterExpression, Expression> map, Dictionary<Expression, GroupByInfo> groupByMap)
        {
            this.mapper = mapper;
            this.language = mapper.Translator.Linguist.Language;
            this.map = map;
            this.groupByMap = groupByMap;
            this.root = root;
        }

        public static Expression Bind(QueryMapper mapper, Expression expression)
        {
            return new QueryBinder(mapper, expression).Visit(expression);
        }

        protected Expression BindAggregate(Expression source, string aggName, Type returnType, LambdaExpression argument, bool isRoot)
        {
            GroupByInfo info;
            bool flag = this.language.AggregateArgumentIsPredicate(aggName);
            bool isDistinct = false;
            bool flag3 = false;
            bool flag4 = false;
            MethodCallExpression expression = source as MethodCallExpression;
            if ((((expression != null) && !flag) && (argument == null)) && ((((expression.Method.Name == "Distinct") && (expression.Arguments.Count == 1)) && ((expression.Method.DeclaringType == typeof(Queryable)) || (expression.Method.DeclaringType == typeof(Enumerable)))) && this.language.AllowDistinctInAggregates))
            {
                source = expression.Arguments[0];
                isDistinct = true;
            }
            if ((argument != null) && flag)
            {
                source = Expression.Call(typeof(Queryable), "Where", new Type[] { King.Framework.Linq.TypeHelper.GetElementType(source.Type) }, new Expression[] { source, argument });
                argument = null;
                flag3 = true;
            }
            ProjectionExpression key = this.VisitSequence(source);
            Expression projector = null;
            if (argument != null)
            {
                this.map[argument.Parameters[0]] = key.Projector;
                projector = this.Visit(argument.Body);
            }
            else if (!(flag && !flag4))
            {
                projector = key.Projector;
            }
            TableAlias nextAlias = this.GetNextAlias();
            Expression expression4 = new AggregateExpression(returnType, aggName, projector, isDistinct);
            QueryType columnType = this.language.TypeSystem.GetColumnType(returnType);
            SelectExpression expression5 = new SelectExpression(nextAlias, new ColumnDeclaration[] { new ColumnDeclaration("", expression4, columnType) }, key.Select, null);
            if (isRoot)
            {
                ParameterExpression expression6 = Expression.Parameter(typeof(IEnumerable<>).MakeGenericType(new Type[] { expression4.Type }), "p");
                return new ProjectionExpression(expression5, new ColumnExpression(returnType, this.language.TypeSystem.GetColumnType(returnType), nextAlias, ""), Expression.Lambda(Expression.Call(typeof(Enumerable), "Single", new Type[] { returnType }, new Expression[] { expression6 }), new ParameterExpression[] { expression6 }));
            }
            ScalarExpression aggregateAsSubquery = new ScalarExpression(returnType, expression5);
            if (flag3 || !this.groupByMap.TryGetValue(key, out info))
            {
                return aggregateAsSubquery;
            }
            if (argument != null)
            {
                this.map[argument.Parameters[0]] = info.Element;
                projector = this.Visit(argument.Body);
            }
            else if (!(flag && !flag4))
            {
                projector = info.Element;
            }
            expression4 = new AggregateExpression(returnType, aggName, projector, isDistinct);
            if (key == this.currentGroupElement)
            {
                return expression4;
            }
            return new AggregateSubqueryExpression(info.Alias, expression4, aggregateAsSubquery);
        }

        private Expression BindAnyAll(Expression source, MethodInfo method, LambdaExpression predicate, bool isRoot)
        {
            bool flag = method.Name == "All";
            ConstantExpression expression = source as ConstantExpression;
            if ((expression != null) && !this.IsQuery(expression))
            {
                Debug.Assert(!isRoot);
                Expression expression2 = null;
                foreach (object obj2 in (IEnumerable) expression.Value)
                {
                    Expression expression3 = Expression.Invoke(predicate, new Expression[] { Expression.Constant(obj2, predicate.Parameters[0].Type) });
                    if (expression2 == null)
                    {
                        expression2 = expression3;
                    }
                    else if (flag)
                    {
                        expression2 = expression2.And(expression3);
                    }
                    else
                    {
                        expression2 = expression2.Or(expression3);
                    }
                }
                return this.Visit(expression2);
            }
            if (flag)
            {
                predicate = Expression.Lambda(Expression.Not(predicate.Body), predicate.Parameters.ToArray<ParameterExpression>());
            }
            if (predicate != null)
            {
                source = Expression.Call(typeof(Enumerable), "Where", method.GetGenericArguments(), new Expression[] { source, predicate });
            }
            ProjectionExpression expression4 = this.VisitSequence(source);
            Expression expression5 = new ExistsExpression(expression4.Select);
            if (flag)
            {
                expression5 = Expression.Not(expression5);
            }
            if (isRoot)
            {
                if (this.language.AllowSubqueryInSelectWithoutFrom)
                {
                    return this.GetSingletonSequence(expression5, "SingleOrDefault");
                }
                QueryType columnType = this.language.TypeSystem.GetColumnType(typeof(int));
                SelectExpression expression6 = expression4.Select.SetColumns(new ColumnDeclaration[] { new ColumnDeclaration("value", new AggregateExpression(typeof(int), "Count", null, false), columnType) });
                ColumnExpression expression7 = new ColumnExpression(typeof(int), columnType, expression6.Alias, "value");
                return new ProjectionExpression(expression6, flag ? expression7.Equal(Expression.Constant(0)) : expression7.GreaterThan(Expression.Constant(0)), Aggregator.GetAggregator(typeof(bool), typeof(IEnumerable<bool>)));
            }
            return expression5;
        }

        private Expression BindBatch(IEntityTable upd, Expression instances, LambdaExpression operation, Expression batchSize, Expression stream)
        {
            IEntityTable batchUpd = this.batchUpd;
            this.batchUpd = upd;
            LambdaExpression expression = (LambdaExpression) this.Visit(operation);
            this.batchUpd = batchUpd;
            Expression input = this.Visit(instances);
            Expression expression3 = this.Visit(batchSize);
            return new BatchExpression(input, expression, expression3, this.Visit(stream));
        }

        private Expression BindCast(Expression source, Type targetElementType)
        {
            ProjectionExpression expression = this.VisitSequence(source);
            Type trueUnderlyingType = this.GetTrueUnderlyingType(expression.Projector);
            if (!targetElementType.IsAssignableFrom(trueUnderlyingType))
            {
                throw new InvalidOperationException(string.Format("Cannot cast elements from type '{0}' to type '{1}'", trueUnderlyingType, targetElementType));
            }
            return expression;
        }

        private Expression BindContains(Expression source, Expression match, bool isRoot)
        {
            ConstantExpression expression = source as ConstantExpression;
            if ((expression != null) && !this.IsQuery(expression))
            {
                Debug.Assert(!isRoot);
                List<Expression> values = new List<Expression>();
                foreach (object obj2 in (IEnumerable) expression.Value)
                {
                    values.Add(Expression.Constant(Convert.ChangeType(obj2, match.Type), match.Type));
                }
                match = this.Visit(match);
                return new InExpression(match, values);
            }
            if (!(!isRoot || this.language.AllowSubqueryInSelectWithoutFrom))
            {
                ParameterExpression expression2 = Expression.Parameter(King.Framework.Linq.TypeHelper.GetElementType(source.Type), "x");
                LambdaExpression expression3 = Expression.Lambda(expression2.Equal(match), new ParameterExpression[] { expression2 });
                MethodCallExpression exp = Expression.Call(typeof(Queryable), "Any", new Type[] { expression2.Type }, new Expression[] { source, expression3 });
                this.root = exp;
                return this.Visit(exp);
            }
            ProjectionExpression expression5 = this.VisitSequence(source);
            match = this.Visit(match);
            Expression expr = new InExpression(match, expression5.Select);
            if (isRoot)
            {
                return this.GetSingletonSequence(expr, "SingleOrDefault");
            }
            return expr;
        }

        private Expression BindDelete(IEntityTable upd, Expression instance, LambdaExpression deleteCheck)
        {
            MappingEntity entity = this.mapper.Mapping.GetEntity((instance != null) ? instance.Type : deleteCheck.Parameters[0].Type, upd.TableId);
            return this.Visit(this.mapper.GetDeleteExpression(entity, instance, deleteCheck));
        }

        private Expression BindDistinct(Expression source)
        {
            ProjectionExpression expression = this.VisitSequence(source);
            SelectExpression select = expression.Select;
            TableAlias nextAlias = this.GetNextAlias();
            ProjectedColumns columns = this.ProjectColumns(expression.Projector, nextAlias, new TableAlias[] { expression.Select.Alias });
            return new ProjectionExpression(new SelectExpression(nextAlias, columns.Columns, expression.Select, null, null, null, true, null, null, false), columns.Projector);
        }

        private Expression BindFirst(Expression source, LambdaExpression predicate, string kind, bool isRoot)
        {
            ProjectionExpression expression = this.VisitSequence(source);
            Expression where = null;
            if (predicate != null)
            {
                this.map[predicate.Parameters[0]] = expression.Projector;
                where = this.Visit(predicate.Body);
            }
            bool flag = kind.StartsWith("First");
            bool reverse = kind.StartsWith("Last");
            Expression take = (flag || reverse) ? Expression.Constant(1) : null;
            if ((take != null) || (where != null))
            {
                TableAlias nextAlias = this.GetNextAlias();
                ProjectedColumns columns = this.ProjectColumns(expression.Projector, nextAlias, new TableAlias[] { expression.Select.Alias });
                expression = new ProjectionExpression(new SelectExpression(nextAlias, columns.Columns, expression.Select, where, null, null, false, null, take, reverse), columns.Projector);
            }
            if (isRoot)
            {
                Type type = expression.Projector.Type;
                ParameterExpression expression4 = Expression.Parameter(typeof(IEnumerable<>).MakeGenericType(new Type[] { type }), "p");
                return new ProjectionExpression(expression.Select, expression.Projector, Expression.Lambda(Expression.Call(typeof(Enumerable), kind, new Type[] { type }, new Expression[] { expression4 }), new ParameterExpression[] { expression4 }));
            }
            return expression;
        }

        protected virtual Expression BindGroupBy(Expression source, LambdaExpression keySelector, LambdaExpression elementSelector, LambdaExpression resultSelector)
        {
            Expression expression9;
            ProjectionExpression expression = this.VisitSequence(source);
            this.map[keySelector.Parameters[0]] = expression.Projector;
            Expression expression2 = this.Visit(keySelector.Body);
            Expression projector = expression.Projector;
            if (elementSelector != null)
            {
                this.map[elementSelector.Parameters[0]] = expression.Projector;
                projector = this.Visit(elementSelector.Body);
            }
            ProjectedColumns columns = this.ProjectColumns(expression2, expression.Select.Alias, new TableAlias[] { expression.Select.Alias });
            Expression[] expressionArray = (from c in columns.Columns select c.Expression).ToArray<Expression>();
            ProjectionExpression expression4 = this.VisitSequence(source);
            this.map[keySelector.Parameters[0]] = expression4.Projector;
            Expression expression5 = this.Visit(keySelector.Body);
            Expression[] expressionArray2 = (from c in this.ProjectColumns(expression5, expression4.Select.Alias, new TableAlias[] { expression4.Select.Alias }).Columns select c.Expression).ToArray<Expression>();
            Expression where = this.BuildPredicateWithNullsEqual(expressionArray2, expressionArray);
            Expression expression7 = expression4.Projector;
            if (elementSelector != null)
            {
                this.map[elementSelector.Parameters[0]] = expression4.Projector;
                expression7 = this.Visit(elementSelector.Body);
            }
            TableAlias nextAlias = this.GetNextAlias();
            ProjectedColumns columns3 = this.ProjectColumns(expression7, nextAlias, new TableAlias[] { expression4.Select.Alias });
            ProjectionExpression key = new ProjectionExpression(new SelectExpression(nextAlias, columns3.Columns, expression4.Select, where), columns3.Projector);
            TableAlias alias = this.GetNextAlias();
            GroupByInfo info = new GroupByInfo(alias, projector);
            this.groupByMap.Add(key, info);
            if (resultSelector != null)
            {
                Expression currentGroupElement = this.currentGroupElement;
                this.currentGroupElement = key;
                this.map[resultSelector.Parameters[0]] = columns.Projector;
                this.map[resultSelector.Parameters[1]] = key;
                expression9 = this.Visit(resultSelector.Body);
                this.currentGroupElement = currentGroupElement;
            }
            else
            {
                expression9 = Expression.Convert(Expression.New(typeof(King.Framework.Linq.Grouping<,>).MakeGenericType(new Type[] { expression2.Type, expression7.Type }).GetConstructors()[0], new Expression[] { expression2, key }), typeof(IGrouping<,>).MakeGenericType(new Type[] { expression2.Type, expression7.Type }));
            }
            ProjectedColumns columns4 = this.ProjectColumns(expression9, alias, new TableAlias[] { expression.Select.Alias });
            NewExpression newExpression = this.GetNewExpression(columns4.Projector);
            if (((newExpression != null) && newExpression.Type.IsGenericType) && (newExpression.Type.GetGenericTypeDefinition() == typeof(King.Framework.Linq.Grouping<,>)))
            {
                Expression expression12 = newExpression.Arguments[1];
                this.groupByMap.Add(expression12, info);
            }
            return new ProjectionExpression(new SelectExpression(alias, columns4.Columns, expression.Select, null, null, expressionArray), columns4.Projector);
        }

        protected virtual Expression BindGroupJoin(MethodInfo groupJoinMethod, Expression outerSource, Expression innerSource, LambdaExpression outerKey, LambdaExpression innerKey, LambdaExpression resultSelector)
        {
            Type[] genericArguments = groupJoinMethod.GetGenericArguments();
            ProjectionExpression expression = this.VisitSequence(outerSource);
            this.map[outerKey.Parameters[0]] = expression.Projector;
            LambdaExpression expression2 = Expression.Lambda(innerKey.Body.Equal(outerKey.Body), new ParameterExpression[] { innerKey.Parameters[0] });
            MethodCallExpression exp = Expression.Call(typeof(Enumerable), "Where", new Type[] { genericArguments[1] }, new Expression[] { innerSource, expression2 });
            Expression expression4 = this.Visit(exp);
            this.map[resultSelector.Parameters[0]] = expression.Projector;
            this.map[resultSelector.Parameters[1]] = expression4;
            Expression expression5 = this.Visit(resultSelector.Body);
            TableAlias nextAlias = this.GetNextAlias();
            ProjectedColumns columns = this.ProjectColumns(expression5, nextAlias, new TableAlias[] { expression.Select.Alias });
            return new ProjectionExpression(new SelectExpression(nextAlias, columns.Columns, expression.Select, null), columns.Projector);
        }

        private Expression BindInsert(IEntityTable upd, Expression instance, LambdaExpression selector)
        {
            MappingEntity entity = this.mapper.Mapping.GetEntity(instance.Type, upd.TableId);
            return this.Visit(this.mapper.GetInsertExpression(entity, instance, selector));
        }

        private Expression BindInsertOrUpdate(IEntityTable upd, Expression instance, LambdaExpression updateCheck, LambdaExpression resultSelector)
        {
            MappingEntity entity = this.mapper.Mapping.GetEntity(instance.Type, upd.TableId);
            return this.Visit(this.mapper.GetInsertOrUpdateExpression(entity, instance, updateCheck, resultSelector));
        }

        protected virtual Expression BindIntersect(Expression outerSource, Expression innerSource, bool negate)
        {
            ProjectionExpression expression = this.VisitSequence(outerSource);
            ProjectionExpression expression2 = this.VisitSequence(innerSource);
            Expression expression3 = new ExistsExpression(new SelectExpression(new TableAlias(), null, expression2.Select, expression2.Projector.Equal(expression.Projector)));
            if (negate)
            {
                expression3 = Expression.Not(expression3);
            }
            TableAlias nextAlias = this.GetNextAlias();
            ProjectedColumns columns = this.ProjectColumns(expression.Projector, nextAlias, new TableAlias[] { expression.Select.Alias });
            return new ProjectionExpression(new SelectExpression(nextAlias, columns.Columns, expression.Select, expression3), columns.Projector, expression.Aggregator);
        }

        protected virtual Expression BindJoin(Type resultType, Expression outerSource, Expression innerSource, LambdaExpression outerKey, LambdaExpression innerKey, LambdaExpression resultSelector)
        {
            ProjectionExpression expression = this.VisitSequence(outerSource);
            ProjectionExpression expression2 = this.VisitSequence(innerSource);
            this.map[outerKey.Parameters[0]] = expression.Projector;
            Expression expression3 = this.Visit(outerKey.Body);
            this.map[innerKey.Parameters[0]] = expression2.Projector;
            Expression expression4 = this.Visit(innerKey.Body);
            this.map[resultSelector.Parameters[0]] = expression.Projector;
            this.map[resultSelector.Parameters[1]] = expression2.Projector;
            Expression expression5 = this.Visit(resultSelector.Body);
            JoinExpression from = new JoinExpression(JoinType.InnerJoin, expression.Select, expression2.Select, expression3.Equal(expression4));
            TableAlias nextAlias = this.GetNextAlias();
            ProjectedColumns columns = this.ProjectColumns(expression5, nextAlias, new TableAlias[] { expression.Select.Alias, expression2.Select.Alias });
            return new ProjectionExpression(new SelectExpression(nextAlias, columns.Columns, from, null), columns.Projector);
        }

        public static Expression BindMember(Expression source, MemberInfo member)
        {
            int num;
            int count;
            switch (source.NodeType)
            {
                case ((ExpressionType) 0x3ec):
                {
                    ProjectionExpression expression7 = (ProjectionExpression) source;
                    Expression projector = BindMember(expression7.Projector, member);
                    Type memberType = King.Framework.Linq.TypeHelper.GetMemberType(member);
                    return new ProjectionExpression(expression7.Select, projector, Aggregator.GetAggregator(memberType, typeof(IEnumerable<>).MakeGenericType(new Type[] { memberType })));
                }
                case ((ExpressionType) 0x3ed):
                {
                    EntityExpression expression = (EntityExpression) source;
                    Expression expression2 = BindMember(expression.Expression, member);
                    MemberExpression expression3 = expression2 as MemberExpression;
                    if (((expression3 == null) || (expression3.Expression != expression.Expression)) || !(expression3.Member == member))
                    {
                        return expression2;
                    }
                    return Expression.MakeMemberAccess(source, member);
                }
                case ((ExpressionType) 0x3f9):
                {
                    OuterJoinedExpression expression9 = (OuterJoinedExpression) source;
                    Expression expression10 = BindMember(expression9.Expression, member);
                    if (expression10 is ColumnExpression)
                    {
                        return expression10;
                    }
                    return new OuterJoinedExpression(expression9.Test, expression10);
                }
                case ExpressionType.New:
                {
                    NewExpression expression6 = (NewExpression) source;
                    if (expression6.Members == null)
                    {
                        if ((expression6.Type.IsGenericType && (expression6.Type.GetGenericTypeDefinition() == typeof(King.Framework.Linq.Grouping<,>))) && (member.Name == "Key"))
                        {
                            return expression6.Arguments[0];
                        }
                        break;
                    }
                    num = 0;
                    count = expression6.Members.Count;
                    while (num < count)
                    {
                        if (MembersMatch(expression6.Members[num], member))
                        {
                            return expression6.Arguments[num];
                        }
                        num++;
                    }
                    break;
                }
                case ExpressionType.Conditional:
                {
                    ConditionalExpression expression11 = (ConditionalExpression) source;
                    return Expression.Condition(expression11.Test, BindMember(expression11.IfTrue, member), BindMember(expression11.IfFalse, member));
                }
                case ExpressionType.Constant:
                {
                    ConstantExpression expression12 = (ConstantExpression) source;
                    Type type = King.Framework.Linq.TypeHelper.GetMemberType(member);
                    if (expression12.Value != null)
                    {
                        return Expression.Constant(GetValue(expression12.Value, member), type);
                    }
                    return Expression.Constant(GetDefault(type), type);
                }
                case ExpressionType.Convert:
                {
                    UnaryExpression expression4 = (UnaryExpression) source;
                    return BindMember(expression4.Operand, member);
                }
                case ExpressionType.MemberInit:
                {
                    MemberInitExpression expression5 = (MemberInitExpression) source;
                    num = 0;
                    count = expression5.Bindings.Count;
                    while (num < count)
                    {
                        MemberAssignment assignment = expression5.Bindings[num] as MemberAssignment;
                        if ((assignment != null) && MembersMatch(assignment.Member, member))
                        {
                            return assignment.Expression;
                        }
                        num++;
                    }
                    break;
                }
            }
            return Expression.MakeMemberAccess(source, member);
        }

        protected virtual Expression BindOrderBy(Type resultType, Expression source, LambdaExpression orderSelector, OrderType orderType)
        {
            List<OrderExpression> thenBys = this.thenBys;
            this.thenBys = null;
            ProjectionExpression expression = this.VisitSequence(source);
            this.map[orderSelector.Parameters[0]] = expression.Projector;
            List<OrderExpression> list2 = new List<OrderExpression> {
                new OrderExpression(orderType, this.Visit(orderSelector.Body))
            };
            if (thenBys != null)
            {
                for (int i = thenBys.Count - 1; i >= 0; i--)
                {
                    OrderExpression expression2 = thenBys[i];
                    LambdaExpression expression3 = (LambdaExpression) expression2.Expression;
                    this.map[expression3.Parameters[0]] = expression.Projector;
                    list2.Add(new OrderExpression(expression2.OrderType, this.Visit(expression3.Body)));
                }
            }
            TableAlias nextAlias = this.GetNextAlias();
            ProjectedColumns columns = this.ProjectColumns(expression.Projector, nextAlias, new TableAlias[] { expression.Select.Alias });
            return new ProjectionExpression(new SelectExpression(nextAlias, columns.Columns, expression.Select, null, list2.AsReadOnly(), null), columns.Projector);
        }

        private Expression BindRelationshipProperty(MemberExpression mex)
        {
            EntityExpression expression = mex.Expression as EntityExpression;
            if ((expression != null) && this.mapper.Mapping.IsRelationship(expression.Entity, mex.Member))
            {
                return this.mapper.GetMemberExpression(mex.Expression, expression.Entity, mex.Member);
            }
            return mex;
        }

        private Expression BindReverse(Expression source)
        {
            ProjectionExpression expression = this.VisitSequence(source);
            TableAlias nextAlias = this.GetNextAlias();
            ProjectedColumns columns = this.ProjectColumns(expression.Projector, nextAlias, new TableAlias[] { expression.Select.Alias });
            return new ProjectionExpression(new SelectExpression(nextAlias, columns.Columns, expression.Select, null).SetReverse(true), columns.Projector);
        }

        private Expression BindSelect(Type resultType, Expression source, LambdaExpression selector)
        {
            ProjectionExpression expression = this.VisitSequence(source);
            this.map[selector.Parameters[0]] = expression.Projector;
            Expression expression2 = this.Visit(selector.Body);
            TableAlias nextAlias = this.GetNextAlias();
            ProjectedColumns columns = this.ProjectColumns(expression2, nextAlias, new TableAlias[] { expression.Select.Alias });
            return new ProjectionExpression(new SelectExpression(nextAlias, columns.Columns, expression.Select, null), columns.Projector);
        }

        protected virtual Expression BindSelectMany(Type resultType, Expression source, LambdaExpression collectionSelector, LambdaExpression resultSelector)
        {
            ProjectedColumns columns;
            ProjectionExpression expression = this.VisitSequence(source);
            this.map[collectionSelector.Parameters[0]] = expression.Projector;
            Expression body = collectionSelector.Body;
            bool flag = false;
            MethodCallExpression expression3 = body as MethodCallExpression;
            if ((((expression3 != null) && (expression3.Method.Name == "DefaultIfEmpty")) && (expression3.Arguments.Count == 1)) && ((expression3.Method.DeclaringType == typeof(Queryable)) || (expression3.Method.DeclaringType == typeof(Enumerable))))
            {
                body = expression3.Arguments[0];
                flag = true;
            }
            ProjectionExpression proj = this.VisitSequence(body);
            JoinType joinType = (proj.Select.From is TableExpression) ? JoinType.CrossJoin : (flag ? JoinType.OuterApply : JoinType.CrossApply);
            if (joinType == JoinType.OuterApply)
            {
                proj = this.language.AddOuterJoinTest(proj);
            }
            JoinExpression from = new JoinExpression(joinType, expression.Select, proj.Select, null);
            TableAlias nextAlias = this.GetNextAlias();
            if (resultSelector == null)
            {
                columns = this.ProjectColumns(proj.Projector, nextAlias, new TableAlias[] { expression.Select.Alias, proj.Select.Alias });
            }
            else
            {
                this.map[resultSelector.Parameters[0]] = expression.Projector;
                this.map[resultSelector.Parameters[1]] = proj.Projector;
                Expression expression6 = this.Visit(resultSelector.Body);
                columns = this.ProjectColumns(expression6, nextAlias, new TableAlias[] { expression.Select.Alias, proj.Select.Alias });
            }
            return new ProjectionExpression(new SelectExpression(nextAlias, columns.Columns, from, null), columns.Projector);
        }

        private Expression BindSkip(Expression source, Expression skip)
        {
            ProjectionExpression expression = this.VisitSequence(source);
            skip = this.Visit(skip);
            SelectExpression select = expression.Select;
            TableAlias nextAlias = this.GetNextAlias();
            ProjectedColumns columns = this.ProjectColumns(expression.Projector, nextAlias, new TableAlias[] { expression.Select.Alias });
            return new ProjectionExpression(new SelectExpression(nextAlias, columns.Columns, expression.Select, null, null, null, false, skip, null, false), columns.Projector);
        }

        private Expression BindTake(Expression source, Expression take)
        {
            ProjectionExpression expression = this.VisitSequence(source);
            take = this.Visit(take);
            SelectExpression select = expression.Select;
            TableAlias nextAlias = this.GetNextAlias();
            ProjectedColumns columns = this.ProjectColumns(expression.Projector, nextAlias, new TableAlias[] { expression.Select.Alias });
            return new ProjectionExpression(new SelectExpression(nextAlias, columns.Columns, expression.Select, null, null, null, false, null, take, false), columns.Projector);
        }

        protected virtual Expression BindThenBy(Expression source, LambdaExpression orderSelector, OrderType orderType)
        {
            if (this.thenBys == null)
            {
                this.thenBys = new List<OrderExpression>();
            }
            this.thenBys.Add(new OrderExpression(orderType, orderSelector));
            return this.Visit(source);
        }

        private Expression BindUpdate(IEntityTable upd, Expression instance, LambdaExpression updateCheck, LambdaExpression resultSelector)
        {
            MappingEntity entity = this.mapper.Mapping.GetEntity(instance.Type, upd.TableId);
            return this.Visit(this.mapper.GetUpdateExpression(entity, instance, updateCheck, resultSelector, null));
        }

        private Expression BindWhere(Type resultType, Expression source, LambdaExpression predicate)
        {
            ProjectionExpression expression = this.VisitSequence(source);
            this.map[predicate.Parameters[0]] = expression.Projector;
            Expression where = new WhereBinder(this.mapper, this.root, this.map, this.groupByMap).Visit(predicate.Body);
            TableAlias nextAlias = this.GetNextAlias();
            ProjectedColumns columns = this.ProjectColumns(expression.Projector, nextAlias, new TableAlias[] { expression.Select.Alias });
            return new ProjectionExpression(new SelectExpression(nextAlias, columns.Columns, expression.Select, where), columns.Projector);
        }

        private Expression BuildPredicateWithNullsEqual(IEnumerable<Expression> source1, IEnumerable<Expression> source2)
        {
            IEnumerator<Expression> enumerator = source1.GetEnumerator();
            IEnumerator<Expression> enumerator2 = source2.GetEnumerator();
            Expression expression = null;
            while (enumerator.MoveNext() && enumerator2.MoveNext())
            {
                Expression expression2 = Expression.Or(new IsNullExpression(enumerator.Current).And(new IsNullExpression(enumerator2.Current)), enumerator.Current.Equal(enumerator2.Current));
                expression = (expression == null) ? expression2 : expression.And(expression2);
            }
            return expression;
        }

        private ProjectionExpression ConvertToSequence(Expression expr)
        {
            NewExpression expression;
            NewExpression expression3;
            ExpressionType nodeType = expr.NodeType;
            switch (nodeType)
            {
                case ExpressionType.MemberAccess:
                {
                    Expression expression2 = this.BindRelationshipProperty((MemberExpression) expr);
                    if (expression2.NodeType != ExpressionType.MemberAccess)
                    {
                        return this.ConvertToSequence(expression2);
                    }
                    goto Label_00AA;
                }
                case ExpressionType.New:
                    break;

                default:
                    if (nodeType != ((ExpressionType) 0x3ec))
                    {
                        goto Label_00AA;
                    }
                    return (ProjectionExpression) expr;
            }
        Label_002F:
            expression = (NewExpression) expr;
            if (expr.Type.IsGenericType && (expr.Type.GetGenericTypeDefinition() == typeof(King.Framework.Linq.Grouping<,>)))
            {
                return (ProjectionExpression) expression.Arguments[1];
            }
        Label_00AA:
            expression3 = this.GetNewExpression(expr);
            if (expression3 == null)
            {
                throw new Exception(string.Format("The expression of type '{0}',node type '{1}' is not a sequence", expr.Type, expr.NodeType));
            }
            expr = expression3;
            goto Label_002F;
        }

        private static object GetDefault(Type type)
        {
            if (!(type.IsValueType && !King.Framework.Linq.TypeHelper.IsNullableType(type)))
            {
                return null;
            }
            return Activator.CreateInstance(type);
        }

        private static LambdaExpression GetLambda(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
            {
                e = ((UnaryExpression) e).Operand;
            }
            if (e.NodeType == ExpressionType.Constant)
            {
                return (((ConstantExpression) e).Value as LambdaExpression);
            }
            return (e as LambdaExpression);
        }

        private NewExpression GetNewExpression(Expression expression)
        {
            while ((expression.NodeType == ExpressionType.Convert) || (expression.NodeType == ExpressionType.ConvertChecked))
            {
                expression = ((UnaryExpression) expression).Operand;
            }
            return (expression as NewExpression);
        }

        internal TableAlias GetNextAlias()
        {
            return new TableAlias();
        }

        private Expression GetSingletonSequence(Expression expr, string aggregator)
        {
            ParameterExpression expression = Expression.Parameter(typeof(IEnumerable<>).MakeGenericType(new Type[] { expr.Type }), "p");
            LambdaExpression expression2 = null;
            if (aggregator != null)
            {
                expression2 = Expression.Lambda(Expression.Call(typeof(Enumerable), aggregator, new Type[] { expr.Type }, new Expression[] { expression }), new ParameterExpression[] { expression });
            }
            TableAlias nextAlias = this.GetNextAlias();
            QueryType columnType = this.language.TypeSystem.GetColumnType(expr.Type);
            return new ProjectionExpression(new SelectExpression(nextAlias, new ColumnDeclaration[] { new ColumnDeclaration("value", expr, columnType) }, null, null), new ColumnExpression(expr.Type, columnType, nextAlias, "value"), expression2);
        }

        private Type GetTrueUnderlyingType(Expression expression)
        {
            while (expression.NodeType == ExpressionType.Convert)
            {
                expression = ((UnaryExpression) expression).Operand;
            }
            return expression.Type;
        }

        private static object GetValue(object instance, MemberInfo member)
        {
            FieldInfo info = member as FieldInfo;
            if (info != null)
            {
                return info.GetValue(instance);
            }
            PropertyInfo info2 = member as PropertyInfo;
            if (info2 != null)
            {
                return info2.GetValue(instance, null);
            }
            return null;
        }

        protected bool IsQuery(Expression expression)
        {
            Type elementType = King.Framework.Linq.TypeHelper.GetElementType(expression.Type);
            if (elementType == null)
            {
                return false;
            }
            Type type2 = typeof(IQueryable<>);
            return (type2.MakeGenericType(new Type[] { elementType }).IsAssignableFrom(expression.Type) || expression.ToString().ToLower().StartsWith("query"));
        }

        protected bool IsRemoteQuery(Expression expression)
        {
            if (expression.NodeType.IsDbExpression())
            {
                return true;
            }
            switch (expression.NodeType)
            {
                case ExpressionType.Call:
                {
                    MethodCallExpression expression2 = (MethodCallExpression) expression;
                    if (expression2.Object != null)
                    {
                        return this.IsRemoteQuery(expression2.Object);
                    }
                    if (expression2.Arguments.Count > 0)
                    {
                        return this.IsRemoteQuery(expression2.Arguments[0]);
                    }
                    break;
                }
                case ExpressionType.MemberAccess:
                    return this.IsRemoteQuery(((MemberExpression) expression).Expression);
            }
            return false;
        }

        private static bool MembersMatch(MemberInfo a, MemberInfo b)
        {
            if (a.Name == b.Name)
            {
                return true;
            }
            if ((a is MethodInfo) && (b is PropertyInfo))
            {
                return (a.Name == ((PropertyInfo) b).GetGetMethod().Name);
            }
            return (((a is PropertyInfo) && (b is MethodInfo)) && (((PropertyInfo) a).GetGetMethod().Name == b.Name));
        }

        private ProjectedColumns ProjectColumns(Expression expression, TableAlias newAlias, params TableAlias[] existingAliases)
        {
            return ColumnProjector.ProjectColumns(this.language, expression, null, newAlias, existingAliases);
        }

        protected override Expression Visit(Expression exp)
        {
            Expression expression = base.Visit(exp);
            if (expression != null)
            {
                Type expectedType = exp.Type;
                ProjectionExpression expression2 = expression as ProjectionExpression;
                if (((expression2 != null) && (expression2.Aggregator == null)) && !expectedType.IsAssignableFrom(expression2.Type))
                {
                    LambdaExpression aggregator = Aggregator.GetAggregator(expectedType, expression2.Type);
                    if (aggregator != null)
                    {
                        return new ProjectionExpression(expression2.Select, expression2.Projector, aggregator);
                    }
                }
            }
            return expression;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            if (this.IsQuery(c))
            {
                MappingEntity entity2;
                IQueryable queryable = (IQueryable) c.Value;
                IEntityTable table = queryable as IEntityTable;
                if (table != null)
                {
                    IHaveMappingEntity entity = table as IHaveMappingEntity;
                    entity2 = (entity != null) ? entity.Entity : this.mapper.Mapping.GetEntity(table.ElementType, table.TableId);
                    return this.VisitSequence(this.mapper.GetQueryExpression(entity2));
                }
                if (queryable.Expression.NodeType == ExpressionType.Constant)
                {
                    entity2 = this.mapper.Mapping.GetEntity(queryable.ElementType);
                    return this.VisitSequence(this.mapper.GetQueryExpression(entity2));
                }
                QueryMapping mapping = this.mapper.Mapping;
                Expression exp = PartialEvaluator.Eval(queryable.Expression, new Func<Expression, bool>(mapping.CanBeEvaluatedLocally));
                return this.Visit(exp);
            }
            return c;
        }

        protected override Expression VisitInvocation(InvocationExpression iv)
        {
            LambdaExpression expression = iv.Expression as LambdaExpression;
            if (expression != null)
            {
                int num = 0;
                int count = expression.Parameters.Count;
                while (num < count)
                {
                    this.map[expression.Parameters[num]] = iv.Arguments[num];
                    num++;
                }
                return this.Visit(expression.Body);
            }
            return base.VisitInvocation(iv);
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            if ((((m.Expression != null) && (m.Expression.NodeType == ExpressionType.Parameter)) && !this.map.ContainsKey((ParameterExpression) m.Expression)) && this.IsQuery(m))
            {
                return this.VisitSequence(this.mapper.GetQueryExpression(this.mapper.Mapping.GetEntity(m.Member)));
            }
            Expression expression = this.Visit(m.Expression);
            if (this.language.IsAggregate(m.Member) && this.IsRemoteQuery(expression))
            {
                return this.BindAggregate(m.Expression, m.Member.Name, King.Framework.Linq.TypeHelper.GetMemberType(m.Member), null, m == this.root);
            }
            Expression expression2 = BindMember(expression, m.Member);
            MemberExpression expression3 = expression2 as MemberExpression;
            if (((expression3 != null) && (expression3.Member == m.Member)) && (expression3.Expression == m.Expression))
            {
                return m;
            }
            return expression2;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if ((m.Method.DeclaringType != typeof(Queryable)) && !(m.Method.DeclaringType == typeof(Enumerable)))
            {
                if (typeof(Updatable).IsAssignableFrom(m.Method.DeclaringType))
                {
                    IEntityTable upd = (this.batchUpd != null) ? this.batchUpd : ((IEntityTable) ((ConstantExpression) m.Arguments[0]).Value);
                    switch (m.Method.Name)
                    {
                        case "Insert":
                            return this.BindInsert(upd, m.Arguments[1], (m.Arguments.Count > 2) ? GetLambda(m.Arguments[2]) : null);

                        case "Update":
                            return this.BindUpdate(upd, m.Arguments[1], (m.Arguments.Count > 2) ? GetLambda(m.Arguments[2]) : null, (m.Arguments.Count > 3) ? GetLambda(m.Arguments[3]) : null);

                        case "InsertOrUpdate":
                            return this.BindInsertOrUpdate(upd, m.Arguments[1], (m.Arguments.Count > 2) ? GetLambda(m.Arguments[2]) : null, (m.Arguments.Count > 3) ? GetLambda(m.Arguments[3]) : null);

                        case "Delete":
                            if ((m.Arguments.Count == 2) && (GetLambda(m.Arguments[1]) != null))
                            {
                                return this.BindDelete(upd, null, GetLambda(m.Arguments[1]));
                            }
                            return this.BindDelete(upd, m.Arguments[1], (m.Arguments.Count > 2) ? GetLambda(m.Arguments[2]) : null);

                        case "Batch":
                            return this.BindBatch(upd, m.Arguments[1], GetLambda(m.Arguments[2]), (m.Arguments.Count > 3) ? m.Arguments[3] : Expression.Constant(50), (m.Arguments.Count > 4) ? m.Arguments[4] : Expression.Constant(false));
                    }
                }
            }
            else
            {
                switch (m.Method.Name)
                {
                    case "Where":
                        return this.BindWhere(m.Type, m.Arguments[0], GetLambda(m.Arguments[1]));

                    case "Select":
                        return this.BindSelect(m.Type, m.Arguments[0], GetLambda(m.Arguments[1]));

                    case "SelectMany":
                        if (m.Arguments.Count != 2)
                        {
                            if (m.Arguments.Count != 3)
                            {
                                break;
                            }
                            return this.BindSelectMany(m.Type, m.Arguments[0], GetLambda(m.Arguments[1]), GetLambda(m.Arguments[2]));
                        }
                        return this.BindSelectMany(m.Type, m.Arguments[0], GetLambda(m.Arguments[1]), null);

                    case "Join":
                        return this.BindJoin(m.Type, m.Arguments[0], m.Arguments[1], GetLambda(m.Arguments[2]), GetLambda(m.Arguments[3]), GetLambda(m.Arguments[4]));

                    case "GroupJoin":
                        if (m.Arguments.Count != 5)
                        {
                            break;
                        }
                        return this.BindGroupJoin(m.Method, m.Arguments[0], m.Arguments[1], GetLambda(m.Arguments[2]), GetLambda(m.Arguments[3]), GetLambda(m.Arguments[4]));

                    case "OrderBy":
                        return this.BindOrderBy(m.Type, m.Arguments[0], GetLambda(m.Arguments[1]), OrderType.Ascending);

                    case "OrderByDescending":
                        return this.BindOrderBy(m.Type, m.Arguments[0], GetLambda(m.Arguments[1]), OrderType.Descending);

                    case "ThenBy":
                        return this.BindThenBy(m.Arguments[0], GetLambda(m.Arguments[1]), OrderType.Ascending);

                    case "ThenByDescending":
                        return this.BindThenBy(m.Arguments[0], GetLambda(m.Arguments[1]), OrderType.Descending);

                    case "GroupBy":
                        if (m.Arguments.Count != 2)
                        {
                            if (m.Arguments.Count == 3)
                            {
                                LambdaExpression lambda = GetLambda(m.Arguments[1]);
                                LambdaExpression elementSelector = GetLambda(m.Arguments[2]);
                                if (elementSelector.Parameters.Count == 1)
                                {
                                    return this.BindGroupBy(m.Arguments[0], lambda, elementSelector, null);
                                }
                                if (elementSelector.Parameters.Count == 2)
                                {
                                    return this.BindGroupBy(m.Arguments[0], lambda, null, elementSelector);
                                }
                                break;
                            }
                            if (m.Arguments.Count != 4)
                            {
                                break;
                            }
                            return this.BindGroupBy(m.Arguments[0], GetLambda(m.Arguments[1]), GetLambda(m.Arguments[2]), GetLambda(m.Arguments[3]));
                        }
                        return this.BindGroupBy(m.Arguments[0], GetLambda(m.Arguments[1]), null, null);

                    case "Distinct":
                        if (m.Arguments.Count != 1)
                        {
                            break;
                        }
                        return this.BindDistinct(m.Arguments[0]);

                    case "Skip":
                        if (m.Arguments.Count != 2)
                        {
                            break;
                        }
                        return this.BindSkip(m.Arguments[0], m.Arguments[1]);

                    case "Take":
                        if (m.Arguments.Count != 2)
                        {
                            break;
                        }
                        return this.BindTake(m.Arguments[0], m.Arguments[1]);

                    case "First":
                    case "FirstOrDefault":
                    case "Single":
                    case "SingleOrDefault":
                    case "Last":
                    case "LastOrDefault":
                        if (m.Arguments.Count != 1)
                        {
                            if (m.Arguments.Count != 2)
                            {
                                break;
                            }
                            return this.BindFirst(m.Arguments[0], GetLambda(m.Arguments[1]), m.Method.Name, m == this.root);
                        }
                        return this.BindFirst(m.Arguments[0], null, m.Method.Name, m == this.root);

                    case "Any":
                        if (m.Arguments.Count != 1)
                        {
                            if (m.Arguments.Count == 2)
                            {
                                return this.BindAnyAll(m.Arguments[0], m.Method, GetLambda(m.Arguments[1]), m == this.root);
                            }
                            break;
                        }
                        return this.BindAnyAll(m.Arguments[0], m.Method, null, m == this.root);

                    case "All":
                        if (m.Arguments.Count != 2)
                        {
                            break;
                        }
                        return this.BindAnyAll(m.Arguments[0], m.Method, GetLambda(m.Arguments[1]), m == this.root);

                    case "Contains":
                        if (m.Arguments.Count != 2)
                        {
                            break;
                        }
                        return this.BindContains(m.Arguments[0], m.Arguments[1], m == this.root);

                    case "Cast":
                        if (m.Arguments.Count != 1)
                        {
                            break;
                        }
                        return this.BindCast(m.Arguments[0], m.Method.GetGenericArguments()[0]);

                    case "Reverse":
                        return this.BindReverse(m.Arguments[0]);

                    case "Intersect":
                    case "Except":
                        if (m.Arguments.Count != 2)
                        {
                            break;
                        }
                        return this.BindIntersect(m.Arguments[0], m.Arguments[1], m.Method.Name == "Except");
                }
            }
            if (this.language.IsAggregate(m.Method))
            {
                return this.BindAggregate(m.Arguments[0], m.Method.Name, m.Method.ReturnType, (m.Arguments.Count > 1) ? GetLambda(m.Arguments[1]) : null, m == this.root);
            }
            return base.VisitMethodCall(m);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            Expression expression;
            if (this.map.TryGetValue(p, out expression))
            {
                return expression;
            }
            return p;
        }

        protected ProjectionExpression VisitSequence(Expression source)
        {
            return this.ConvertToSequence(base.Visit(source));
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            if (((u.NodeType == ExpressionType.Convert) || (u.NodeType == ExpressionType.ConvertChecked)) && (u == this.root))
            {
                this.root = u.Operand;
            }
            return base.VisitUnary(u);
        }

        internal class GroupByInfo
        {
            internal GroupByInfo(TableAlias alias, Expression element)
            {
                this.Alias = alias;
                this.Element = element;
            }

            internal TableAlias Alias { get; private set; }

            internal Expression Element { get; private set; }
        }
    }
}
