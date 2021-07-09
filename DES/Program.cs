using System;
using System.Text;

namespace DES
{
    class Program
    {
        static void Main(string[] args)
        {
            DesEncryption des = new DesEncryption();
            byte[] key = des.GenerateRandomNumber(8);
            var iv = des.GenerateRandomNumber(8);
            const string original = "Text to encrypt";

            var encrypted = des.Encrypt(Encoding.UTF8.GetBytes(original),
                key, iv);
            var decrypted = des.Decrypt(encrypted, key, iv);


            var decryptedMessage = Encoding.UTF8.GetString(decrypted);

            Console.WriteLine("DES Encryption ");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("original Text = "+ original);
            Console.WriteLine("Encrypted  Text = " + Convert.ToBase64String(encrypted));
            Console.WriteLine("Decrypted  Text = " + decryptedMessage);
            Console.WriteLine("");
            Console.ReadLine();

        }
    }
}
