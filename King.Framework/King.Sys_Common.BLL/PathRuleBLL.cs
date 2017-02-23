namespace King.Sys_Common.BLL
{
    using King.Framework.Common;
    using King.Framework.EntityLibrary;
    using King.Framework.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PathRuleBLL : BaseBLL
    {
        public PathRuleBLL()
        {
        }

        public PathRuleBLL(IUserIdentity user) : base(user)
        {
        }

        public SysPathRule GetRule(int id)
        {
            using (BizDataContext context = new BizDataContext(true))
            {
                SysPathRule rule = context.FindById<SysPathRule>(new object[] { id });
                List<SysPathRuleItem> list = context.Where<SysPathRuleItem>(p => p.PathRuleId == id);
                foreach (SysPathRuleItem item in list)
                {
                    if (item.ItemType == PathRuleMasterType.Role)
                    {
                        rule.Roles.Add(item.ObjectId);
                    }
                    else
                    {
                        rule.Users.Add(item.ObjectId);
                    }
                }
                return rule;
            }
        }

        public SysPathRule[] GetRules()
        {
            List<SysPathRule> list;
            List<SysPathRuleItem> list2;
            Dictionary<int, SysPath> dictionary;
            SysPathRule current;
            using (BizDataContext context = new BizDataContext(true))
            {
                dictionary = context.FetchAll<SysPath>().ToDictionary<SysPath, int>(t => t.PathId);
                list = context.FetchAll<SysPathRule>();
                list2 = context.FetchAll<SysPathRuleItem>();
            }
            using (List<SysPathRule>.Enumerator enumerator = list.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    current = enumerator.Current;
                    current.Path = dictionary[current.PathId].Path.ToLower();
                }
            }
            Dictionary<int, SysPathRule> dictionary2 = list.ToDictionary<SysPathRule, int, SysPathRule>(p => p.PathRuleId, p => p);
            Dictionary<int, IGrouping<int, SysPathRuleItem>> dictionary3 = (from p in list2 group p by p.PathRuleId).ToDictionary<IGrouping<int, SysPathRuleItem>, int, IGrouping<int, SysPathRuleItem>>(p => p.Key, p => p);
            foreach (KeyValuePair<int, IGrouping<int, SysPathRuleItem>> pair in dictionary3)
            {
                current = dictionary2[pair.Key];
                foreach (SysPathRuleItem item in pair.Value)
                {
                    if (item.ItemType == PathRuleMasterType.Role)
                    {
                        current.Roles.Add(item.ObjectId);
                    }
                    else
                    {
                        current.Users.Add(item.ObjectId);
                    }
                }
            }
            dictionary2.Clear();
            dictionary2 = null;
            dictionary3.Clear();
            dictionary3 = null;
            dictionary.Clear();
            dictionary = null;
            list2.Clear();
            list2 = null;
            return (from p in list
                orderby p.RuleIndex
                select p).ToArray<SysPathRule>();
        }
    }
}

