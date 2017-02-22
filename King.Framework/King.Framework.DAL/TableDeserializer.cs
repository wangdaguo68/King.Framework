namespace King.Framework.DAL
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class TableDeserializer
    {
        private readonly string content;
        private string current;
        private int length;
        private int pos = 0;
        private List<string> row = new List<string>(100);
        private StringBuilder sb = new StringBuilder(0x400);

        public TableDeserializer(string _content)
        {
            this.content = _content;
            this.length = (_content == null) ? 0 : _content.Length;
        }

        public Dictionary<string, TableData> Deserializer()
        {
            Dictionary<string, TableData> dictionary = new Dictionary<string, TableData>();
            while (this.IsNotEnd)
            {
                this.Fetch();
                string current = this.current;
                this.Fetch();
                int rowCount = Convert.ToInt32(this.current);
                int count = this.row.Count;
                TableData data = new TableData(this.row.ToArray(), rowCount);
                for (int i = 0; i < rowCount; i++)
                {
                    this.FetchRow();
                    data.Append(this.row.ToArray());
                }
                dictionary.Add(current, data);
            }
            this.pos = 0;
            return dictionary;
        }

        private bool Fetch()
        {
            this.sb.Clear();
            while (this.pos < this.length)
            {
                char ch = this.content[this.pos];
                switch (ch)
                {
                    case ',':
                        this.pos++;
                        this.current = this.sb.ToString();
                        return false;

                    case '\n':
                        this.pos++;
                        this.current = this.sb.ToString();
                        return true;

                    case '\\':
                        this.pos++;
                        if (this.content[this.pos] == ',')
                        {
                            this.sb.Append(',');
                        }
                        else if (this.content[this.pos] == 'n')
                        {
                            this.sb.Append('\n');
                        }
                        else
                        {
                            if (this.content[this.pos] != '\\')
                            {
                                throw new ApplicationException("不支持的转义字符");
                            }
                            this.sb.Append('\\');
                        }
                        this.pos++;
                        break;

                    default:
                        this.sb.Append(ch);
                        this.pos++;
                        break;
                }
            }
            return true;
        }

        private bool FetchRow()
        {
            this.row.Clear();
            if (!this.IsNotEnd)
            {
                return false;
            }
            while (true)
            {
                bool flag = this.Fetch();
                this.row.Add(this.current);
                if (flag)
                {
                    return true;
                }
            }
        }

        public string[] GetRow()
        {
            this.FetchRow();
            return this.row.ToArray();
        }

        public string GetString()
        {
            this.Fetch();
            return this.current;
        }

        private bool IsNotEnd
        {
            get
            {
                return (this.pos < this.length);
            }
        }
    }
}
