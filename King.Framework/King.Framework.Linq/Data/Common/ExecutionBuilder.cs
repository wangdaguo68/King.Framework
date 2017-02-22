namespace King.Framework.Linq.Data.Common
{
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class ExecutionBuilder : DbExpressionVisitor
    {
        private Expression executor;
        private List<Expression> initializers = new List<Expression>();
        private bool isTop = true;
        private QueryLinguist linguist;
        private int nLookup = 0;
        private int nReaders = 0;
        private QueryPolicy policy;
        private MemberInfo receivingMember;
        private Scope scope;
        private Dictionary<string, Expression> variableMap = new Dictionary<string, Expression>();
        private List<ParameterExpression> variables = new List<ParameterExpression>();

        private ExecutionBuilder(QueryLinguist linguist, QueryPolicy policy, Expression executor)
        {
            this.linguist = linguist;
            this.policy = policy;
            this.executor = executor;
        }

        private Expression AddVariables(Expression expression)
        {
            if (this.variables.Count > 0)
            {
                List<Expression> expressions = new List<Expression>();
                int num = 0;
                int count = this.variables.Count;
                while (num < count)
                {
                    expressions.Add(MakeAssign(this.variables[num], this.initializers[num]));
                    num++;
                }
                expressions.Add(expression);
                Expression body = MakeSequence(expressions);
                Expression[] arguments = (from v in this.variables select Expression.Constant(null, v.Type)).ToArray<ConstantExpression>();
                expression = Expression.Invoke(Expression.Lambda(body, this.variables.ToArray()), arguments);
            }
            return expression;
        }

        public static T Assign<T>(ref T variable, T value)
        {
            variable = value;
            return value;
        }

        public static IEnumerable<R> Batch<T, R>(IEnumerable<T> items, Func<T, R> selector, bool stream)
        {
            IEnumerable<R> source = items.Select<T, R>(selector);
            if (!stream)
            {
                return (IEnumerable<R>) source.ToList<R>();
            }
            return new EnumerateOnce<R>(source);
        }

        private Expression Build(Expression expression)
        {
            expression = this.Visit(expression);
            expression = this.AddVariables(expression);
            return expression;
        }

        public static Expression Build(QueryLinguist linguist, QueryPolicy policy, Expression expression, Expression provider)
        {
            ParameterExpression expression2;
            return new ExecutionBuilder(linguist, policy, expression2 = Expression.Parameter(typeof(QueryExecutor), "executor")) { variables = { expression2 }, initializers = { Expression.Call((Expression) Expression.Convert(provider, typeof(ICreateExecutor)), "CreateExecutor", (Type[]) null, (Expression[]) null) } }.Build(expression);
        }

        protected virtual Expression BuildExecuteBatch(BatchExpression batch)
        {
            Expression expression = this.Parameterize(batch.Operation.Body);
            string commandText = this.linguist.Format(expression);
            ReadOnlyCollection<NamedValueExpression> onlys = NamedValueGatherer.Gather(expression);
            QueryCommand command = new QueryCommand(commandText, from v in onlys select new QueryParameter(v.Name, v.Type, v.QueryType));
            Expression[] initializers = (from v in onlys select Expression.Convert(this.Visit(v.Value), typeof(object))).ToArray<UnaryExpression>();
            Expression expression2 = Expression.Call(typeof(Enumerable), "Select", new Type[] { batch.Operation.Parameters[1].Type, typeof(object[]) }, new Expression[] { batch.Input, Expression.Lambda(Expression.NewArrayInit(typeof(object), initializers), new ParameterExpression[] { batch.Operation.Parameters[1] }) });
            ProjectionExpression expression4 = ProjectionFinder.FindProjection(expression);
            if (expression4 != null)
            {
                Scope scope = this.scope;
                ParameterExpression fieldReader = Expression.Parameter(typeof(FieldReader), "r" + this.nReaders++);
                this.scope = new Scope(this.scope, fieldReader, expression4.Select.Alias, expression4.Select.Columns);
                LambdaExpression expression6 = Expression.Lambda(this.Visit(expression4.Projector), new ParameterExpression[] { fieldReader });
                this.scope = scope;
                MappingEntity entity = EntityFinder.Find(expression4.Projector);
                command = new QueryCommand(command.CommandText, command.Parameters);
                return Expression.Call(this.executor, "ExecuteBatch", new Type[] { expression6.Body.Type }, new Expression[] { Expression.Constant(command), expression2, expression6, Expression.Constant(entity, typeof(MappingEntity)), batch.BatchSize, batch.Stream });
            }
            return Expression.Call(this.executor, "ExecuteBatch", (Type[]) null, new Expression[] { Expression.Constant(command), expression2, batch.BatchSize, batch.Stream });
        }

        protected virtual Expression BuildExecuteCommand(CommandExpression command)
        {
            Expression expression = this.Parameterize(command);
            string commandText = this.linguist.Format(expression);
            ReadOnlyCollection<NamedValueExpression> onlys = NamedValueGatherer.Gather(expression);
            QueryCommand command2 = new QueryCommand(commandText, from v in onlys select new QueryParameter(v.Name, v.Type, v.QueryType));
            Expression[] values = (from v in onlys select Expression.Convert(this.Visit(v.Value), typeof(object))).ToArray<UnaryExpression>();
            ProjectionExpression projection = ProjectionFinder.FindProjection(expression);
            if (projection != null)
            {
                return this.ExecuteProjection(projection, false, command2, values);
            }
            return Expression.Call(this.executor, "ExecuteCommand", (Type[]) null, new Expression[] { Expression.Constant(command2), Expression.NewArrayInit(typeof(object), values) });
        }

        private Expression BuildInner(Expression expression)
        {
            ExecutionBuilder builder = new ExecutionBuilder(this.linguist, this.policy, this.executor) {
                scope = this.scope,
                receivingMember = this.receivingMember,
                nReaders = this.nReaders,
                nLookup = this.nLookup,
                variableMap = this.variableMap
            };
            return builder.Build(expression);
        }

        private Expression ExecuteProjection(ProjectionExpression projection, bool okayToDefer)
        {
            projection = (ProjectionExpression) this.Parameterize(projection);
            if (this.scope != null)
            {
                projection = (ProjectionExpression) OuterParameterizer.Parameterize(this.scope.Alias, projection);
            }
            string commandText = this.linguist.Format(projection.Select);
            ReadOnlyCollection<NamedValueExpression> onlys = NamedValueGatherer.Gather(projection.Select);
            QueryCommand command = new QueryCommand(commandText, from v in onlys select new QueryParameter(v.Name, v.Type, v.QueryType));
            Expression[] values = (from v in onlys select Expression.Convert(this.Visit(v.Value), typeof(object))).ToArray<UnaryExpression>();
            return this.ExecuteProjection(projection, okayToDefer, command, values);
        }

        private Expression ExecuteProjection(ProjectionExpression projection, bool okayToDefer, QueryCommand command, Expression[] values)
        {
            okayToDefer &= (this.receivingMember != null) && this.policy.IsDeferLoaded(this.receivingMember);
            Scope scope = this.scope;
            ParameterExpression fieldReader = Expression.Parameter(typeof(FieldReader), "r" + this.nReaders++);
            this.scope = new Scope(this.scope, fieldReader, projection.Select.Alias, projection.Select.Columns);
            LambdaExpression expression2 = Expression.Lambda(this.Visit(projection.Projector), new ParameterExpression[] { fieldReader });
            this.scope = scope;
            MappingEntity entity = EntityFinder.Find(projection.Projector);
            string methodName = okayToDefer ? "ExecuteDeferred" : "Execute";
            Expression replaceWith = Expression.Call(this.executor, methodName, new Type[] { expression2.Body.Type }, new Expression[] { Expression.Constant(command), expression2, Expression.Constant(entity, typeof(MappingEntity)), Expression.NewArrayInit(typeof(object), values) });
            if (projection.Aggregator != null)
            {
                replaceWith = DbExpressionReplacer.Replace(projection.Aggregator.Body, projection.Aggregator.Parameters[0], replaceWith);
            }
            return replaceWith;
        }

        protected virtual bool IsMultipleCommands(CommandExpression command)
        {
            if (command == null)
            {
                return false;
            }
            switch (command.NodeType)
            {
                case ((ExpressionType) 0x3fa):
                case ((ExpressionType) 0x3fb):
                case ((ExpressionType) 0x3fc):
                    return false;
            }
            return true;
        }

        private static Expression MakeAssign(ParameterExpression variable, Expression value)
        {
            return Expression.Call(typeof(ExecutionBuilder), "Assign", new Type[] { variable.Type }, new Expression[] { variable, value });
        }

        private Expression MakeJoinKey(IList<Expression> key)
        {
            if (key.Count == 1)
            {
                return key[0];
            }
            return Expression.New(typeof(CompoundKey).GetConstructors()[0], new Expression[] { Expression.NewArrayInit(typeof(object), (IEnumerable<Expression>) (from k in key select Expression.Convert(k, typeof(object)))) });
        }

        private static Expression MakeSequence(IList<Expression> expressions)
        {
            Expression expression = expressions[expressions.Count - 1];
            expressions = (from e in expressions select e).ToList<Expression>();
            return Expression.Convert(Expression.Call(typeof(ExecutionBuilder), "Sequence", null, new Expression[] { Expression.NewArrayInit(typeof(object), expressions) }), expression.Type);
        }

        protected virtual Expression Parameterize(Expression expression)
        {
            if (this.variableMap.Count > 0)
            {
                expression = VariableSubstitutor.Substitute(this.variableMap, expression);
            }
            return this.linguist.Parameterize(expression);
        }

        public static object Sequence(params object[] values)
        {
            return values[values.Length - 1];
        }

        protected override Expression VisitBatch(BatchExpression batch)
        {
            if (!(!this.linguist.Language.AllowsMultipleCommands && this.IsMultipleCommands(batch.Operation.Body as CommandExpression)))
            {
                return this.BuildExecuteBatch(batch);
            }
            Expression expression = this.Visit(batch.Input);
            LambdaExpression expression3 = Expression.Lambda(this.Visit(batch.Operation.Body), new ParameterExpression[] { batch.Operation.Parameters[1] });
            return Expression.Call(base.GetType(), "Batch", new Type[] { King.Framework.Linq.TypeHelper.GetElementType(expression.Type), batch.Operation.Body.Type }, new Expression[] { expression, expression3, batch.Stream });
        }

        protected override MemberBinding VisitBinding(MemberBinding binding)
        {
            MemberInfo receivingMember = this.receivingMember;
            this.receivingMember = binding.Member;
            MemberBinding binding2 = base.VisitBinding(binding);
            this.receivingMember = receivingMember;
            return binding2;
        }

        protected override Expression VisitBlock(BlockCommand block)
        {
            return MakeSequence(this.VisitExpressionList(block.Commands));
        }

        protected override Expression VisitClientJoin(ClientJoinExpression join)
        {
            Expression expression = this.MakeJoinKey(join.InnerKey as IList<Expression>);
            Expression exp = this.MakeJoinKey(join.OuterKey as IList<Expression>);
            Expression projector = Expression.New(typeof(KeyValuePair<,>).MakeGenericType(new Type[] { expression.Type, join.Projection.Projector.Type }).GetConstructor(new Type[] { expression.Type, join.Projection.Projector.Type }), new Expression[] { expression, join.Projection.Projector });
            ProjectionExpression projection = new ProjectionExpression(join.Projection.Select, projector);
            int num = ++this.nLookup;
            Expression expression5 = this.ExecuteProjection(projection, false);
            ParameterExpression expression6 = Expression.Parameter(projector.Type, "kvp");
            if (join.Projection.Projector.NodeType == ((ExpressionType) 0x3f9))
            {
                LambdaExpression expression7 = Expression.Lambda(Expression.PropertyOrField(expression6, "Value").NotEqual(King.Framework.Linq.TypeHelper.GetNullConstant(join.Projection.Projector.Type)), new ParameterExpression[] { expression6 });
                expression5 = Expression.Call(typeof(Enumerable), "Where", new Type[] { expression6.Type }, new Expression[] { expression5, expression7 });
            }
            LambdaExpression expression8 = Expression.Lambda(Expression.PropertyOrField(expression6, "Key"), new ParameterExpression[] { expression6 });
            LambdaExpression expression9 = Expression.Lambda(Expression.PropertyOrField(expression6, "Value"), new ParameterExpression[] { expression6 });
            Expression item = Expression.Call(typeof(Enumerable), "ToLookup", new Type[] { expression6.Type, exp.Type, join.Projection.Projector.Type }, new Expression[] { expression5, expression8, expression9 });
            ParameterExpression instance = Expression.Parameter(item.Type, "lookup" + num);
            Expression replaceWith = Expression.Call(instance, instance.Type.GetProperty("Item").GetGetMethod(), new Expression[] { this.Visit(exp) });
            if (join.Projection.Aggregator != null)
            {
                replaceWith = DbExpressionReplacer.Replace(join.Projection.Aggregator.Body, join.Projection.Aggregator.Parameters[0], replaceWith);
            }
            this.variables.Add(instance);
            this.initializers.Add(item);
            return replaceWith;
        }

        protected override Expression VisitColumn(ColumnExpression column)
        {
            ParameterExpression expression;
            int num;
            if ((this.scope != null) && this.scope.TryGetValue(column, out expression, out num))
            {
                MethodInfo readerMethod = FieldReader.GetReaderMethod(column.Type);
                return Expression.Call(expression, readerMethod, new Expression[] { Expression.Constant(num) });
            }
            Debug.Fail(string.Format("column not in scope: {0}", column));
            return column;
        }

        protected override Expression VisitCommand(CommandExpression command)
        {
            if (!(!this.linguist.Language.AllowsMultipleCommands && this.IsMultipleCommands(command)))
            {
                return this.BuildExecuteCommand(command);
            }
            return base.VisitCommand(command);
        }

        protected override Expression VisitDeclaration(DeclarationCommand decl)
        {
            ParameterExpression expression2;
            if (decl.Source == null)
            {
                throw new InvalidOperationException("Declaration query not allowed for this langauge");
            }
            ProjectionExpression exp = new ProjectionExpression(decl.Source, Expression.NewArrayInit(typeof(object), (from v in decl.Variables select v.Expression).ToArray<Expression>()), Aggregator.GetAggregator(typeof(object[]), typeof(IEnumerable<object[]>)));
            this.variables.Add(expression2 = Expression.Parameter(typeof(object[]), "vars"));
            this.initializers.Add(Expression.Constant(null, typeof(object[])));
            int num = 0;
            int count = decl.Variables.Count;
            while (num < count)
            {
                VariableDeclaration declaration = decl.Variables[num];
                NamedValueExpression expression3 = new NamedValueExpression(declaration.Name, declaration.QueryType, Expression.Convert(Expression.ArrayIndex(expression2, Expression.Constant(num)), declaration.Expression.Type));
                this.variableMap.Add(declaration.Name, expression3);
                num++;
            }
            return MakeAssign(expression2, this.Visit(exp));
        }

        protected override Expression VisitDelete(DeleteCommand delete)
        {
            return this.BuildExecuteCommand(delete);
        }

        protected override Expression VisitEntity(EntityExpression entity)
        {
            return this.Visit(entity.Expression);
        }

        protected override Expression VisitExists(ExistsExpression exists)
        {
            QueryType columnType = this.linguist.Language.TypeSystem.GetColumnType(typeof(int));
            SelectExpression source = exists.Select.SetColumns(new ColumnDeclaration[] { new ColumnDeclaration("value", new AggregateExpression(typeof(int), "Count", null, false), columnType) });
            Expression exp = new ProjectionExpression(source, new ColumnExpression(typeof(int), columnType, source.Alias, "value"), Aggregator.GetAggregator(typeof(int), typeof(IEnumerable<int>))).GreaterThan(Expression.Constant(0));
            return this.Visit(exp);
        }

        protected override Expression VisitFunction(FunctionExpression func)
        {
            if (this.linguist.Language.IsRowsAffectedExpressions(func))
            {
                return Expression.Property(this.executor, "RowsAffected");
            }
            return base.VisitFunction(func);
        }

        protected override Expression VisitIf(IFCommand ifx)
        {
            ConditionalExpression exp = Expression.Condition(ifx.Check, ifx.IfTrue, (ifx.IfFalse != null) ? ifx.IfFalse : ((ifx.IfTrue.Type == typeof(int)) ? ((Expression) Expression.Property(this.executor, "RowsAffected")) : ((Expression) Expression.Constant(King.Framework.Linq.TypeHelper.GetDefault(ifx.IfTrue.Type), ifx.IfTrue.Type))));
            return this.Visit(exp);
        }

        protected override Expression VisitInsert(InsertCommand insert)
        {
            return this.BuildExecuteCommand(insert);
        }

        protected override Expression VisitOuterJoined(OuterJoinedExpression outer)
        {
            ParameterExpression expression3;
            int num;
            Expression ifFalse = this.Visit(outer.Expression);
            ColumnExpression test = (ColumnExpression) outer.Test;
            if (this.scope.TryGetValue(test, out expression3, out num))
            {
                return Expression.Condition(Expression.Call((Expression) expression3, "IsDbNull", (Type[]) null, new Expression[] { Expression.Constant(num) }), Expression.Constant(King.Framework.Linq.TypeHelper.GetDefault(outer.Type), outer.Type), ifFalse);
            }
            return ifFalse;
        }

        protected override Expression VisitProjection(ProjectionExpression projection)
        {
            if (this.isTop)
            {
                this.isTop = false;
                return this.ExecuteProjection(projection, this.scope != null);
            }
            return this.BuildInner(projection);
        }

        protected override Expression VisitUpdate(UpdateCommand update)
        {
            return this.BuildExecuteCommand(update);
        }

        private class ColumnGatherer : DbExpressionVisitor
        {
            private Dictionary<string, ColumnExpression> columns = new Dictionary<string, ColumnExpression>();

            internal static IEnumerable<ColumnExpression> Gather(Expression expression)
            {
                ExecutionBuilder.ColumnGatherer gatherer = new ExecutionBuilder.ColumnGatherer();
                gatherer.Visit(expression);
                return gatherer.columns.Values;
            }

            protected override Expression VisitColumn(ColumnExpression column)
            {
                if (!this.columns.ContainsKey(column.Name))
                {
                    this.columns.Add(column.Name, column);
                }
                return column;
            }
        }

        private class EntityFinder : DbExpressionVisitor
        {
            private MappingEntity entity;

            public static MappingEntity Find(Expression expression)
            {
                ExecutionBuilder.EntityFinder finder = new ExecutionBuilder.EntityFinder();
                finder.Visit(expression);
                return finder.entity;
            }

            protected override Expression Visit(Expression exp)
            {
                if (this.entity == null)
                {
                    return base.Visit(exp);
                }
                return exp;
            }

            protected override Expression VisitEntity(EntityExpression entity)
            {
                if (this.entity == null)
                {
                    this.entity = entity.Entity;
                }
                return entity;
            }

            protected override Expression VisitMemberInit(MemberInitExpression init)
            {
                return init;
            }

            protected override NewExpression VisitNew(NewExpression nex)
            {
                return nex;
            }
        }

        private class OuterParameterizer : DbExpressionVisitor
        {
            private int iParam;
            private Dictionary<ColumnExpression, NamedValueExpression> map = new Dictionary<ColumnExpression, NamedValueExpression>();
            private TableAlias outerAlias;

            internal static Expression Parameterize(TableAlias outerAlias, Expression expr)
            {
                ExecutionBuilder.OuterParameterizer parameterizer = new ExecutionBuilder.OuterParameterizer {
                    outerAlias = outerAlias
                };
                return parameterizer.Visit(expr);
            }

            protected override Expression VisitColumn(ColumnExpression column)
            {
                if (column.Alias == this.outerAlias)
                {
                    NamedValueExpression expression;
                    if (!this.map.TryGetValue(column, out expression))
                    {
                        expression = new NamedValueExpression("n" + this.iParam++, column.QueryType, column);
                        this.map.Add(column, expression);
                    }
                    return expression;
                }
                return column;
            }

            protected override Expression VisitProjection(ProjectionExpression proj)
            {
                SelectExpression select = (SelectExpression) this.Visit(proj.Select);
                return base.UpdateProjection(proj, select, proj.Projector, proj.Aggregator);
            }
        }

        private class ProjectionFinder : DbExpressionVisitor
        {
            private ProjectionExpression found = null;

            internal static ProjectionExpression FindProjection(Expression expression)
            {
                ExecutionBuilder.ProjectionFinder finder = new ExecutionBuilder.ProjectionFinder();
                finder.Visit(expression);
                return finder.found;
            }

            protected override Expression VisitProjection(ProjectionExpression proj)
            {
                this.found = proj;
                return proj;
            }
        }

        private class Scope
        {
            private ParameterExpression fieldReader;
            private Dictionary<string, int> nameMap;
            private ExecutionBuilder.Scope outer;

            internal Scope(ExecutionBuilder.Scope outer, ParameterExpression fieldReader, TableAlias alias, IEnumerable<ColumnDeclaration> columns)
            {
                this.outer = outer;
                this.fieldReader = fieldReader;
                this.Alias = alias;
                //this.nameMap = columns.Select(((Func<ColumnDeclaration, int, <>f__AnonymousType0<ColumnDeclaration, int>>) ((c, i) => new { c = c, i = i }))).ToDictionary(x => x.c.Name, x => x.i);
            }

            internal bool TryGetValue(ColumnExpression column, out ParameterExpression fieldReader, out int ordinal)
            {
                for (ExecutionBuilder.Scope scope = this; scope != null; scope = scope.outer)
                {
                    if ((column.Alias == scope.Alias) && this.nameMap.TryGetValue(column.Name, out ordinal))
                    {
                        fieldReader = this.fieldReader;
                        return true;
                    }
                }
                fieldReader = null;
                ordinal = 0;
                return false;
            }

            internal TableAlias Alias { get; private set; }
        }

        private class VariableSubstitutor : DbExpressionVisitor
        {
            private Dictionary<string, Expression> map;

            private VariableSubstitutor(Dictionary<string, Expression> map)
            {
                this.map = map;
            }

            public static Expression Substitute(Dictionary<string, Expression> map, Expression expression)
            {
                return new ExecutionBuilder.VariableSubstitutor(map).Visit(expression);
            }

            protected override Expression VisitVariable(VariableExpression vex)
            {
                Expression expression;
                if (this.map.TryGetValue(vex.Name, out expression))
                {
                    return expression;
                }
                return vex;
            }
        }
    }
}
