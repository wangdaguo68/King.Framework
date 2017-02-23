namespace King.Sys_Common.Models
{
    using King.Framework.EntityLibrary;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class MenuModel : IComparable<MenuModel>
    {
        private List<MenuModel> _children;
        private bool _isExpanded;
        private bool _isSelected;

        public MenuModel()
        {
            this._children = new List<MenuModel>();
            this._isExpanded = false;
            this._isSelected = false;
        }

        public MenuModel(SysFunction func, string ssotoken)
        {
            this._children = new List<MenuModel>();
            this._isExpanded = false;
            this._isSelected = false;
            this.Function_ID = func.Function_ID;
            this.Is_Menu = func.Is_Menu;
            this.OrderIndex = func.OrderIndex;
            this.PageId = func.PageId;
            this.Permission_Name = func.Permission_Name;
            this.Permission_Type = func.Permission_Type;
            this.URL = func.URL;
            this.AppURL = func.AppURL;
            this.Func = func;
            this.Ssotoken = ssotoken;
        }

        public int CompareTo(MenuModel other)
        {
            int? orderIndex = this.OrderIndex;
            orderIndex = other.OrderIndex;
            return ((orderIndex.HasValue ? orderIndex.GetValueOrDefault() : 0) - (orderIndex.HasValue ? orderIndex.GetValueOrDefault() : 0));
        }

        public string GetWholeAppURL()
        {
            if (string.IsNullOrEmpty(this.AppURL))
            {
                return "";
            }
            if (this.AppURL.Contains("?"))
            {
                return (this.AppURL + "&sso_token=" + this.Ssotoken);
            }
            return (this.AppURL + "?sso_token=" + this.Ssotoken);
        }

        public string GetWholeURL()
        {
            if (string.IsNullOrEmpty(this.URL))
            {
                return "";
            }
            if (this.URL.Contains("?"))
            {
                return (this.URL + "&fid=" + this.Function_ID.ToString());
            }
            return (this.URL + "?fid=" + this.Function_ID.ToString());
        }

        public virtual string AppURL { get; set; }

        public List<MenuModel> Children
        {
            get
            {
                return this._children;
            }
        }

        public SysFunction Func { get; set; }

        public long Function_ID { get; set; }

        public int? Is_Menu { get; set; }

        public bool IsExpanded
        {
            get
            {
                return this._isExpanded;
            }
            set
            {
                this._isExpanded = value;
            }
        }

        public bool IsSelected
        {
            get
            {
                return this._isSelected;
            }
            set
            {
                this._isSelected = value;
            }
        }

        public int? OrderIndex { get; set; }

        public long? PageId { get; set; }

        public MenuModel Parent { get; set; }

        public string Permission_Name { get; set; }

        public long? Permission_Type { get; set; }

        public string Ssotoken { get; set; }

        public virtual string URL { get; set; }
    }
}

