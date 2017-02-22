namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Linq.Expressions;

    public class EntityExpression : DbExpression
    {
        private MappingEntity entity;
        private System.Linq.Expressions.Expression expression;

        public EntityExpression(MappingEntity entity, System.Linq.Expressions.Expression expression) : base(DbExpressionType.Entity, expression.Type)
        {
            this.entity = entity;
            this.expression = expression;
        }

        public MappingEntity Entity
        {
            get
            {
                return this.entity;
            }
        }

        public System.Linq.Expressions.Expression Expression
        {
            get
            {
                return this.expression;
            }
        }
    }
}
