using AES;
using DataProcessor;
using RSA;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using static System.Console;

namespace Hybrid
{
    public class HypridEncryption
    {
        private readonly AESEncryption _aes = new AESEncryption();
        
        public EncryptedPacket EncryptedData(string original,
            RSAWithRSAParameterKey rsaParams)
        {
            var sessionKey = _aes.GenerateRandomNumber(32);
            var encryptedPacket = new EncryptedPacket { Iv = _aes.GenerateRandomNumber(16) };

            encryptedPacket.EncryptedData = _aes.Encrypt(Encoding.UTF8.GetBytes(original), sessionKey, encryptedPacket.Iv);

            encryptedPacket.EncryptedSessionKey = rsaParams.EncryptData(sessionKey);


            using (var hmac = new HMACSHA256(sessionKey))
            {
                encryptedPacket.Hmac = hmac.ComputeHash(encryptedPacket.EncryptedData);

            }




            return encryptedPacket;
        }

        public EncryptedPacket EncryptedData(RSAWithRSAParameterKey rsaParams, string filePath,
            string backupFilePath)
        {

            var sessionKey = _aes.GenerateRandomNumber(32);
            var encryptedPacket = new EncryptedPacket { Iv = _aes.GenerateRandomNumber(16) };

           // encryptedPacket.EncryptedData = _aes.Encrypt(Encoding.UTF8.GetBytes(original), sessionKey, encryptedPacket.Iv);

            encryptedPacket.EncryptedSessionKey = rsaParams.EncryptData(sessionKey);


            //using (var hmac = new HMACSHA256(sessionKey))
            //{
            //    encryptedPacket.Hmac = hmac.ComputeHash(encryptedPacket.EncryptedData);

            //}

            AesManaged aes = new AesManaged();
            ICryptoTransform encryptor = aes.CreateEncryptor(sessionKey, encryptedPacket.Iv);

            
            using (FileStream fsOutput = new FileStream(backupFilePath, FileMode.Create))
            {
                using (CryptoStream cs = new CryptoStream(fsOutput, encryptor, CryptoStreamMode.Write))
                {
                    using (FileStream fsInput = new FileStream(filePath, FileMode.Open))
                    {
                        int data;
                        while ((data = fsInput.ReadByte()) != -1)
                        {
                            cs.WriteByte((byte)data);
                        }
                    }
                }
            }
            encryptedPacket.EncryptedData = File.ReadAllBytes(backupFilePath);

            return encryptedPacket;
        }


        public EncryptedPacket  DecryptedData(EncryptedPacket encryptedPacket,
            RSAWithRSAParameterKey rsaParams, string filePath, string backupFilePath)
        {

            var decryptedSessionKey = rsaParams.DecryptData(encryptedPacket.EncryptedSessionKey);

            //using (var hmac = new HMACSHA256(decryptedSessionKey))
            //{
            //    var hmacToCheck = hmac.ComputeHash(encryptedPacket.EncryptedData);
            //    if (!Compare(encryptedPacket.Hmac, hmacToCheck))
            //    {
            //        throw new CryptographicException("Hmac for decryption does not match encrypted packet Data");
            //    }
            //}

            //var decryptedData = _aes.Decrypt(encryptedPacket.EncryptedData,
            //    decryptedSessionKey, encryptedPacket.Iv);




            AesManaged aes = new AesManaged();
            ICryptoTransform decryptor = aes.CreateDecryptor(rsaParams.DecryptData(encryptedPacket.EncryptedSessionKey), encryptedPacket.Iv);
            using (FileStream fsOutput = new FileStream(backupFilePath, FileMode.Create))
            {
                using (CryptoStream cs = new CryptoStream(fsOutput, decryptor, CryptoStreamMode.Write))
                {
                    using (FileStream fsInput = new FileStream(filePath, FileMode.Open))
                    {
                        int data;
                        while ((data = fsInput.ReadByte()) != -1)
                        {
                            cs.WriteByte((byte)data);
                        }
                    }
                }
            }


            return encryptedPacket;
        }

        public byte[] DecryptData(EncryptedPacket encryptedPacket,
            RSAWithRSAParameterKey rsaParams)
        {
            var decryptedSessionKey = rsaParams.DecryptData(encryptedPacket.EncryptedSessionKey);

            using (var hmac = new HMACSHA256(decryptedSessionKey))
            {
                var hmacToCheck = hmac.ComputeHash(encryptedPacket.EncryptedData);
                if (!Compare(encryptedPacket.Hmac, hmacToCheck))
                {
                    throw new CryptographicException("Hmac for decryption does not match encrypted packet Data");
                }
            }

            var decryptedData = _aes.Decrypt(encryptedPacket.EncryptedData,
                decryptedSessionKey, encryptedPacket.Iv);

            return decryptedData;
        }

        private static bool Compare(byte[] array1, byte[] array2)
        {
            var result = array1.Length == array2.Length;
            for (int i = 0; i < array1.Length && i < array2.Length && result; ++i)
            {
                result &= array1[i] == array2[i];

            }
            return result;
        }


        
    }
}
