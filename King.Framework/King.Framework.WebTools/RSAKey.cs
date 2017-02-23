<<<<<<< HEAD
﻿namespace King.Framework.WebTools
=======
﻿namespace Drision.Framework.WebTools
>>>>>>> 2b573943baeab4164300c7a14b5c182ddccdbe59
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

