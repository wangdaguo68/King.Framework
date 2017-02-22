namespace King.Framework.DAL
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Property)]
    public class KingColumnAttribute : Attribute
    {
        private int _maxLength = 0;

        public string ColumnName { get; set; }

        public Type DataType { get; set; }

        public bool IsIdentity { get; set; }

        public bool IsPrimaryKey { get; set; }

        public int KeyOrder { get; set; }

        public int MaxLength
        {
            get
            {
                return this._maxLength;
            }
            set
            {
                this._maxLength = value;
            }
        }
    }
}
