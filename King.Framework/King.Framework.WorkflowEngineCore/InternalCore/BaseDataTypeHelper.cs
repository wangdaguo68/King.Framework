namespace King.Framework.WorkflowEngineCore.InternalCore
{
    using King.Framework.EntityLibrary;
    using System;

    internal abstract class BaseDataTypeHelper
    {
        private readonly DataTypeEnum _dataType;

        public BaseDataTypeHelper(DataTypeEnum data_type)
        {
            this._dataType = data_type;
        }

        public abstract object ConvertFromString(string s);

        public DataTypeEnum DataType
        {
            get
            {
                return this._dataType;
            }
        }

        public abstract Type TargetType { get; }
    }
}

