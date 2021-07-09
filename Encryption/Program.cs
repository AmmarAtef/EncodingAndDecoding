using System;
using System.Text;

namespace Encryption
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Hello World!");
            Console.WriteLine("------------------------");
            Console.WriteLine();
            for (var i = 0; i < 10; i++)
            {
                Console.WriteLine("Random Number" + i + " : "
                    + Convert.ToBase64String(Random.GenerateRandomNumber(32)));
            }
            var key =Random.GenerateRandomNumber(32);

            const string originalMessage = "Original Message to hash";
            const string originalMessage2 = "this is another message to hash";

            var md5hashedMessage = HashData.computeMD5(Encoding.UTF8.GetBytes(originalMessage));
            var md5hashedMessage2 = HashData.computeMD5(Encoding.UTF8.GetBytes(originalMessage2));

            var sha1hashedMessage = HashData.computeSha1(Encoding.UTF8.GetBytes(originalMessage));
            var sha1hashedMessage2 = HashData.computeSha1(Encoding.UTF8.GetBytes(originalMessage2));

            var sha256hashedMessage = HashData.computeSha256(Encoding.UTF8.GetBytes(originalMessage));
            var sha256hashedMessage2 = HashData.computeSha256(Encoding.UTF8.GetBytes(originalMessage2));

            var sha512hashedMessage = HashData.computeSha512(Encoding.UTF8.GetBytes(originalMessage));
            var sha512hashedMessage2 = HashData.computeSha512(Encoding.UTF8.GetBytes(originalMessage2));


            Console.WriteLine();
            Console.WriteLine("MD5 Hashes");
            Console.WriteLine();
            Console.WriteLine("Message 1 Hash = "+ Convert.ToBase64String(md5hashedMessage));
            Console.WriteLine("Message 2 Hash = " + Convert.ToBase64String(md5hashedMessage2));
           
            Console.WriteLine();
            Console.WriteLine("Sha1 Hashes");
            Console.WriteLine();
            Console.WriteLine("Message 1 Hash = " + Convert.ToBase64String(sha1hashedMessage));
            Console.WriteLine("Message 2 Hash = " + Convert.ToBase64String(sha1hashedMessage2));


            Console.WriteLine();
            Console.WriteLine("Hmac Sha256 Hashes");
            Console.WriteLine();
            Console.WriteLine("Hmac Message 1 Hash = " + Convert.ToBase64String(sha256hashedMessage));
            Console.WriteLine("Hmac Message 2 Hash = " + Convert.ToBase64String(sha256hashedMessage2));


            Console.WriteLine();
            Console.WriteLine("Hmac Sha512 Hashes");
            Console.WriteLine();
            Console.WriteLine("Message 1 Hash = " + Convert.ToBase64String(sha512hashedMessage));
            Console.WriteLine("Message 2 Hash = " + Convert.ToBase64String(sha512hashedMessage2));



            var hmacMd5hashedMessage = HMAC.computeMD5(Encoding.UTF8.GetBytes(originalMessage), key);
            var hmacMd5hashedMessage2 = HMAC.computeMD5(Encoding.UTF8.GetBytes(originalMessage2),key);
                
            var hmacSha1hashedMessage = HMAC.computeSha1(Encoding.UTF8.GetBytes(originalMessage), key);
            var hmacSha1hashedMessage2 = HMAC.computeSha1(Encoding.UTF8.GetBytes(originalMessage2), key);
                
            var hmacSha256hashedMessage = HMAC.computeSha256(Encoding.UTF8.GetBytes(originalMessage), key);
            var hmacSha256hashedMessage2 = HMAC.computeSha256(Encoding.UTF8.GetBytes(originalMessage2), key);
                
            var hmacSha512hashedMessage = HMAC.computeSha512(Encoding.UTF8.GetBytes(originalMessage), key);
            var hmacSha512hashedMessage2 = HMAC.computeSha512(Encoding.UTF8.GetBytes(originalMessage2), key);


            Console.WriteLine();
            Console.WriteLine("Hmac MD5 Hashes");
            Console.WriteLine();
            Console.WriteLine("Hmac Message 1 Hash = " + Convert.ToBase64String(hmacMd5hashedMessage));
            Console.WriteLine("Hmac Message 2 Hash = " + Convert.ToBase64String(hmacMd5hashedMessage2));

            Console.WriteLine();
            Console.WriteLine("Hmac Sha1 Hashes");
            Console.WriteLine();
            Console.WriteLine("Hmac Message 1 Hash = " + Convert.ToBase64String(hmacSha1hashedMessage));
            Console.WriteLine("Hmac Message 2 Hash = " + Convert.ToBase64String(hmacSha1hashedMessage2));


            Console.WriteLine();
            Console.WriteLine("Hmac Sha256 Hashes");
            Console.WriteLine();
            Console.WriteLine("Hmac Message 1 Hash = " + Convert.ToBase64String(hmacSha256hashedMessage));
            Console.WriteLine("Hmac Message 2 Hash = " + Convert.ToBase64String(hmacSha256hashedMessage2));


            Console.WriteLine();
            Console.WriteLine("Hmac Sha512 Hashes");
            Console.WriteLine();
            Console.WriteLine("Hmac Message 1 Hash = " + Convert.ToBase64String(hmacSha512hashedMessage));
            Console.WriteLine("Hmac Message 2 Hash = " + Convert.ToBase64String(hmacSha512hashedMessage2));



            Console.ReadLine();
        }
    }
}
