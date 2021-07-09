using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace RSA
{
    public class RSAWithCSPKey
    {
        const string ConatinerName = "ereer";
        private RSAParameters _publicKey;
        private RSAParameters _privateKey;
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
            var cspParams = new CspParameters { KeyContainerName = ConatinerName };
            var rsa = new RSACryptoServiceProvider(cspParams) { PersistKeyInCsp = false };
            rsa.Clear();
        }

        public byte[] EncryptData(string filePath, string backupFilePath)
        {
            byte[] cipherbytes;
            var cspParams = new CspParameters { KeyContainerName = ConatinerName };

            using (var rsa = new RSACryptoServiceProvider(2048, cspParams))
            {
                AesManaged aes = new AesManaged();
                byte[] keyEncrypted = rsa.Encrypt(aes.Key, false);
                byte[] LenK = new byte[4];
                byte[] LenIV = new byte[4];
                int lKey = keyEncrypted.Length;
                LenK = BitConverter.GetBytes(lKey);
                int lIV = aes.IV.Length;
                LenIV = BitConverter.GetBytes(lIV);
                ICryptoTransform encryptor = aes.CreateEncryptor();
                using (FileStream outFs = new FileStream(backupFilePath, FileMode.Create))
                {

                    outFs.Write(LenK, 0, 4);
                    outFs.Write(LenIV, 0, 4);
                    outFs.Write(keyEncrypted, 0, lKey);
                    outFs.Write(aes.IV, 0, lIV);

                    // Now write the cipher text using
                    // a CryptoStream for encrypting.
                    using (CryptoStream outStreamEncrypted = new CryptoStream(outFs, encryptor, CryptoStreamMode.Write))
                    {

                        // By encrypting a chunk at
                        // a time, you can save memory
                        // and accommodate large files.
                        int count = 0;
                        int offset = 0;

                        // blockSizeBytes can be any arbitrary size.
                        int blockSizeBytes = aes.BlockSize / 8;
                        byte[] data = new byte[blockSizeBytes];
                        int bytesRead = 0;

                        using (FileStream inFs = new FileStream(filePath, FileMode.Open))
                        {
                            do
                            {
                                count = inFs.Read(data, 0, blockSizeBytes);
                                offset += count;
                                outStreamEncrypted.Write(data, 0, count);
                                bytesRead += blockSizeBytes;
                            }
                            while (count > 0);
                            inFs.Close();
                        }
                        outStreamEncrypted.FlushFinalBlock();
                        outStreamEncrypted.Close();
                    }
                }
                return File.ReadAllBytes(backupFilePath);
            }
        }

        public string DecryptData(string filePath, string backupFilePath)
        {
            var cspParams = new CspParameters { KeyContainerName = ConatinerName };

            using (var rsa = new RSACryptoServiceProvider(2048, cspParams))
            {
                Aes aes = Aes.Create();

                // Create byte arrays to get the length of
                // the encrypted key and IV.
                // These values were stored as 4 bytes each
                // at the beginning of the encrypted package.
                byte[] LenK = new byte[4];
                byte[] LenIV = new byte[4];


                // Use FileStream objects to read the encrypted
                // file (inFs) and save the decrypted file (outFs).
                using (FileStream inFs = new FileStream(filePath, FileMode.Open))
                {

                    inFs.Seek(0, SeekOrigin.Begin);
                    inFs.Seek(0, SeekOrigin.Begin);
                    inFs.Read(LenK, 0, 3);
                    inFs.Seek(4, SeekOrigin.Begin);
                    inFs.Read(LenIV, 0, 3);

                    // Convert the lengths to integer values.
                    int lenK = BitConverter.ToInt32(LenK, 0);
                    int lenIV = BitConverter.ToInt32(LenIV, 0);

                    // Determine the start postition of
                    // the ciphter text (startC)
                    // and its length(lenC).
                    int startC = lenK + lenIV + 8;
                    int lenC = (int)inFs.Length - startC;

                    // Create the byte arrays for
                    // the encrypted Aes key,
                    // the IV, and the cipher text.
                    byte[] KeyEncrypted = new byte[lenK];
                    byte[] IV = new byte[lenIV];

                    // Extract the key and IV
                    // starting from index 8
                    // after the length values.
                    inFs.Seek(8, SeekOrigin.Begin);
                    inFs.Read(KeyEncrypted, 0, lenK);
                    inFs.Seek(8 + lenK, SeekOrigin.Begin);
                    inFs.Read(IV, 0, lenIV);
                    // Use RSACryptoServiceProvider
                    // to decrypt the AES key.
                    byte[] KeyDecrypted = rsa.Decrypt(KeyEncrypted, false);

                    // Decrypt the key.
                    ICryptoTransform transform = aes.CreateDecryptor(KeyDecrypted, IV);

                    // Decrypt the cipher text from
                    // from the FileSteam of the encrypted
                    // file (inFs) into the FileStream
                    // for the decrypted file (outFs).
                    using (FileStream outFs = new FileStream(backupFilePath, FileMode.Create))
                    {

                        int count = 0;
                        int offset = 0;

                        // blockSizeBytes can be any arbitrary size.
                        int blockSizeBytes = aes.BlockSize / 8;
                        byte[] data = new byte[blockSizeBytes];

                        // By decrypting a chunk a time,
                        // you can save memory and
                        // accommodate large files.

                        // Start at the beginning
                        // of the cipher text.
                        inFs.Seek(startC, SeekOrigin.Begin);
                        using (CryptoStream outStreamDecrypted = new CryptoStream(outFs, transform, CryptoStreamMode.Write))
                        {
                            do
                            {
                                count = inFs.Read(data, 0, blockSizeBytes);
                                offset += count;
                                outStreamDecrypted.Write(data, 0, count);
                            }
                            while (count > 0);

                            outStreamDecrypted.FlushFinalBlock();
                            outStreamDecrypted.Close();
                        }
                        outFs.Close();
                    }
                    inFs.Close();
                }
                return backupFilePath;
            }
            //byte[] plain;
            //var cspParams = new CspParameters { KeyContainerName = ConatinerName };


            //using (var rsa = new RSACryptoServiceProvider(2048, cspParams))
            //{
            //    //plain = rsa.Decrypt(dataToDecrypt, false);


            //    AesManaged aes = new AesManaged();
            //    /// aes.Padding = PaddingMode.None;
            //    ICryptoTransform decryptor = aes.CreateDecryptor();
            //    using (FileStream fsOutput = new FileStream(backupFilePath, FileMode.Create))
            //    {
            //        using (CryptoStream cs = new CryptoStream(fsOutput, decryptor, CryptoStreamMode.Write))
            //        {
            //            using (FileStream fsInput = new FileStream(filePath, FileMode.Open))
            //            {
            //                int data;
            //                while ((data = fsInput.ReadByte()) != -1)
            //                {
            //                    cs.WriteByte((byte)data);
            //                }
            //            }
            //        }
            //    }

            //    rsa.Clear();
            //}
            //return backupFilePath;
        }

    }
}
