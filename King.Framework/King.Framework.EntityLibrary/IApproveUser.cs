namespace King.Framework.EntityLibrary
{
    using System;

    public interface IApproveUser
    {
        int? FlagInt { get; set; }

        string FlagString { get; set; }

        bool? IsMajor { get; set; }

        int? UserId { get; set; }
    }
}

