namespace King.Framework.Interfaces
{
    using System;

    public interface IUser
    {
        int? Department_ID { get; set; }

        string User_Code { get; set; }

        string User_EMail { get; set; }

        int User_ID { get; set; }

        string User_Mobile { get; set; }

        string User_Name { get; set; }
    }
}

