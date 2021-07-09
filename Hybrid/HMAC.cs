using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Encryption
{
    public class HMAC
    {
        public static byte[] computeSha1(byte[] toBeHashed, byte[] key)
        {
            using (var sha1 = new HMACSHA1(key))
            {
                return sha1.ComputeHash(toBeHashed);
            }
        }
        public static byte[] computeSha256(byte[] toBeHashed, byte[] key)
        {
            using (var sha256 = new HMACSHA256(key))
            {
                return sha256.ComputeHash(toBeHashed);
            }
        }
        public static byte[] computeSha512(byte[] toBeHashed, byte[] key)
        {
            using (var sha512 = new HMACSHA512(key))
            {
                return sha512.ComputeHash(toBeHashed);
            }
        }
        public static byte[] computeMD5(byte[] toBeHashed, byte[] key)
        {
            using (var md5 = new HMACMD5(key))
            {
                return md5.ComputeHash(toBeHashed);
            }
        }
    }
}
