namespace King.Framework.DAL
{
    using King.Framework.DAL.CSharpTypes;
    using System;

    internal class InternalColumnWrapper
    {
        public readonly Type ColType;
        public readonly string ColumnName;
        public readonly int Index;
        public readonly ISharpType TypeHelper;

        public InternalColumnWrapper(int index, Type colType, ISharpType typeHelper, string columnName)
        {
            this.Index = index;
            this.ColType = colType;
            this.TypeHelper = typeHelper;
            this.ColumnName = columnName;
        }
    }
}
