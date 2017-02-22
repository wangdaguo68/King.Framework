namespace King.Framework.DAL
{
    using System;

    public class ColumnWrapper
    {
        public readonly Column col;
        public readonly int Index;
        public readonly bool IsUsed;

        public ColumnWrapper(int index, Column col)
        {
            this.Index = index;
            this.col = col;
            this.IsUsed = index >= 0;
        }
    }
}
