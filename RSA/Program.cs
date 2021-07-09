using DataProcessor;
using System;
using System.IO;
using System.Text;

namespace RSA
{
    class Program
    {
        static void Main(string[] args)
        {
            //RSAWithRSAParameterKey();
            // RsaWithXml();
            RSAWithCSP();
            Console.ReadLine();
        }

        

        private static void RsaWithXml()
        {
            var rsa = new RsaWithXmlKey();
            const string original = "Text to encrypt";
            const string publicKeyPath = "c:\\temp\\publickey.xml";
            const string privateKeyPath = "c:\\temp\\privatekey.xml";
            rsa.AssignNewKey(publicKeyPath, privateKeyPath);
            var encrypted = rsa.EncryptData(publicKeyPath, Encoding.UTF8.GetBytes(original));
            var decrypted = rsa.DecryptData(privateKeyPath, encrypted);
            Console.WriteLine("Xml Based Key");
            Console.WriteLine();
            Console.WriteLine(" Original Text = " + original);
            Console.WriteLine();
            Console.WriteLine(" Encrypted Text = " + Convert.ToBase64String(encrypted));
            Console.WriteLine();
            Console.WriteLine(" Decrypted Text = " + Encoding.Default.GetString(decrypted));
            Console.WriteLine();
        }


        private static void RSAWithCSP()
        {
            //new FileProcessor().MonitoringDirectory(@"E:\Project\psdata\SourceFolder");
            FileProcessor fileProcessor = new FileProcessor();
            fileProcessor.MonitoringEncryptedDirectory(@"E:\Project\psdata\Encrypted");
            Console.WriteLine();

            //RSAWithCSPKey rSAWithCSPKey = new RSAWithCSPKey();
            //rSAWithCSPKey.DecryptData(
            //    @"E:\Project\psdata\Encrypted\Actual-1d7d9b1f-a180-49ab-9b78-ee2a1702e66a.xlsx",
            //    @"E:\Project\psdata\Decrypted\Actual-1.xlsx");
            Console.ReadLine();
        }
    }
}
