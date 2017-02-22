namespace King.Framework.Common
{
    using System;
    using System.Globalization;
    using System.Text;

    public class StringHelper
    {
        public static string ASCIIBytesToString(byte[] input)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            return encoding.GetString(input);
        }

        public static string BytesToDecString(byte[] input)
        {
            StringBuilder builder = new StringBuilder(0x40);
            for (int i = 0; i < input.Length; i++)
            {
                builder.Append(string.Format((i == 0) ? "{0:D3}" : "-{0:D3}", input[i]));
            }
            return builder.ToString();
        }

        public static string BytesToHexString(byte[] input)
        {
            StringBuilder builder = new StringBuilder(input.Length * 2);
            for (int i = 0; i < input.Length; i++)
            {
                builder.Append(string.Format("{0:X2}", input[i]));
            }
            return builder.ToString();
        }

        public static byte[] FromBase64(string base64)
        {
            return Convert.FromBase64String(base64);
        }

        public static byte[] HexStringToBytes(string hex)
        {
            if (hex.Length == 0)
            {
                return new byte[1];
            }
            if ((hex.Length % 2) == 1)
            {
                hex = "0" + hex;
            }
            int num = hex.Length >> 1;
            byte[] buffer = new byte[num];
            for (int i = 0; i < num; i++)
            {
                buffer[i] = byte.Parse(hex.Substring(i + i, 2), NumberStyles.AllowHexSpecifier);
            }
            return buffer;
        }

        public static string ToBase64(byte[] input)
        {
            return Convert.ToBase64String(input);
        }

        public static string UTF16BytesToString(byte[] input)
        {
            UnicodeEncoding encoding = new UnicodeEncoding();
            return encoding.GetString(input);
        }

        public static string UTF8BytesToString(byte[] input)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            return encoding.GetString(input);
        }
    }
}
