namespace King.Framework.Manager
{
    using System;

    public class Message
    {
        private string body;
        private string signature;
        private string to;

        public Message()
        {
            this.to = string.Empty;
            this.body = string.Empty;
            this.signature = string.Empty;
        }

        public Message(string _to, string _body, string _signature)
        {
            this.to = string.Empty;
            this.body = string.Empty;
            this.signature = string.Empty;
            this.to = _to;
            this.body = _body;
            this.signature = _signature;
        }

        public string Body
        {
            get
            {
                return this.body;
            }
            set
            {
                this.body = value;
            }
        }

        public string[] MessageInfo
        {
            get
            {
                int num = 60;
                string str = " --";
                int num2 = string.IsNullOrEmpty(this.signature) ? 0 : (this.signature.Length + str.Length);
                int index = Convert.ToInt32(Math.Ceiling((double) (((double) (this.body.Length + num2)) / ((double) num))));
                if (index < 1)
                {
                    return new string[0];
                }
                string[] strArray = new string[index];
                string body = this.body;
                if (string.IsNullOrEmpty(this.signature))
                {
                    index--;
                    while (index != -1)
                    {
                        strArray[index] = body.Substring(num * index);
                        body = body.Substring(0, num * index);
                        index--;
                    }
                    return strArray;
                }
                index--;
                while (index != -1)
                {
                    if ((index + 1) == strArray.Length)
                    {
                        if ((((this.body.Length + num2) % num) < num2) & (((this.body.Length + num2) % num) != 0))
                        {
                            strArray[index] = str + this.signature;
                        }
                        else
                        {
                            strArray[index] = body.Substring(num * index) + str + this.signature;
                            body = body.Substring(0, num * index);
                        }
                    }
                    else
                    {
                        strArray[index] = body.Substring(num * index);
                        body = body.Substring(0, num * index);
                    }
                    index--;
                }
                return strArray;
            }
        }

        public string Signature
        {
            get
            {
                return this.signature;
            }
            set
            {
                this.signature = value;
            }
        }

        public string To
        {
            get
            {
                return this.to;
            }
            set
            {
                this.to = value;
            }
        }
    }
}
