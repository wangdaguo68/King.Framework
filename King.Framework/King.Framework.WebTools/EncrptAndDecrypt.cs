
﻿namespace King.Framework.WebTools

{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;

    public static class EncrptAndDecrypt
    {
        public static string Decrypt_DES(string strText, string sKey)
        {
            string str;
            if (string.IsNullOrEmpty(sKey))
            {
                throw new ArgumentNullException("skey");
            }
            sKey = (sKey + "0000000000").Substring(0, 8);
            try
            {
                DESCryptoServiceProvider provider = null;
                byte[] buffer = Convert.FromBase64String(HttpUtility.UrlDecode(strText));
                provider = new DESCryptoServiceProvider {
                    Key = Encoding.ASCII.GetBytes(sKey),
                    IV = provider.Key
                };
                using (MemoryStream stream = new MemoryStream())
                {
                    using (CryptoStream stream2 = new CryptoStream(stream, provider.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        stream2.Write(buffer, 0, buffer.Length);
                        stream2.FlushFinalBlock();
                        stream2.Close();
                    }
                    stream.Close();
                    str = Encoding.Default.GetString(stream.ToArray());
                }
            }
            catch
            {
                str = strText;
            }
            return str;
        }

        public static string Decrypt_RSA(string strText, string privateKey)
        {
            try
            {
                RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
                byte[] rgb = Convert.FromBase64String(HttpUtility.UrlDecode(strText));
                provider.FromXmlString(privateKey);
                byte[] bytes = provider.Decrypt(rgb, false);
                return Encoding.Default.GetString(bytes);
            }
            catch
            {
                return strText;
            }
        }

        public static string Encrypt_DES(string strText, string sKey)
        {
            if (string.IsNullOrEmpty(sKey))
            {
                throw new ArgumentNullException("skey");
            }
            sKey = (sKey + "0000000000").Substring(0, 8);
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            byte[] bytes = Encoding.Default.GetBytes(strText);
            provider.Key = Encoding.ASCII.GetBytes(sKey);
            provider.IV = provider.Key;
            using (MemoryStream stream = new MemoryStream())
            {
                using (CryptoStream stream2 = new CryptoStream(stream, provider.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    stream2.Write(bytes, 0, bytes.Length);
                    stream2.FlushFinalBlock();
                    stream2.Close();
                }
                string str = Convert.ToBase64String(stream.ToArray());
                stream.Close();
                return HttpUtility.UrlEncode(str);
            }
        }

        public static string Encrypt_MD5(string strText)
        {
            using (MD5 md = MD5.Create())
            {
                byte[] buffer = md.ComputeHash(Encoding.UTF8.GetBytes(strText));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < buffer.Length; i++)
                {
                    builder.Append(buffer[i].ToString("X2"));
                }
                return builder.ToString();
            }
        }

        public static string Encrypt_RSA(string strText, string publicKey)
        {
            byte[] bytes = Encoding.Default.GetBytes(strText);
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.FromXmlString(publicKey);
            return HttpUtility.UrlEncode(Convert.ToBase64String(provider.Encrypt(bytes, false)));
        }

        public static RSAKey GetRSA_Key()
        {
            int dwKeySize = 0x400;
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider(dwKeySize);
            string privateKey = provider.ToXmlString(true);
            return new RSAKey(privateKey, provider.ToXmlString(false));
        }
    }
}

