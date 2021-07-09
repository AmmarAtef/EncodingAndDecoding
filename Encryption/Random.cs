using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Encryption
{
    public class Random
    {
        public static byte[] GenerateRandomNumber(int length)
        {
            //  to avoid duplicate the Name of the file 
            using(var randomNumberGenerator = new RNGCryptoServiceProvider())
            {
                var randomNumber = new byte[length];
                randomNumberGenerator.GetBytes(randomNumber);
                return randomNumber;
            }
        }
    }
}
