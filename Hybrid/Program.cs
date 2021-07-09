using DataProcessor;
using RSA;
using System;
using System.Text;

namespace Hybrid
{
    class Program
    {
        static void Main(string[] args)
        {
            var rsaParams = new RSAWithRSAParameterKey();
            rsaParams.AssignNewKey();
            HypridEncryption hypridEncryption = new HypridEncryption();
            EncryptedPacket encryptedPacket = hypridEncryption.EncryptedData(rsaParams, @"E:\Project\psdata\in\file-sample_150kB.pdf",
           @"E:\Project\psdata\backup\file-sample_123.pdf");


            hypridEncryption.DecryptedData(encryptedPacket, rsaParams,
               @"E:\Project\psdata\backup\file-sample_123.pdf",
               @"E:\Project\psdata\backup\file-sample.pdf");
            //FileProcessor fileProcessor = new FileProcessor(args[1]);


            //    //"Text to encrypt";
            //    //  var rsaParams = new RSAWithRSAParameterKey();

            ////fileProcessor.WriteFile(args[1], Convert.ToBase64String(encryptedBlock.EncryptedData));
            //fileProcessor.MonitoringDirectory(@"E:\Project\psdata\in");
            //FileProcessor fileProcessor = new FileProcessor();
            //fileProcessor.MonitoringDirectory(@"E:\Project\psdata\in");
        }
    }
}
