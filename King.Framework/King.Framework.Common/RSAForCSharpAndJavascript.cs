namespace King.Framework.Common
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public class RSAForCSharpAndJavascript : IDisposable
    {
        private readonly RSACryptoServiceProvider _sp;

        public RSAForCSharpAndJavascript()
        {
            int dwKeySize = 0x400;
            this._sp = new RSACryptoServiceProvider(dwKeySize);
        }

        public RSAForCSharpAndJavascript(string privateKey)
        {
            if (!string.IsNullOrEmpty(privateKey))
            {
                CspParameters parameters = new CspParameters {
                    Flags = CspProviderFlags.UseMachineKeyStore
                };
                this._sp = new RSACryptoServiceProvider(parameters);
                this._sp.FromXmlString(privateKey);
            }
            else
            {
                int dwKeySize = 0x400;
                this._sp = new RSACryptoServiceProvider(dwKeySize);
            }
        }

        public byte[] Decrypt(byte[] txt)
        {
            return this._sp.Decrypt(txt, false);
        }

        public void Dispose()
        {
            this._sp.Dispose();
        }

        public byte[] Encrypt(string txt)
        {
            byte[] bytes = new ASCIIEncoding().GetBytes(txt);
            return this._sp.Encrypt(bytes, false);
        }

        public RSAParameters ExportParameters(bool includePrivateParameters)
        {
            return this._sp.ExportParameters(includePrivateParameters);
        }

        public string GetPrivateKey()
        {
            return this._sp.ToXmlString(true);
        }

        public string GetPublicKey()
        {
            return this._sp.ToXmlString(false);
        }

        public string GetRSA_E()
        {
            return StringHelper.BytesToHexString(this.ExportParameters(false).Exponent);
        }

        public string GetRSA_M()
        {
            return StringHelper.BytesToHexString(this.ExportParameters(false).Modulus);
        }

        public string[] GetRSAKey()
        {
            string str = this._sp.ToXmlString(true);
            string str2 = this._sp.ToXmlString(false);
            return new string[] { str2, str };
        }
    }
}
