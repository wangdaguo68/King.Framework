namespace King.Framework.Manager
{
    using King.Framework.Common;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Reflection;

    public class ViewFieldDict : Dictionary<string, ViewField>
    {
        private void ProcessFormatString(ViewField value)
        {
            using (BizDataContext context = new BizDataContext(true))
            {
                //ConfigurationManager instance = ConfigurationManager.GetInstance(context);
                switch (value.FormatType)
                {
                    case FormatType.Date:
                        //value.FormatString = instance.DateFormatString;
                        break;

                    case FormatType.DateTime:
                        //value.FormatString = instance.DateTimeFormatString;
                        break;

                    case FormatType.Time:
                        //value.FormatString = instance.TimeFormatString;
                        break;

                    case FormatType.Config:
                    {
                        string formatString = value.FormatString;
                        if (value.FormatString.IsNullOrEmpty())
                        {
                            throw new Exception(string.Format("自定义格式化 {0} 不存在或者不能为空", formatString));
                        }
                        value.FormatString = ConfigurationManager.AppSettings[value.FormatString];
                        if (value.FormatString.IsNullOrEmpty())
                        {
                            throw new Exception(string.Format("自定义格式化 {0} 不存在或者不能为空", formatString));
                        }
                        value.FormatString = value.FormatString;
                        break;
                    }
                    case FormatType.Money:
                       // value.FormatString = instance.MoneyFormatString;
                        break;
                }
                if (!value.FormatString.IsNullOrEmpty())
                {
                    value.FormatString = "{0:" + value.FormatString + "}";
                }
            }
        }

        private void tEST()
        {
            ViewFieldDict dict = new ViewFieldDict();
            ViewField field = new ViewField {
                Name = "xx"
            };
            dict["xxx"] = field;
            Console.WriteLine(dict.Count);
            Console.WriteLine(dict["xxx"].Name == null);
        }

        public ViewField this[string key]
        {
            get
            {
                if (base.ContainsKey(key))
                {
                    return base[key];
                }
                return new ViewField();
            }
            set
            {
                if (base.ContainsKey(key))
                {
                    base.Remove(key);
                }
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.ProcessFormatString(value);
                base.Add(key, value);
            }
        }
    }
}
