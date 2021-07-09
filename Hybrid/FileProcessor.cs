using static System.Console;

using System.IO;
using System.Text;
using System;
using System.Runtime.Caching;
using RSA;
using Hybrid;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using iTextSharp.text;
using System.Security.Cryptography;

namespace DataProcessor
{
    public class FileProcessor
    {
        private static MemoryCache filesToProcess = MemoryCache.Default;
        private static readonly string BackupDirectoryName = "backup";
        private static readonly string InProgressDirectoryName = "processing";
        private static readonly string CompletedDirectoryName = "complete";



        public FileProcessor()
        {

        }

        //public void Process()
        //{
        //    WriteLine($"Begin process of {InputfilePath}");

        //    //check if  file exists before processing
        //    if (!File.Exists(InputfilePath))
        //    {
        //        WriteLine($"Error:file {InputfilePath} does not exist");
        //        return;
        //    }

        //    string rootDirectoryPath = new DirectoryInfo(InputfilePath)
        //        .Parent.Parent.FullName;
        //    WriteLine($"Root data path is {rootDirectoryPath}");


        //    //check if backup dir exists

        //    string inputFileDirectoryPath = Path.GetDirectoryName(InputfilePath);
        //    string backupDirectoryPath = Path.Combine(rootDirectoryPath, BackupDirectoryName);

        //    if (!Directory.Exists(backupDirectoryPath))
        //    {
        //        WriteLine($"Creating {backupDirectoryPath}");
        //        Directory.CreateDirectory(backupDirectoryPath);
        //    }

        //    // Copy file to backup directory 
        //    string inputFileName = Path.GetFileName(InputfilePath);
        //    string backupFilePath = Path.Combine(backupDirectoryPath, inputFileName);
        //    WriteLine($"Copying {InputfilePath} to {backupFilePath}");
        //    File.Copy(InputfilePath, backupFilePath);


        //    // Move to in progress dir
        //    Directory.CreateDirectory(Path.Combine(rootDirectoryPath, InProgressDirectoryName));

        //    string inProgressFilePath =
        //        Path.Combine(rootDirectoryPath,
        //        InProgressDirectoryName, inputFileName);
        //    if (File.Exists(inProgressFilePath))
        //    {
        //        WriteLine($"Error: a file with name {inProgressFilePath} is already being existed");
        //        return;
        //    }

        //    WriteLine($"Moving {InputfilePath} to  {inProgressFilePath}");
        //    File.Move(InputfilePath, inProgressFilePath);



        //    // Determine the type of the file 
        //    string extension = Path.GetExtension(InputfilePath);
        //    switch (extension)
        //    {
        //        case ".txt":
        //            ProcessTextFile(inProgressFilePath);
        //            break;
        //        default:
        //            WriteLine($"{extension} is an unsupported file type.");
        //            break;
        //    }

        //    string completedDirectoryPath = Path.Combine(rootDirectoryPath, CompletedDirectoryName);
        //    Directory.CreateDirectory(completedDirectoryPath);

        //    WriteLine($"Moving {inProgressFilePath} to {completedDirectoryPath}");

        //    var completedFileName =
        //        $"{Path.GetFileNameWithoutExtension(InputfilePath)}-{Guid.NewGuid()}{extension}";

        //    var completedFilePath = Path.Combine(completedDirectoryPath, completedFileName);

        //    File.Move(inProgressFilePath, completedFilePath);

        //    //string inProgressDirectoryPath = Path.GetDirectoryName(inProgressFilePath);

        //    //Directory.Delete(inProgressDirectoryPath, true);

        //}

        private void ProcessTextFile(string inProgressFilePath)
        {
            WriteLine($"Processing text file {inProgressFilePath}");
        }

        public string ReadFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    string extension = System.IO.Path.GetExtension(filePath);
                    string read = string.Empty;
                    switch (extension)
                    {
                        case ".pdf":
                            read = pdfText(filePath);
                            break;
                        case ".txt":
                            read = File.ReadAllText(filePath);
                            break;
                        default:
                            read = "";
                            break;
                    }
                    return read;

                }
                catch (Exception e)
                {
                    Console.WriteLine("The file could not be read:");
                    Console.WriteLine(e.Message);
                    return null;
                }
            }
            else
            {
                return null;
            }

        }


        public bool WriteFile(string filePath)
        {
            try
            {


                string original = ReadFromFile(filePath);
                var rsaParams = new RSAWithCSPKey();
                rsaParams.AssignNewKey();

                var hybrid = new HypridEncryption();

                //var encryptedBlock = hybrid.EncryptedData(original,
                //    rsaParams);

                //var decrypted = hybrid.DecryptData(encryptedBlock, rsaParams);

                Console.WriteLine("Hybrid Encryption");
                Console.WriteLine("--------------------------");
                Console.WriteLine();
                Console.WriteLine("Original Message = " + original);
                Console.WriteLine();
                Console.WriteLine("Decrypted Message = " + Convert.ToBase64String(encryptedBlock.EncryptedData));
                Console.WriteLine();
                Console.WriteLine(" Message After Decryption = " + Encoding.UTF8.GetString(decrypted));

                // Determine the type of the file 
                string extension = System.IO.Path.GetExtension(filePath);

                string rootDirectoryPath = new DirectoryInfo(filePath)
                .Parent.Parent.FullName;

                string backupDirectoryPath = System.IO.Path.Combine(rootDirectoryPath, BackupDirectoryName);
                if (!Directory.Exists(backupDirectoryPath))
                {
                    WriteLine($"Creating {backupDirectoryPath}");
                    Directory.CreateDirectory(backupDirectoryPath);
                }

                var completedFileName =
                $"{System.IO.Path.GetFileNameWithoutExtension(filePath)}-{Guid.NewGuid()}{extension}";
              
                string inputFileName = System.IO.Path.GetFileName(filePath);
                string backupFilePath = System.IO.Path.Combine(backupDirectoryPath, completedFileName);
                //   File.WriteAllText(backupFilePath, Convert.ToBase64String(encryptedBlock.EncryptedData));
                // File.WriteAllBytes(backupFilePath, decrypted);

                switch (extension)
                {
                    case ".pdf":
                        writePdf(encryptedBlock.EncryptedData, backupFilePath, decrypted);
                        break;
                    case ".txt":

                        break;
                    default:
                        break;
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("The file could not be written:");
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public void MonitoringDirectory(string directory)
        {
            var directoryToWatch = directory;

            if (!Directory.Exists(directoryToWatch))
            {
                WriteLine($"ERROR: {directoryToWatch} does not exist");
            }
            else
            {
                WriteLine($"Watching directory {directoryToWatch} for changes");
                ProcessExistingFiles(directoryToWatch);
                using (var inputFileWatcher = new FileSystemWatcher(directoryToWatch))
                {
                    inputFileWatcher.IncludeSubdirectories = false;
                    inputFileWatcher.InternalBufferSize = 32768; //32 KB 
                    //inputFileWatcher.Filter = "*.*";
                    inputFileWatcher.NotifyFilter = NotifyFilters.FileName;
                    inputFileWatcher.Created += FileCreated;
                    inputFileWatcher.Changed += FileChanged;
                    inputFileWatcher.Deleted += FileDeleted;
                    inputFileWatcher.Renamed += FileRenamed;
                    inputFileWatcher.Error += WatcherError;

                    inputFileWatcher.EnableRaisingEvents = true;
                    WriteLine("Press enter to quit. ");
                    // ReadLine();
                }
            }
        }


        private void ProcessExistingFiles(string inputDirectory)
        {
            WriteLine($"Checking {inputDirectory} for existing files");
            foreach (var filePath in Directory.EnumerateFiles(inputDirectory))
            {
                WriteLine($" -Found {filePath}");
                AddToCache(filePath);
            }
            Console.ReadLine();
        }

        private void FileCreated(object sender, FileSystemEventArgs e)
        {
            WriteLine($"* File created: {e.Name} - type: {e.ChangeType}");
            //FileProcessor fileProcessor = new FileProcessor(e.FullPath);
            //fileProcessor.Process();

            AddToCache(e.FullPath);
            Console.ReadLine();
        }

        private void FileChanged(object sender, FileSystemEventArgs e)
        {
            WriteLine($"* File changed: {e.Name} - type: {e.ChangeType}");
        }

        private void FileDeleted(object sender, FileSystemEventArgs e)
        {
            WriteLine($"* File deleted: {e.Name} - type: {e.ChangeType}");
        }

        private void FileRenamed(object sender, RenamedEventArgs e)
        {
            WriteLine($"* File renamed: {e.OldName} to {e.Name} - type: {e.ChangeType}");
        }

        private void WatcherError(object sender, ErrorEventArgs e)
        {
            WriteLine($"ERROR: file system watching may no longer be active: {e.GetException()}");
        }


        private void ProcessingSingleFile(string filePath)
        {
            //WriteFile(filePath, EncryptedContent);
        }



        private void ProcessDirectory(string directoryPath, string fileType)
        {
            // Get all files in directory 
            //var allFiles = Directory.GetFiles(directoryPath);
            switch (fileType)
            {
                case "TEXT":
                    string[] textFiles = Directory.GetFiles(directoryPath, "*.txt");
                    foreach (var textFilePath in textFiles)
                    {
                        //WriteFile(textFilePath, EncryptedContent);
                    }
                    break;
                default:
                    WriteLine($"ERROR: {fileType} is not supported");
                    return;
            }
        }


        private void ProcessFiles(CacheEntryRemovedArguments args)
        {
            WriteLine($"* Cache item removed: {args.CacheItem.Key} because {args.RemovedReason}");
            if (args.RemovedReason == CacheEntryRemovedReason.Expired)
            {
                WriteFile(args.CacheItem.Key);
            }

        }


        private void AddToCache(string fullPath)
        {
            var item = new CacheItem(fullPath, fullPath);
            var policy = new CacheItemPolicy
            {
                RemovedCallback = ProcessFiles,
                SlidingExpiration = TimeSpan.FromSeconds(2)
            };

            filesToProcess.Add(item, policy);
        }


        public static string pdfText(string path)
        {
            PdfReader reader = new PdfReader(path);
          
            string text = string.Empty;
            AcroFields fields = reader.AcroFields;
            foreach (String name in fields.GetSignatureNames())
            {
                Console.Write(" Signature {0}\n", name);

                PdfDictionary sigDict = fields.GetSignatureDictionary(name);
                PdfName subFilter = sigDict.GetAsName(PdfName.SUBFILTER);
                Console.Write("  SubFilter {0}\n", subFilter);

                PdfString contents = sigDict.GetAsString(PdfName.CONTENTS);
                if (contents != null)
                {
                    byte[] contentBytes = contents.GetOriginalBytes();
                    string contentBas64 = Convert.ToBase64String(contentBytes);
                    // contentBytes contains the actual signature container as is,
                    // contentBas64 contains it encoded using Base64 for better printability
                    Console.Write("  Content {0}\n", contentBas64);
                }
            }
            for (int page = 1; page <= reader.NumberOfPages; page++)
            {
                text += PdfTextExtractor.GetTextFromPage(reader, page);
            }
            reader.Close();
            return text;
        }

        public static bool writePdf(byte[] encrypted, string str, byte[] decrypted )
        {
            File.WriteAllText(str, Convert.ToBase64String(encrypted));
            //   string oldFile = @"E:\Project\oldFile.pdf";
            string newFile = @"E:\Project\newFile.pdf";


            using (FileStream fsNew = new FileStream(newFile,
               FileMode.Create, FileAccess.Write))
            {
                fsNew.Write(decrypted, 0, decrypted.Length);
            }

            PdfReader reader = new PdfReader(str);
            
            Rectangle size = reader.GetPageSizeWithRotation(1);
            Document document = new Document(size);
          

            FileStream fs = new FileStream(newFile, FileMode.Create, FileAccess.Write);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            document.Open();

            PdfContentByte cb = writer.DirectContent;

            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            cb.SetColorFill(BaseColor.DARK_GRAY);
            cb.SetFontAndSize(bf, 8);

            cb.BeginText();
            string text = "Some random blablablabla...";
            cb.ShowTextAligned(1, text, 520, 640, 0);
            cb.EndText();
            cb.BeginText();
            text = "Other random blabla...";
            cb.ShowTextAligned(2, text, 100, 200, 0);
            cb.EndText();

            PdfImportedPage page = writer.GetImportedPage(reader, 1);
            cb.AddTemplate(page, 0, 0);

            document.Close();
            fs.Close();
            writer.Close();
            reader.Close();
            return true;
        }

    }



}


