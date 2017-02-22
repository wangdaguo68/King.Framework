namespace King.Framework.Linq.Data.Common
{
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;

    public abstract class DbExpressionVisitor : King.Framework.Linq.ExpressionVisitor
    {
        protected DbExpressionVisitor()
        {
        }

        protected AggregateExpression UpdateAggregate(AggregateExpression aggregate, Type type, string aggType, Expression arg, bool isDistinct)
        {
            if ((((type != aggregate.Type) || (aggType != aggregate.AggregateName)) || (arg != aggregate.Argument)) || (isDistinct != aggregate.IsDistinct))
            {
                return new AggregateExpression(type, aggType, arg, isDistinct);
            }
            return aggregate;
        }

        protected AggregateSubqueryExpression UpdateAggregateSubquery(AggregateSubqueryExpression aggregate, ScalarExpression subquery)
        {
            if (subquery != aggregate.AggregateAsSubquery)
            {
                return new AggregateSubqueryExpression(aggregate.GroupByAlias, aggregate.AggregateInGroupSelect, subquery);
            }
            return aggregate;
        }

        protected BatchExpression UpdateBatch(BatchExpression batch, Expression input, LambdaExpression operation, Expression batchSize, Expression stream)
        {
            if ((((input != batch.Input) || (operation != batch.Operation)) || (batchSize != batch.BatchSize)) || (stream != batch.Stream))
            {
                return new BatchExpression(input, operation, batchSize, stream);
            }
            return batch;
        }

        protected BetweenExpression UpdateBetween(BetweenExpression between, Expression expression, Expression lower, Expression upper)
        {
            if (((expression != between.Expression) || (lower != between.Lower)) || (upper != between.Upper))
            {
                return new BetweenExpression(expression, lower, upper);
            }
            return between;
        }

        protected BlockCommand UpdateBlock(BlockCommand block, IList<Expression> commands)
        {
            if (block.Commands != commands)
            {
                return new BlockCommand(commands);
            }
            return block;
        }

        protected ClientJoinExpression UpdateClientJoin(ClientJoinExpression join, ProjectionExpression projection, IEnumerable<Expression> outerKey, IEnumerable<Expression> innerKey)
        {
            if (((projection != join.Projection) || (outerKey != join.OuterKey)) || (innerKey != join.InnerKey))
            {
                return new ClientJoinExpression(projection, outerKey, innerKey);
            }
            return join;
        }

        protected ColumnAssignment UpdateColumnAssignment(ColumnAssignment ca, ColumnExpression c, Expression e)
        {
            if ((c != ca.Column) || (e != ca.Expression))
            {
                return new ColumnAssignment(c, e);
            }
            return ca;
        }

        protected DeclarationCommand UpdateDeclaration(DeclarationCommand decl, IEnumerable<VariableDeclaration> variables, SelectExpression source)
        {
            if ((variables != decl.Variables) || (source != decl.Source))
            {
                return new DeclarationCommand(variables, source);
            }
            return decl;
        }

        protected DeleteCommand UpdateDelete(DeleteCommand delete, TableExpression table, Expression where)
        {
            if ((table != delete.Table) || (where != delete.Where))
            {
                return new DeleteCommand(table, where);
            }
            return delete;
        }

        protected EntityExpression UpdateEntity(EntityExpression entity, Expression expression)
        {
            if (expression != entity.Expression)
            {
                return new EntityExpression(entity.Entity, expression);
            }
            return entity;
        }

        protected ExistsExpression UpdateExists(ExistsExpression exists, SelectExpression select)
        {
            if (select != exists.Select)
            {
                return new ExistsExpression(select);
            }
            return exists;
        }

        protected FunctionExpression UpdateFunction(FunctionExpression func, string name, IEnumerable<Expression> arguments)
        {
            if ((name != func.Name) || (arguments != func.Arguments))
            {
                return new FunctionExpression(func.Type, name, arguments);
            }
            return func;
        }

        protected IFCommand UpdateIf(IFCommand ifx, Expression check, Expression ifTrue, Expression ifFalse)
        {
            if (((check != ifx.Check) || (ifTrue != ifx.IfTrue)) || (ifFalse != ifx.IfFalse))
            {
                return new IFCommand(check, ifTrue, ifFalse);
            }
            return ifx;
        }

        protected InExpression UpdateIn(InExpression @in, Expression expression, SelectExpression select, IEnumerable<Expression> values)
        {
            if (((expression != @in.Expression) || (select != @in.Select)) || (values != @in.Values))
            {
                if (select != null)
                {
                    return new InExpression(expression, select);
                }
                return new InExpression(expression, values);
            }
            return @in;
        }

        protected InsertCommand UpdateInsert(InsertCommand insert, TableExpression table, IEnumerable<ColumnAssignment> assignments)
        {
            if ((table != insert.Table) || (assignments != insert.Assignments))
            {
                return new InsertCommand(table, assignments);
            }
            return insert;
        }

        protected IsNullExpression UpdateIsNull(IsNullExpression isnull, Expression expression)
        {
            if (expression != isnull.Expression)
            {
                return new IsNullExpression(expression);
            }
            return isnull;
        }

        protected JoinExpression UpdateJoin(JoinExpression join, JoinType joinType, Expression left, Expression right, Expression condition)
        {
            if ((((joinType != join.Join) || (left != join.Left)) || (right != join.Right)) || (condition != join.Condition))
            {
                return new JoinExpression(joinType, left, right, condition);
            }
            return join;
        }

        protected OuterJoinedExpression UpdateOuterJoined(OuterJoinedExpression outer, Expression test, Expression expression)
        {
            if ((test != outer.Test) || (expression != outer.Expression))
            {
                return new OuterJoinedExpression(test, expression);
            }
            return outer;
        }

        protected ProjectionExpression UpdateProjection(ProjectionExpression proj, SelectExpression select, Expression projector, LambdaExpression aggregator)
        {
            if (((select != proj.Select) || (projector != proj.Projector)) || (aggregator != proj.Aggregator))
            {
                return new ProjectionExpression(select, projector, aggregator);
            }
            return proj;
        }

        protected RowNumberExpression UpdateRowNumber(RowNumberExpression rowNumber, IEnumerable<OrderExpression> orderBy)
        {
            if (orderBy != rowNumber.OrderBy)
            {
                return new RowNumberExpression(orderBy);
            }
            return rowNumber;
        }

        protected ScalarExpression UpdateScalar(ScalarExpression scalar, SelectExpression select)
        {
            if (select != scalar.Select)
            {
                return new ScalarExpression(scalar.Type, select);
            }
            return scalar;
        }

        protected SelectExpression UpdateSelect(SelectExpression select, Expression from, Expression where, IEnumerable<OrderExpression> orderBy, IEnumerable<Expression> groupBy, Expression skip, Expression take, bool isDistinct, bool isReverse, IEnumerable<ColumnDeclaration> columns)
        {
            if (((((from != select.From) || (where != select.Where)) || ((orderBy != select.OrderBy) || (groupBy != select.GroupBy))) || (((take != select.Take) || (skip != select.Skip)) || ((isDistinct != select.IsDistinct) || (columns != select.Columns)))) || (isReverse != select.IsReverse))
            {
                return new SelectExpression(select.Alias, columns, from, where, orderBy, groupBy, isDistinct, skip, take, isReverse);
            }
            return select;
        }

        protected UpdateCommand UpdateUpdate(UpdateCommand update, TableExpression table, Expression where, IEnumerable<ColumnAssignment> assignments)
        {
            if (((table != update.Table) || (where != update.Where)) || (assignments != update.Assignments))
            {
                return new UpdateCommand(table, where, assignments);
            }
            return update;
        }

        protected override Expression Visit(Expression exp)
        {
            if (exp == null)
            {
                return null;
            }
            switch (exp.NodeType)
            {
                case ((ExpressionType) 0x3e8):
                    return this.VisitTable((TableExpression) exp);

                case ((ExpressionType) 0x3e9):
                    return this.VisitClientJoin((ClientJoinExpression) exp);

                case ((ExpressionType) 0x3ea):
                    return this.VisitColumn((ColumnExpression) exp);

                case ((ExpressionType) 0x3eb):
                    return this.VisitSelect((SelectExpression) exp);

                case ((ExpressionType) 0x3ec):
                    return this.VisitProjection((ProjectionExpression) exp);

                case ((ExpressionType) 0x3ed):
                    return this.VisitEntity((EntityExpression) exp);

                case ((ExpressionType) 0x3ee):
                    return this.VisitJoin((JoinExpression) exp);

                case ((ExpressionType) 0x3ef):
                    return this.VisitAggregate((AggregateExpression) exp);

                case ((ExpressionType) 0x3f0):
                case ((ExpressionType) 0x3f1):
                case ((ExpressionType) 0x3f2):
                    return this.VisitSubquery((SubqueryExpression) exp);

                case ((ExpressionType) 0x3f4):
                    return this.VisitAggregateSubquery((AggregateSubqueryExpression) exp);

                case ((ExpressionType) 0x3f5):
                    return this.VisitIsNull((IsNullExpression) exp);

                case ((ExpressionType) 0x3f6):
                    return this.VisitBetween((BetweenExpression) exp);

                case ((ExpressionType) 0x3f7):
                    return this.VisitRowNumber((RowNumberExpression) exp);

                case ((ExpressionType) 0x3f8):
                    return this.VisitNamedValue((NamedValueExpression) exp);

                case ((ExpressionType) 0x3f9):
                    return this.VisitOuterJoined((OuterJoinedExpression) exp);

                case ((ExpressionType) 0x3fa):
                case ((ExpressionType) 0x3fb):
                case ((ExpressionType) 0x3fc):
                case ((ExpressionType) 0x3ff):
                case ((ExpressionType) 0x400):
                case ((ExpressionType) 0x401):
                    return this.VisitCommand((CommandExpression) exp);

                case ((ExpressionType) 0x3fd):
                    return this.VisitBatch((BatchExpression) exp);

                case ((ExpressionType) 0x3fe):
                    return this.VisitFunction((FunctionExpression) exp);

                case ((ExpressionType) 0x402):
                    return this.VisitVariable((VariableExpression) exp);
            }
            return base.Visit(exp);
        }

        protected virtual Expression VisitAggregate(AggregateExpression aggregate)
        {
            Expression arg = this.Visit(aggregate.Argument);
            return this.UpdateAggregate(aggregate, aggregate.Type, aggregate.AggregateName, arg, aggregate.IsDistinct);
        }

        protected virtual Expression VisitAggregateSubquery(AggregateSubqueryExpression aggregate)
        {
            ScalarExpression subquery = (ScalarExpression) this.Visit(aggregate.AggregateAsSubquery);
            return this.UpdateAggregateSubquery(aggregate, subquery);
        }

        protected virtual Expression VisitBatch(BatchExpression batch)
        {
            LambdaExpression operation = (LambdaExpression) this.Visit(batch.Operation);
            Expression batchSize = this.Visit(batch.BatchSize);
            Expression stream = this.Visit(batch.Stream);
            return this.UpdateBatch(batch, batch.Input, operation, batchSize, stream);
        }

        protected virtual Expression VisitBetween(BetweenExpression between)
        {
            Expression expression = this.Visit(between.Expression);
            Expression lower = this.Visit(between.Lower);
            Expression upper = this.Visit(between.Upper);
            return this.UpdateBetween(between, expression, lower, upper);
        }

        protected virtual Expression VisitBlock(BlockCommand block)
        {
            ReadOnlyCollection<Expression> commands = this.VisitExpressionList(block.Commands);
            return this.UpdateBlock(block, commands);
        }

        protected virtual Expression VisitClientJoin(ClientJoinExpression join)
        {
            ProjectionExpression projection = (ProjectionExpression) this.Visit(join.Projection);
            ReadOnlyCollection<Expression> outerKey = this.VisitExpressionList(join.OuterKey);
            ReadOnlyCollection<Expression> innerKey = this.VisitExpressionList(join.InnerKey);
            return this.UpdateClientJoin(join, projection, outerKey, innerKey);
        }

        protected virtual Expression VisitColumn(ColumnExpression column)
        {
            return column;
        }

        protected virtual ColumnAssignment VisitColumnAssignment(ColumnAssignment ca)
        {
            ColumnExpression c = (ColumnExpression) this.Visit(ca.Column);
            Expression e = this.Visit(ca.Expression);
            return this.UpdateColumnAssignment(ca, c, e);
        }

        protected virtual ReadOnlyCollection<ColumnAssignment> VisitColumnAssignments(ReadOnlyCollection<ColumnAssignment> assignments)
        {
            List<ColumnAssignment> list = null;
            int count = 0;
            int num2 = assignments.Count;
            while (count < num2)
            {
                ColumnAssignment item = this.VisitColumnAssignment(assignments[count]);
                if ((list == null) && (item != assignments[count]))
                {
                    list = assignments.Take<ColumnAssignment>(count).ToList<ColumnAssignment>();
                }
                if (list != null)
                {
                    list.Add(item);
                }
                count++;
            }
            if (list != null)
            {
                return list.AsReadOnly();
            }
            return assignments;
        }

        protected virtual ReadOnlyCollection<ColumnDeclaration> VisitColumnDeclarations(ReadOnlyCollection<ColumnDeclaration> columns)
        {
            List<ColumnDeclaration> list = null;
            int count = 0;
            int num2 = columns.Count;
            while (count < num2)
            {
                ColumnDeclaration declaration = columns[count];
                Expression expression = this.Visit(declaration.Expression);
                if ((list == null) && (expression != declaration.Expression))
                {
                    list = columns.Take<ColumnDeclaration>(count).ToList<ColumnDeclaration>();
                }
                if (list != null)
                {
                    list.Add(new ColumnDeclaration(declaration.Name, expression, declaration.QueryType));
                }
                count++;
            }
            if (list != null)
            {
                return list.AsReadOnly();
            }
            return columns;
        }

        protected virtual Expression VisitCommand(CommandExpression command)
        {
            switch (command.NodeType)
            {
                case ((ExpressionType) 0x3fa):
                    return this.VisitInsert((InsertCommand) command);

                case ((ExpressionType) 0x3fb):
                    return this.VisitUpdate((UpdateCommand) command);

                case ((ExpressionType) 0x3fc):
                    return this.VisitDelete((DeleteCommand) command);

                case ((ExpressionType) 0x3ff):
                    return this.VisitBlock((BlockCommand) command);

                case ((ExpressionType) 0x400):
                    return this.VisitIf((IFCommand) command);

                case ((ExpressionType) 0x401):
                    return this.VisitDeclaration((DeclarationCommand) command);
            }
            return this.VisitUnknown(command);
        }

        protected virtual Expression VisitDeclaration(DeclarationCommand decl)
        {
            ReadOnlyCollection<VariableDeclaration> variables = this.VisitVariableDeclarations(decl.Variables);
            SelectExpression source = (SelectExpression) this.Visit(decl.Source);
            return this.UpdateDeclaration(decl, variables, source);
        }

        protected virtual Expression VisitDelete(DeleteCommand delete)
        {
            TableExpression table = (TableExpression) this.Visit(delete.Table);
            Expression where = this.Visit(delete.Where);
            return this.UpdateDelete(delete, table, where);
        }

        protected virtual Expression VisitEntity(EntityExpression entity)
        {
            Expression expression = this.Visit(entity.Expression);
            return this.UpdateEntity(entity, expression);
        }

        protected virtual Expression VisitExists(ExistsExpression exists)
        {
            SelectExpression select = (SelectExpression) this.Visit(exists.Select);
            return this.UpdateExists(exists, select);
        }

        protected virtual Expression VisitFunction(FunctionExpression func)
        {
            ReadOnlyCollection<Expression> arguments = this.VisitExpressionList(func.Arguments);
            return this.UpdateFunction(func, func.Name, arguments);
        }

        protected virtual Expression VisitIf(IFCommand ifx)
        {
            Expression check = this.Visit(ifx.Check);
            Expression ifTrue = this.Visit(ifx.IfTrue);
            Expression ifFalse = this.Visit(ifx.IfFalse);
            return this.UpdateIf(ifx, check, ifTrue, ifFalse);
        }

        protected virtual Expression VisitIn(InExpression @in)
        {
            Expression expression = this.Visit(@in.Expression);
            SelectExpression select = (SelectExpression) this.Visit(@in.Select);
            ReadOnlyCollection<Expression> values = this.VisitExpressionList(@in.Values);
            return this.UpdateIn(@in, expression, select, values);
        }

        protected virtual Expression VisitInsert(InsertCommand insert)
        {
            TableExpression table = (TableExpression) this.Visit(insert.Table);
            ReadOnlyCollection<ColumnAssignment> assignments = this.VisitColumnAssignments(insert.Assignments);
            return this.UpdateInsert(insert, table, assignments);
        }

        protected virtual Expression VisitIsNull(IsNullExpression isnull)
        {
            Expression expression = this.Visit(isnull.Expression);
            return this.UpdateIsNull(isnull, expression);
        }

        protected virtual Expression VisitJoin(JoinExpression join)
        {
            Expression left = this.VisitSource(join.Left);
            Expression right = this.VisitSource(join.Right);
            Expression condition = this.Visit(join.Condition);
            return this.UpdateJoin(join, join.Join, left, right, condition);
        }

        protected virtual Expression VisitNamedValue(NamedValueExpression value)
        {
            return value;
        }

        protected virtual ReadOnlyCollection<OrderExpression> VisitOrderBy(ReadOnlyCollection<OrderExpression> expressions)
        {
            if (expressions != null)
            {
                List<OrderExpression> list = null;
                int count = 0;
                int num2 = expressions.Count;
                while (count < num2)
                {
                    OrderExpression expression = expressions[count];
                    Expression expression2 = this.Visit(expression.Expression);
                    if ((list == null) && (expression2 != expression.Expression))
                    {
                        list = expressions.Take<OrderExpression>(count).ToList<OrderExpression>();
                    }
                    if (list != null)
                    {
                        list.Add(new OrderExpression(expression.OrderType, expression2));
                    }
                    count++;
                }
                if (list != null)
                {
                    return list.AsReadOnly();
                }
            }
            return expressions;
        }

        protected virtual Expression VisitOuterJoined(OuterJoinedExpression outer)
        {
            Expression test = this.Visit(outer.Test);
            Expression expression = this.Visit(outer.Expression);
            return this.UpdateOuterJoined(outer, test, expression);
        }

        protected virtual Expression VisitProjection(ProjectionExpression proj)
        {
            SelectExpression select = (SelectExpression) this.Visit(proj.Select);
            Expression projector = this.Visit(proj.Projector);
            return this.UpdateProjection(proj, select, projector, proj.Aggregator);
        }

        protected virtual Expression VisitRowNumber(RowNumberExpression rowNumber)
        {
            ReadOnlyCollection<OrderExpression> orderBy = this.VisitOrderBy(rowNumber.OrderBy);
            return this.UpdateRowNumber(rowNumber, orderBy);
        }

        protected virtual Expression VisitScalar(ScalarExpression scalar)
        {
            SelectExpression select = (SelectExpression) this.Visit(scalar.Select);
            return this.UpdateScalar(scalar, select);
        }

        protected virtual Expression VisitSelect(SelectExpression select)
        {
            Expression from = this.VisitSource(select.From);
            Expression where = this.Visit(select.Where);
            ReadOnlyCollection<OrderExpression> orderBy = this.VisitOrderBy(select.OrderBy);
            ReadOnlyCollection<Expression> groupBy = this.VisitExpressionList(select.GroupBy);
            Expression skip = this.Visit(select.Skip);
            Expression take = this.Visit(select.Take);
            ReadOnlyCollection<ColumnDeclaration> columns = this.VisitColumnDeclarations(select.Columns);
            return this.UpdateSelect(select, from, where, orderBy, groupBy, skip, take, select.IsDistinct, select.IsReverse, columns);
        }

        protected virtual Expression VisitSource(Expression source)
        {
            return this.Visit(source);
        }

        protected virtual Expression VisitSubquery(SubqueryExpression subquery)
        {
            switch (subquery.NodeType)
            {
                case ((ExpressionType) 0x3f0):
                    return this.VisitScalar((ScalarExpression) subquery);

                case ((ExpressionType) 0x3f1):
                    return this.VisitExists((ExistsExpression) subquery);

                case ((ExpressionType) 0x3f2):
                    return this.VisitIn((InExpression) subquery);
            }
            return subquery;
        }

        protected virtual Expression VisitTable(TableExpression table)
        {
            return table;
        }

        protected virtual Expression VisitUpdate(UpdateCommand update)
        {
            TableExpression table = (TableExpression) this.Visit(update.Table);
            Expression where = this.Visit(update.Where);
            ReadOnlyCollection<ColumnAssignment> assignments = this.VisitColumnAssignments(update.Assignments);
            return this.UpdateUpdate(update, table, where, assignments);
        }

        protected virtual Expression VisitVariable(VariableExpression vex)
        {
            return vex;
        }

        protected virtual ReadOnlyCollection<VariableDeclaration> VisitVariableDeclarations(ReadOnlyCollection<VariableDeclaration> decls)
        {
            List<VariableDeclaration> list = null;
            int count = 0;
            int num2 = decls.Count;
            while (count < num2)
            {
                VariableDeclaration declaration = decls[count];
                Expression expression = this.Visit(declaration.Expression);
                if ((list == null) && (expression != declaration.Expression))
                {
                    list = decls.Take<VariableDeclaration>(count).ToList<VariableDeclaration>();
                }
                if (list != null)
                {
                    list.Add(new VariableDeclaration(declaration.Name, declaration.QueryType, expression));
                }
                count++;
            }
            if (list != null)
            {
                return list.AsReadOnly();
            }
            return decls;
        }
    }
}
