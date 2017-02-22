namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    [KingTable]
    public class SysPathRule
    {
        private readonly HashSet<int> _roles = new HashSet<int>();
        private readonly HashSet<int> _users = new HashSet<int>();
        private readonly HashSet<string> _usersByName = new HashSet<string>();
        private readonly HashSet<string> _verbs = new HashSet<string>();

        [KingColumn(MaxLength=0x200)]
        public string Path { get; set; }

        [KingColumn]
        public int PathId { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public int PathRuleId { get; set; }

        [KingColumn(MaxLength=200)]
        public string RequestVers
        {
            get
            {
                if ((this._verbs == null) || (this._verbs.Count == 0))
                {
                    return string.Empty;
                }
                return string.Join(",", this._verbs);
            }
            set
            {
                this._verbs.Clear();
                if (!string.IsNullOrEmpty(value))
                {
                    string[] strArray = value.Split(new char[] { ',' });
                    foreach (string str in strArray)
                    {
                        if (!string.IsNullOrWhiteSpace(str))
                        {
                            this._verbs.Add(str);
                        }
                    }
                }
            }
        }

        public HashSet<int> Roles
        {
            get
            {
                return this._roles;
            }
        }

        [KingColumn]
        public int RuleIndex { get; set; }

        [KingColumn]
        public int RuleType { get; set; }

        public HashSet<int> Users
        {
            get
            {
                return this._users;
            }
        }

        public HashSet<string> UsersByName
        {
            get
            {
                return this._usersByName;
            }
        }

        public HashSet<string> Verbs
        {
            get
            {
                return this._verbs;
            }
        }
    }
}

