using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace RSA
{
   public class RSAWithCSPKey
    {
        const string ConatinerName = "MyContainer1";

        public void AssignNewKey()
        {
            CspParameters cspParams = new CspParameters(1);
            cspParams.KeyContainerName = ConatinerName;
            cspParams.Flags = CspProviderFlags.UseMachineKeyStore;
            cspParams.ProviderName = "Microsoft Strong Cryptographic Provider";

            var rsa = new RSACryptoServiceProvider(cspParams)
            {
                PersistKeyInCsp = true
            };

        }

        public void DeleteKeyInCsp()
        {
            var cspParams = new CspParameters { KeyContainerName = ConatinerName};
            var rsa = new RSACryptoServiceProvider(cspParams) { PersistKeyInCsp = false };
            rsa.Clear();
        }

        public byte[] EncryptData(byte[] dataToEncrypt)
        {
            byte[] cipherbytes;
            var cspParams = new CspParameters { KeyContainerName = ConatinerName };

            using(var rsa = new RSACryptoServiceProvider(2048, cspParams))
            {
                cipherbytes = rsa.Encrypt(dataToEncrypt, false);

            }
            return cipherbytes;
        }

        public byte[] DecryptData(byte[] dataToDecrypt)
        {
            byte[] plain;
            var cspParams = new CspParameters { KeyContainerName = ConatinerName };

            using (var rsa = new RSACryptoServiceProvider(2048, cspParams))
            {
                plain = rsa.Decrypt(dataToDecrypt, false);

            }
            return plain;
        }

    }
}
