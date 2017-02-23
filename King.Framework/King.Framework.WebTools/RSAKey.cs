namespace King.Framework.WebTools
{
    using System;
    using System.Runtime.CompilerServices;

    public class RSAKey
    {
        internal RSAKey(string privateKey, string publicKey)
        {
            this.PrivateKeyXml = privateKey;
            this.PublicKeyXml = publicKey;
        }

        public string PrivateKeyXml { get; private set; }

        public string PublicKeyXml { get; private set; }
    }
}

