namespace King.Framework.DAL
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class TableData
    {
        private string[] _columns;
        private List<string[]> _rowDatas;

        public TableData(string[] columns, int rowCount)
        {
            this._columns = new string[columns.Length];
            columns.CopyTo(this._columns, 0);
            this._rowDatas = new List<string[]>(rowCount);
        }

        internal void Append(string[] row)
        {
            this._rowDatas.Add(row);
        }

        public string[] GetRow(int index)
        {
            return this._rowDatas[index];
        }

        public ReadOnlyCollection<string[]> GetRows()
        {
            return this._rowDatas.AsReadOnly();
        }

        public string[] Columns
        {
            get
            {
                return this._columns;
            }
        }

        public int RowCount
        {
            get
            {
                return this._rowDatas.Count;
            }
        }
    }
}
