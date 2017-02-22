namespace King.Framework.AspNet.Identity
{
    using Microsoft.AspNet.Identity;
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Security.Claims;

    public static class ClaimsIdentityExtensions
    {
        private static readonly ClaimsIdentityFactory<SysUser> _factory = new ClaimsIdentityFactory<SysUser>();

        private static void Add(this ClaimsIdentity identity, string type, object value)
        {
            identity.AddClaim(new Claim(type, (value == null) ? string.Empty : value.ToString()));
        }

        private static void AddClaim(this ClaimsIdentity identity, string type, int? value)
        {
            if (!value.HasValue)
            {
                identity.AddClaim(new Claim(type, string.Empty));
            }
            else
            {
                identity.AddClaim(new Claim(type, value.ToString()));
            }
        }

        private static void AddClaim(this ClaimsIdentity identity, string type, string value)
        {
            if (value == null)
            {
                value = string.Empty;
            }
            identity.AddClaim(new Claim(type, value));
        }

        public static void AddUserData(this ClaimsIdentity identity, SysUser user)
        {
            identity.AddClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/userdata", user.ToJason());
        }

        private static int? GetAsInt2(this ClaimsIdentity identity, string type)
        {
            Claim claim = identity.FindFirst(type);
            if (claim == null)
            {
                return null;
            }
            if (string.IsNullOrWhiteSpace(claim.Value))
            {
                return null;
            }
            return new int?(int.Parse(claim.Value));
        }

        public static string GetAsString(this ClaimsIdentity identity, string type)
        {
            Claim claim = identity.FindFirst(type);
            if (claim == null)
            {
                return null;
            }
            return claim.Value;
        }

        private static int? GetCompanyId(this ClaimsIdentity identity)
        {
            return identity.GetAsInt2(KingClaimTypes.CompanyID);
        }

        private static int? GetDepartmentId(this ClaimsIdentity identity)
        {
            return identity.GetAsInt2(KingClaimTypes.Department_ID);
        }

        private static string GetDisplayName(this ClaimsIdentity identity)
        {
            return identity.GetAsString(KingClaimTypes.User_Name);
        }

        private static string GetLoginName(this ClaimsIdentity identity)
        {
            return identity.Name;
        }

        public static SysUser GetUserData(this ClaimsIdentity identity)
        {
            string asString = identity.GetAsString("http://schemas.microsoft.com/ws/2008/06/identity/claims/userdata");
            Trace.WriteLine("user data in identity:" + asString);
            if (!string.IsNullOrEmpty(asString))
            {
                return SysUser.FromJson(asString);
            }
            return null;
        }

        private static int GetUserId(this ClaimsIdentity identity)
        {
            int? nullable2 = identity.GetAsInt2(_factory.UserIdClaimType);
            return (nullable2.HasValue ? nullable2.GetValueOrDefault() : -1);
        }
    }
}

