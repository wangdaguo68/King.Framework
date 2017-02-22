namespace King.Framework.Manager
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Web;

    public class MessageSender : IMessageSender
    {
        private string account;
        private string companycode;
        private string password;

        public MessageSender()
        {
        }

        public MessageSender(string _companycode, string _account, string _passwod)
        {
            this.companycode = _companycode;
            this.account = _account;
            this.password = _passwod;
        }

        protected string HttpPostData(string url, string data)
        {
            string str = string.Empty;
            try
            {
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = Encoding.UTF8.GetByteCount(data);
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(data);
                    writer.Flush();
                    writer.Close();
                }
                using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream()))
                {
                    str = reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch
            {
            }
            return str;
        }

        public void Send(Message msg)
        {
            foreach (string str in msg.MessageInfo)
            {
                this.ToMobile(msg.To, str);
            }
        }

        public void Send(IList<Message> msgList)
        {
            foreach (Message message in msgList)
            {
                this.Send(message);
            }
        }

        internal bool ToMobile(string mobile, string msgInfo)
        {
            string data = string.Format("enterpriseid={0}&accountid={1}&pswd={2}&mobs={3}&msg={4}", new object[] { this.companycode, this.account, this.password, mobile, HttpUtility.UrlEncode(msgInfo) });
            string url = "http://211.136.163.68:8000/httpserver";
            return (this.HttpPostData(url, data) == "100");
        }

        public bool TrySend(string to)
        {
            return this.ToMobile(to, "Message Test Success!");
        }

        public string Account
        {
            get
            {
                return this.account;
            }
            set
            {
                this.account = value;
            }
        }

        public string CompanyCode
        {
            get
            {
                return this.companycode;
            }
            set
            {
                this.companycode = value;
            }
        }

        public string Password
        {
            get
            {
                return this.password;
            }
            set
            {
                this.password = value;
            }
        }
    }
}
