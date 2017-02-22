namespace King.Framework.Common
{
    using King.Framework.Interfaces;
    using System;

    public abstract class BaseBLL
    {
        private readonly IUserIdentity _user;

        public BaseBLL() : this(null)
        {
        }

        public BaseBLL(IUserIdentity userIdentity)
        {
            this._user = userIdentity;
        }

        public IUserIdentity CurrentUser
        {
            get
            {
                return this._user;
            }
        }
    }
}
