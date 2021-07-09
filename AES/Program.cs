using System;
using System.Text;

namespace AES
{
    class Program
    {
        static void Main(string[] args)
        {
            AESEncryption aes = new AESEncryption();
            // three Keys 
            byte[] key = aes.GenerateRandomNumber(32);
            var iv = aes.GenerateRandomNumber(16);
            const string original = "Text to encrypt";

            var encrypted = aes.Encrypt(Encoding.UTF8.GetBytes(original),
                key, iv);
            var decrypted = aes.Decrypt(encrypted, key, iv);


            var decryptedMessage = Encoding.UTF8.GetString(decrypted);

            Console.WriteLine("AES DES Encryption ");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("original Text = " + original);
            Console.WriteLine("Encrypted  Text = " + Convert.ToBase64String(encrypted));
            Console.WriteLine("Decrypted  Text = " + decryptedMessage);
            Console.WriteLine("");
            Console.ReadLine();
        }
    }
}
