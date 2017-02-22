namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public class BatchExpression : Expression
    {
        private readonly ExpressionType _nodeType = ((ExpressionType)0x3fd);
        private readonly System.Type _type;
        private Expression batchSize;
        private Expression input;
        private LambdaExpression operation;
        private Expression stream;

        public BatchExpression(Expression input, LambdaExpression operation, Expression batchSize, Expression stream)
        {
            this._type = typeof(IEnumerable<>).MakeGenericType(new System.Type[] { operation.Body.Type });
            this.input = input;
            this.operation = operation;
            this.batchSize = batchSize;
            this.stream = stream;
        }

        public Expression BatchSize
        {
            get
            {
                return this.batchSize;
            }
        }

        public Expression Input
        {
            get
            {
                return this.input;
            }
        }

        public override ExpressionType NodeType
        {
            get
            {
                return this._nodeType;
            }
        }

        public LambdaExpression Operation
        {
            get
            {
                return this.operation;
            }
        }

        public Expression Stream
        {
            get
            {
                return this.stream;
            }
        }

        public override System.Type Type
        {
            get
            {
                return this._type;
            }
        }
    }
}
