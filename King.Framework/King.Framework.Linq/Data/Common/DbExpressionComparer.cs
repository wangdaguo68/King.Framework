namespace King.Framework.Linq.Data.Common
{
    using King.Framework.Linq;
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;

    public class DbExpressionComparer : ExpressionComparer
    {
        private ScopedDictionary<TableAlias, TableAlias> aliasScope;

        protected DbExpressionComparer(ScopedDictionary<ParameterExpression, ParameterExpression> parameterScope, Func<object, object, bool> fnCompare, ScopedDictionary<TableAlias, TableAlias> aliasScope) : base(parameterScope, fnCompare)
        {
            this.aliasScope = aliasScope;
        }

        public static bool AreEqual(Expression a, Expression b)
        {
            return AreEqual(null, null, a, b, null);
        }

        public static bool AreEqual(Expression a, Expression b, Func<object, object, bool> fnCompare)
        {
            return AreEqual(null, null, a, b, fnCompare);
        }

        public static bool AreEqual(ScopedDictionary<ParameterExpression, ParameterExpression> parameterScope, ScopedDictionary<TableAlias, TableAlias> aliasScope, Expression a, Expression b)
        {
            return new DbExpressionComparer(parameterScope, null, aliasScope).Compare(a, b);
        }

        public static bool AreEqual(ScopedDictionary<ParameterExpression, ParameterExpression> parameterScope, ScopedDictionary<TableAlias, TableAlias> aliasScope, Expression a, Expression b, Func<object, object, bool> fnCompare)
        {
            return new DbExpressionComparer(parameterScope, fnCompare, aliasScope).Compare(a, b);
        }

        protected override bool Compare(Expression a, Expression b)
        {
            if (a == b)
            {
                return true;
            }
            if ((a == null) || (b == null))
            {
                return false;
            }
            if (a.NodeType != b.NodeType)
            {
                return false;
            }
            if (a.Type != b.Type)
            {
                return false;
            }
            switch (a.NodeType)
            {
                case ((ExpressionType) 0x3e8):
                    return this.CompareTable((TableExpression) a, (TableExpression) b);

                case ((ExpressionType) 0x3ea):
                    return this.CompareColumn((ColumnExpression) a, (ColumnExpression) b);

                case ((ExpressionType) 0x3eb):
                    return this.CompareSelect((SelectExpression) a, (SelectExpression) b);

                case ((ExpressionType) 0x3ec):
                    return this.CompareProjection((ProjectionExpression) a, (ProjectionExpression) b);

                case ((ExpressionType) 0x3ed):
                    return this.CompareEntity((EntityExpression) a, (EntityExpression) b);

                case ((ExpressionType) 0x3ee):
                    return this.CompareJoin((JoinExpression) a, (JoinExpression) b);

                case ((ExpressionType) 0x3ef):
                    return this.CompareAggregate((AggregateExpression) a, (AggregateExpression) b);

                case ((ExpressionType) 0x3f0):
                case ((ExpressionType) 0x3f1):
                case ((ExpressionType) 0x3f2):
                    return this.CompareSubquery((SubqueryExpression) a, (SubqueryExpression) b);

                case ((ExpressionType) 0x3f4):
                    return this.CompareAggregateSubquery((AggregateSubqueryExpression) a, (AggregateSubqueryExpression) b);

                case ((ExpressionType) 0x3f5):
                    return this.CompareIsNull((IsNullExpression) a, (IsNullExpression) b);

                case ((ExpressionType) 0x3f6):
                    return this.CompareBetween((BetweenExpression) a, (BetweenExpression) b);

                case ((ExpressionType) 0x3f7):
                    return this.CompareRowNumber((RowNumberExpression) a, (RowNumberExpression) b);

                case ((ExpressionType) 0x3f8):
                    return this.CompareNamedValue((NamedValueExpression) a, (NamedValueExpression) b);

                case ((ExpressionType) 0x3fa):
                    return this.CompareInsert((InsertCommand) a, (InsertCommand) b);

                case ((ExpressionType) 0x3fb):
                    return this.CompareUpdate((UpdateCommand) a, (UpdateCommand) b);

                case ((ExpressionType) 0x3fc):
                    return this.CompareDelete((DeleteCommand) a, (DeleteCommand) b);

                case ((ExpressionType) 0x3fd):
                    return this.CompareBatch((BatchExpression) a, (BatchExpression) b);

                case ((ExpressionType) 0x3fe):
                    return this.CompareFunction((FunctionExpression) a, (FunctionExpression) b);

                case ((ExpressionType) 0x3ff):
                    return this.CompareBlock((BlockCommand) a, (BlockCommand) b);

                case ((ExpressionType) 0x400):
                    return this.CompareIf((IFCommand) a, (IFCommand) b);
            }
            return base.Compare(a, b);
        }

        protected virtual bool CompareAggregate(AggregateExpression a, AggregateExpression b)
        {
            return ((a.AggregateName == b.AggregateName) && this.Compare(a.Argument, b.Argument));
        }

        protected virtual bool CompareAggregateSubquery(AggregateSubqueryExpression a, AggregateSubqueryExpression b)
        {
            return ((this.Compare(a.AggregateAsSubquery, b.AggregateAsSubquery) && this.Compare(a.AggregateInGroupSelect, b.AggregateInGroupSelect)) && (a.GroupByAlias == b.GroupByAlias));
        }

        protected virtual bool CompareAlias(TableAlias a, TableAlias b)
        {
            TableAlias alias;
            if ((this.aliasScope != null) && this.aliasScope.TryGetValue(a, out alias))
            {
                return (alias == b);
            }
            return (a == b);
        }

        protected virtual bool CompareBatch(BatchExpression x, BatchExpression y)
        {
            return (((this.Compare(x.Input, y.Input) && this.Compare(x.Operation, y.Operation)) && this.Compare(x.BatchSize, y.BatchSize)) && this.Compare(x.Stream, y.Stream));
        }

        protected virtual bool CompareBetween(BetweenExpression a, BetweenExpression b)
        {
            return ((this.Compare(a.Expression, b.Expression) && this.Compare(a.Lower, b.Lower)) && this.Compare(a.Upper, b.Upper));
        }

        protected virtual bool CompareBlock(BlockCommand x, BlockCommand y)
        {
            if (x.Commands.Count != y.Commands.Count)
            {
                return false;
            }
            int num = 0;
            int count = x.Commands.Count;
            while (num < count)
            {
                if (!this.Compare(x.Commands[num], y.Commands[num]))
                {
                    return false;
                }
                num++;
            }
            return true;
        }

        protected virtual bool CompareColumn(ColumnExpression a, ColumnExpression b)
        {
            return (this.CompareAlias(a.Alias, b.Alias) && (a.Name == b.Name));
        }

        protected virtual bool CompareColumnAssignments(ReadOnlyCollection<ColumnAssignment> x, ReadOnlyCollection<ColumnAssignment> y)
        {
            if (x != y)
            {
                if (x.Count != y.Count)
                {
                    return false;
                }
                int num = 0;
                int count = x.Count;
                while (num < count)
                {
                    if (!(this.Compare(x[num].Column, y[num].Column) && this.Compare(x[num].Expression, y[num].Expression)))
                    {
                        return false;
                    }
                    num++;
                }
            }
            return true;
        }

        protected virtual bool CompareColumnDeclaration(ColumnDeclaration a, ColumnDeclaration b)
        {
            return ((a.Name == b.Name) && this.Compare(a.Expression, b.Expression));
        }

        protected virtual bool CompareColumnDeclarations(ReadOnlyCollection<ColumnDeclaration> a, ReadOnlyCollection<ColumnDeclaration> b)
        {
            if (a != b)
            {
                if ((a == null) || (b == null))
                {
                    return false;
                }
                if (a.Count != b.Count)
                {
                    return false;
                }
                int num = 0;
                int count = a.Count;
                while (num < count)
                {
                    if (!this.CompareColumnDeclaration(a[num], b[num]))
                    {
                        return false;
                    }
                    num++;
                }
            }
            return true;
        }

        protected virtual bool CompareDelete(DeleteCommand x, DeleteCommand y)
        {
            return (this.Compare(x.Table, y.Table) && this.Compare(x.Where, y.Where));
        }

        protected virtual bool CompareEntity(EntityExpression x, EntityExpression y)
        {
            return ((x.Entity == y.Entity) && this.Compare(x.Expression, y.Expression));
        }

        protected virtual bool CompareExists(ExistsExpression a, ExistsExpression b)
        {
            return this.Compare(a.Select, b.Select);
        }

        protected virtual bool CompareFunction(FunctionExpression x, FunctionExpression y)
        {
            return ((x.Name == y.Name) && this.CompareExpressionList(x.Arguments, y.Arguments));
        }

        protected virtual bool CompareIf(IFCommand x, IFCommand y)
        {
            return ((this.Compare(x.Check, y.Check) && this.Compare(x.IfTrue, y.IfTrue)) && this.Compare(x.IfFalse, y.IfFalse));
        }

        protected virtual bool CompareIn(InExpression a, InExpression b)
        {
            return ((this.Compare(a.Expression, b.Expression) && this.Compare(a.Select, b.Select)) && this.CompareExpressionList(a.Values, b.Values));
        }

        protected virtual bool CompareInsert(InsertCommand x, InsertCommand y)
        {
            return (this.Compare(x.Table, y.Table) && this.CompareColumnAssignments(x.Assignments, y.Assignments));
        }

        protected virtual bool CompareIsNull(IsNullExpression a, IsNullExpression b)
        {
            return this.Compare(a.Expression, b.Expression);
        }

        protected virtual bool CompareJoin(JoinExpression a, JoinExpression b)
        {
            if (!((a.Join == b.Join) && this.Compare(a.Left, b.Left)))
            {
                return false;
            }
            if ((a.Join == JoinType.CrossApply) || (a.Join == JoinType.OuterApply))
            {
                ScopedDictionary<TableAlias, TableAlias> aliasScope = this.aliasScope;
                try
                {
                    this.aliasScope = new ScopedDictionary<TableAlias, TableAlias>(this.aliasScope);
                    this.MapAliases(a.Left, b.Left);
                    return (this.Compare(a.Right, b.Right) && this.Compare(a.Condition, b.Condition));
                }
                finally
                {
                    this.aliasScope = aliasScope;
                }
            }
            return (this.Compare(a.Right, b.Right) && this.Compare(a.Condition, b.Condition));
        }

        protected virtual bool CompareNamedValue(NamedValueExpression a, NamedValueExpression b)
        {
            return ((a.Name == b.Name) && this.Compare(a.Value, b.Value));
        }

        protected virtual bool CompareOrderList(ReadOnlyCollection<OrderExpression> a, ReadOnlyCollection<OrderExpression> b)
        {
            if (a != b)
            {
                if ((a == null) || (b == null))
                {
                    return false;
                }
                if (a.Count != b.Count)
                {
                    return false;
                }
                int num = 0;
                int count = a.Count;
                while (num < count)
                {
                    if (!((a[num].OrderType == b[num].OrderType) && this.Compare(a[num].Expression, b[num].Expression)))
                    {
                        return false;
                    }
                    num++;
                }
            }
            return true;
        }

        protected virtual bool CompareProjection(ProjectionExpression a, ProjectionExpression b)
        {
            bool flag;
            if (!this.Compare(a.Select, b.Select))
            {
                return false;
            }
            ScopedDictionary<TableAlias, TableAlias> aliasScope = this.aliasScope;
            try
            {
                this.aliasScope = new ScopedDictionary<TableAlias, TableAlias>(this.aliasScope);
                this.aliasScope.Add(a.Select.Alias, b.Select.Alias);
                flag = (this.Compare(a.Projector, b.Projector) && this.Compare(a.Aggregator, b.Aggregator)) && (a.IsSingleton == b.IsSingleton);
            }
            finally
            {
                this.aliasScope = aliasScope;
            }
            return flag;
        }

        protected virtual bool CompareRowNumber(RowNumberExpression a, RowNumberExpression b)
        {
            return this.CompareOrderList(a.OrderBy, b.OrderBy);
        }

        protected virtual bool CompareScalar(ScalarExpression a, ScalarExpression b)
        {
            return this.Compare(a.Select, b.Select);
        }

        protected virtual bool CompareSelect(SelectExpression a, SelectExpression b)
        {
            bool flag;
            ScopedDictionary<TableAlias, TableAlias> aliasScope = this.aliasScope;
            try
            {
                if (!this.Compare(a.From, b.From))
                {
                    return false;
                }
                this.aliasScope = new ScopedDictionary<TableAlias, TableAlias>(aliasScope);
                this.MapAliases(a.From, b.From);
                flag = (((this.Compare(a.Where, b.Where) && this.CompareOrderList(a.OrderBy, b.OrderBy)) && (this.CompareExpressionList(a.GroupBy, b.GroupBy) && this.Compare(a.Skip, b.Skip))) && ((this.Compare(a.Take, b.Take) && (a.IsDistinct == b.IsDistinct)) && (a.IsReverse == b.IsReverse))) && this.CompareColumnDeclarations(a.Columns, b.Columns);
            }
            finally
            {
                this.aliasScope = aliasScope;
            }
            return flag;
        }

        protected virtual bool CompareSubquery(SubqueryExpression a, SubqueryExpression b)
        {
            if (a.NodeType == b.NodeType)
            {
                switch (a.NodeType)
                {
                    case ((ExpressionType) 0x3f0):
                        return this.CompareScalar((ScalarExpression) a, (ScalarExpression) b);

                    case ((ExpressionType) 0x3f1):
                        return this.CompareExists((ExistsExpression) a, (ExistsExpression) b);

                    case ((ExpressionType) 0x3f2):
                        return this.CompareIn((InExpression) a, (InExpression) b);
                }
            }
            return false;
        }

        protected virtual bool CompareTable(TableExpression a, TableExpression b)
        {
            return (a.Name == b.Name);
        }

        protected virtual bool CompareUpdate(UpdateCommand x, UpdateCommand y)
        {
            return ((this.Compare(x.Table, y.Table) && this.Compare(x.Where, y.Where)) && this.CompareColumnAssignments(x.Assignments, y.Assignments));
        }

        private void MapAliases(Expression a, Expression b)
        {
            TableAlias[] aliasArray = DeclaredAliasGatherer.Gather(a).ToArray<TableAlias>();
            TableAlias[] aliasArray2 = DeclaredAliasGatherer.Gather(b).ToArray<TableAlias>();
            int index = 0;
            int length = aliasArray.Length;
            while (index < length)
            {
                this.aliasScope.Add(aliasArray[index], aliasArray2[index]);
                index++;
            }
        }
    }
}
