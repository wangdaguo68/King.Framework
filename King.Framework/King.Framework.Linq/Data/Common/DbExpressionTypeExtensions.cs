namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;

    public static class DbExpressionTypeExtensions
    {
        public static bool IsDbExpression(this ExpressionType et)
        {
            return (et >= ((ExpressionType) 0x3e8));
        }
    }
}
