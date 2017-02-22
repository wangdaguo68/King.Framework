namespace King.Framework.Linq
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;

    internal class TraceWritter : TextWriter
    {
        public override void Write(char c)
        {
            Trace.Write(c);
        }

        public override void Write(string value)
        {
            Trace.Write(value);
        }

        public override System.Text.Encoding Encoding
        {
            get
            {
                return System.Text.Encoding.UTF8;
            }
        }
    }
}
