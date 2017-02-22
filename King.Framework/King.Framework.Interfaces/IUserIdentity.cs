namespace King.Framework.Interfaces
{
    using System;
    using System.Collections.Generic;

    public interface IUserIdentity
    {
        int? CompanyID { get; }

        int? Department_ID { get; }

        bool? IsRootAdmin { get; }

        string LoginName { get; }

        List<int> Roles { get; }

        string SsoToken { get; set; }

        string User_Code { get; }

        string User_EMail { get; }

        int User_ID { get; }

        string User_Mobile { get; }

        string User_Name { get; }
    }
}

