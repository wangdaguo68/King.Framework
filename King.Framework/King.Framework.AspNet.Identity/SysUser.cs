namespace King.Framework.AspNet.Identity
{
    using King.Framework.DAL;
    using King.Framework.Interfaces;
    using Microsoft.AspNet.Identity;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    [Serializable, KingTable("T_User")]
    public class SysUser : ISerializable, IUserIdentity, Microsoft.AspNet.Identity.IUser
    {
        private int? _CompanyId;
        private int? _DepartmentId;
        private string _loginName;
        private string _Password;
        private string _PasswordSalt;
        private readonly List<int> _roles;
        private string _UserCode;
        private int _UserId;
        private string _UserName;
        private int? _UserState;
        private int? _UserType;

        public SysUser()
        {
            this._roles = new List<int>();
        }

        public SysUser(SerializationInfo info, StreamingContext context)
        {
            this._roles = new List<int>();
            int num = 1;
            try
            {
                num = info.GetInt32("__Version");
            }
            catch
            {
                num = 1;
            }
            this._UserId = info.GetInt32("UserId");
            this._loginName = info.GetString("LoginName");
            this._Password = info.GetString("Password");
            this._UserName = info.GetString("UserName");
            this._CompanyId = new int?(info.GetInt32("CompanyId"));
            this._DepartmentId = new int?(info.GetInt32("DepartmentId"));
            this._UserCode = info.GetString("UserCode");
            this._UserState = new int?(info.GetInt32("UserState"));
            this._UserType = new int?(info.GetInt32("UserType"));
            this.User_EMail = info.GetString("User_EMail");
            this.User_Mobile = info.GetString("User_Mobile");
            this.IsRootAdmin = new bool?(info.GetBoolean("IsRootAdmin"));
            try
            {
                this._roles = info.GetValue("Roles", typeof(List<int>)) as List<int>;
            }
            catch
            {
                this._roles = new List<int>();
            }
            if (this._roles == null)
            {
                this._roles = new List<int>();
            }
            if (num > 1)
            {
                this.SsoToken = info.GetString("SsoToken");
            }
            if (num > 2)
            {
                this.DbProvider = info.GetString("DbProvider");
                this.DbConnectionString = info.GetString("DbConnectionString");
            }
            if (num > 3)
            {
                long ticks = info.GetInt64("LastTokenBeatTime");
                if (ticks > 0L)
                {
                    this.LastTokenBeatTime = new DateTime(ticks);
                }
            }
        }

        public static SysUser FromJson(string json)
        {
            return JsonConvert.DeserializeObject<SysUser>(json);
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter=true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("__Version", this.__Version);
            info.AddValue("UserId", this._UserId);
            info.AddValue("LoginName", this._loginName);
            info.AddValue("Password", string.Empty);
            info.AddValue("UserName", this._UserName);
            int? nullable = this._CompanyId;
            info.AddValue("CompanyId", nullable.HasValue ? nullable.GetValueOrDefault() : -1);
            nullable = this._DepartmentId;
            info.AddValue("DepartmentId", nullable.HasValue ? nullable.GetValueOrDefault() : -1);
            info.AddValue("UserCode", this._UserCode);
            nullable = this._UserState;
            info.AddValue("UserState", nullable.HasValue ? nullable.GetValueOrDefault() : -1);
            nullable = this._UserType;
            info.AddValue("UserType", nullable.HasValue ? nullable.GetValueOrDefault() : -1);
            info.AddValue("User_EMail", this.User_EMail);
            info.AddValue("User_Mobile", this.User_Mobile);
            bool? isRootAdmin = this.IsRootAdmin;
            info.AddValue("IsRootAdmin", isRootAdmin.HasValue ? isRootAdmin.GetValueOrDefault() : false);
            info.AddValue("Roles", this.Roles);
            info.AddValue("SsoToken", this.SsoToken);
            info.AddValue("DbProvider", this.DbProvider);
            info.AddValue("DbConnectionString", this.DbConnectionString);
            if (this.LastTokenBeatTime.HasValue)
            {
                info.AddValue("LastTokenBeatTime", this.LastTokenBeatTime.Value.Ticks);
            }
            else
            {
                info.AddValue("LastTokenBeatTime", (long) 0L);
            }
        }

        public string ToJason()
        {
            return JsonConvert.SerializeObject(this);
        }

        private int __Version
        {
            get
            {
                return 4;
            }
        }

        [KingColumn]
        public int? CompanyID
        {
            get
            {
                return this._CompanyId;
            }
            set
            {
                this._CompanyId = value;
            }
        }

        public string DbConnectionString { get; set; }

        public string DbProvider { get; set; }

        [KingColumn]
        public int? Department_ID
        {
            get
            {
                return this._DepartmentId;
            }
            set
            {
                this._DepartmentId = value;
            }
        }

        int? IUserIdentity.CompanyID
        {
            get
            {
                return this.CompanyID;
            }
        }

        int? IUserIdentity.Department_ID
        {
            get
            {
                return this.Department_ID;
            }
        }

        bool? IUserIdentity.IsRootAdmin
        {
            get
            {
                return this.IsRootAdmin;
            }
        }

        string IUserIdentity.LoginName
        {
            get
            {
                return this.LoginName;
            }
        }

        List<int> IUserIdentity.Roles
        {
            get
            {
                return this._roles;
            }
        }

        string IUserIdentity.User_Code
        {
            get
            {
                return this.User_Code;
            }
        }

        string IUserIdentity.User_EMail
        {
            get
            {
                return this.User_EMail;
            }
        }

        int IUserIdentity.User_ID
        {
            get
            {
                return this.User_ID;
            }
        }

        string IUserIdentity.User_Mobile
        {
            get
            {
                return this.User_Mobile;
            }
        }

        string IUserIdentity.User_Name
        {
            get
            {
                return this._UserName;
            }
        }

        public string Id
        {
            get
            {
                return this._UserId.ToString();
            }
            set
            {
                this._UserId = string.IsNullOrEmpty(value) ? -1 : int.Parse(value);
            }
        }

        public string UserName { get; set; }

        [KingColumn]
        public bool? IsLock { get; set; }

        [KingColumn]
        public bool? IsRootAdmin { get; set; }

        public DateTime? LastTokenBeatTime { get; set; }

        [KingColumn]
        public string LoginName
        {
            get
            {
                return this._loginName;
            }
            set
            {
                this._loginName = value;
            }
        }



        [KingColumn]
        public string PasswordSalt
        {
            get
            {
                return this._PasswordSalt;
            }
            set
            {
                this._PasswordSalt = value;
            }
        }

        public List<int> Roles
        {
            get
            {
                return this._roles;
            }
        }

        public string SsoToken { get; set; }

        [KingColumn]
        public string User_Code
        {
            get
            {
                return this._UserCode;
            }
            set
            {
                this._UserCode = value;
            }
        }

        [KingColumn]
        public string User_EMail { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public int User_ID
        {
            get
            {
                return this._UserId;
            }
            set
            {
                this._UserId = value;
            }
        }

        [KingColumn]
        public string User_Mobile { get; set; }

        [KingColumn]
        public string User_Name
        {
            get
            {
                return this._UserName;
            }
            set
            {
                this._UserName = value;
            }
        }

        [KingColumn]
        public string User_Password
        {
            get
            {
                return this._Password;
            }
            set
            {
                this._Password = value;
            }
        }

        [KingColumn]
        public int? User_Status
        {
            get
            {
                return this._UserState;
            }
            set
            {
                this._UserState = value;
            }
        }

        [KingColumn]
        public int? User_Type
        {
            get
            {
                return this._UserType;
            }
            set
            {
                this._UserType = value;
            }
        }
    }
}

