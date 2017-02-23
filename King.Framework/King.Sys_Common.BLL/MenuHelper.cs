namespace King.Sys_Common.BLL
{
    using King.Sys_Common.Models;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public static class MenuHelper
    {
        public static MenuModel FindFuncFromTree(this List<MenuModel> treeList, long fid)
        {
            Action<MenuModel> action = null;
            if ((treeList == null) || (treeList.Count <= 0))
            {
                return null;
            }
            Stack<MenuModel> stack = new Stack<MenuModel>(treeList);
            while (stack.Count > 0)
            {
                MenuModel model2 = stack.Pop();
                if (model2.Function_ID == fid)
                {
                    return model2;
                }
                if (model2.Children.Count > 0)
                {
                    if (action == null)
                    {
                        action = delegate (MenuModel p) {
                            stack.Push(p);
                        };
                    }
                    model2.Children.ForEach(action);
                }
            }
            return null;
        }

        public static int GetMenuTreeLevel(this List<MenuModel> treeList)
        {
            List<MenuModel> list = new List<MenuModel>(treeList);
            int num = 0;
            while (list.Count > 0)
            {
                List<MenuModel> list2 = new List<MenuModel>();
                foreach (MenuModel model in list)
                {
                    list2.AddRange(model.Children);
                }
                num++;
                list = list2;
            }
            return num;
        }
    }
}

