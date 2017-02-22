namespace King.Framework.Interfaces
{
    using System;

    public interface IMetaField
    {
        string DisplayText { get; }

        long FieldId { get; }

        string FieldName { get; }
    }
}

