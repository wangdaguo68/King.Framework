namespace King.Framework.Linq.Data.Common
{
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;

    public class BlockCommand : CommandExpression
    {
        private ReadOnlyCollection<Expression> commands;

        public BlockCommand(IList<Expression> commands) : base(DbExpressionType.Block, commands[commands.Count - 1].Type)
        {
            this.commands = commands.ToReadOnly<Expression>();
        }

        public BlockCommand(params Expression[] commands) : this((IList<Expression>)commands)
        {
        }

        public ReadOnlyCollection<Expression> Commands
        {
            get
            {
                return this.commands;
            }
        }
    }
}
