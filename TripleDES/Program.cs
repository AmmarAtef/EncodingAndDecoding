using System;
using System.Text;

namespace TripleDES
{
    class Program
    {
        static void Main(string[] args)
        {
            TripleDesEncryption tripleDes = new TripleDesEncryption();
            // three Keys 
            byte[] key = tripleDes.GenerateRandomNumber(24);
            var iv = tripleDes.GenerateRandomNumber(8);
            const string original = "Text to encrypt";

            var encrypted = tripleDes.Encrypt(Encoding.UTF8.GetBytes(original),
                key, iv);
            var decrypted = tripleDes.Decrypt(encrypted, key, iv);


            var decryptedMessage = Encoding.UTF8.GetString(decrypted);

            Console.WriteLine("Triple DES Encryption ");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("original Text = " + original);
            Console.WriteLine("Encrypted  Text = " + Convert.ToBase64String(encrypted));
            Console.WriteLine("Decrypted  Text = " + decryptedMessage);
            Console.WriteLine("");
            Console.ReadLine();
        }
    }
}
