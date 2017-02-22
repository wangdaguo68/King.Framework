namespace King.Framework.EntityLibrary
{
    using System;

    public enum OperationType
    {
        Add = 1,
        Custom = 5,
        Delete = 4,
        HistoryTrack = 0x18,
        LogicDelete = 20,
        M_Add = 0x26,
        M_Approve = 0x2c,
        M_Delete = 0x23,
        M_Delete_Refresh = 0x24,
        M_Relation_Add = 0x1c,
        M_Relation_Edit = 0x1d,
        M_Return = 0x20,
        M_Save = 0x1a,
        M_Save_Refresh = 0x1b,
        M_Search = 0x2b,
        M_Update = 30,
        M_Update_Refresh = 0x1f,
        MMRelationAdd = 7,
        MMRelationSave = 8,
        ProcessActivityFinish = 0x13,
        ProcessSelect = 0x12,
        ProcessSingle = 0x11,
        ProcessTrack = 0x17,
        RelationAdd = 6,
        Return = 3,
        RowCustom = 15,
        RowDelete = 13,
        RowDetail = 12,
        RowEdit = 11,
        RowHistoryTrack = 0x19,
        RowHref = 14,
        RowLogicDelete = 0x15,
        RowMMDelete = 0x10,
        RowSharePrivilege = 0x65,
        Save = 2
    }
}

