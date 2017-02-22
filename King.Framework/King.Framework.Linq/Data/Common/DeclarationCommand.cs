namespace King.Framework.Linq.Data.Common
{
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class DeclarationCommand : CommandExpression
    {
        private SelectExpression source;
        private ReadOnlyCollection<VariableDeclaration> variables;

        public DeclarationCommand(IEnumerable<VariableDeclaration> variables, SelectExpression source) : base(DbExpressionType.Declaration, typeof(void))
        {
            this.variables = variables.ToReadOnly<VariableDeclaration>();
            this.source = source;
        }

        public SelectExpression Source
        {
            get
            {
                return this.source;
            }
        }

        public ReadOnlyCollection<VariableDeclaration> Variables
        {
            get
            {
                return this.variables;
            }
        }
    }
}
