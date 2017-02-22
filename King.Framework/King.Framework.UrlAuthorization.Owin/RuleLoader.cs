namespace King.Framework.UrlAuthorization.Owin
{
    using King.Framework.Common;
    using King.Framework.EntityLibrary;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    internal static class RuleLoader
    {
        private static CacheItem<SysPathRule[]> cacheRules;
        private static readonly object locker = new object();

        static RuleLoader()
        {
            SysPathRule[] ruleArray = InnerGetRules();
            DateTime expireTime = DateTime.Now.AddMinutes(10.0);
            cacheRules = new CacheItem<SysPathRule[]>(ruleArray, expireTime);
        }

        public static SysPathRule[] GetRules()
        {
            CacheItem<SysPathRule[]> cacheRules = RuleLoader.cacheRules;
            DateTime now = DateTime.Now;
            if ((now > cacheRules.ExpireTime) && Monitor.TryEnter(locker))
            {
                try
                {
                    SysPathRule[] ruleArray = InnerGetRules();
                    DateTime expireTime = now.AddMinutes(10.0);
                    CacheItem<SysPathRule[]> item2 = new CacheItem<SysPathRule[]>(ruleArray, expireTime);
                    Interlocked.Exchange<CacheItem<SysPathRule[]>>(ref RuleLoader.cacheRules, item2);
                }
                finally
                {
                    Monitor.Exit(locker);
                }
            }
            return RuleLoader.cacheRules.CacheObject;
        }

        private static SysPathRule[] InnerGetRules()
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

