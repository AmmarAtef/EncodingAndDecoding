using static System.Console;

using System.IO;
using System.Text;
using System;
using System.Runtime.Caching;
using RSA;
using System.Security.Cryptography;

namespace DataProcessor
{
    public class FileProcessor
    {
        private static MemoryCache filesToProcess = MemoryCache.Default;
        private static MemoryCache decryptedFilesToProcess = MemoryCache.Default;
        private static readonly string EncryptedDirectoryName = "Encrypted";
        private static readonly string DecryptedDirectoryName = "Decrypted";
        private static readonly string SourceFolderDirectoryName = "SourceFolder";



        public FileProcessor()
        {

        }


        public string DecryptFile(string filePath)
        {
            string extension = Path.GetExtension(filePath);
            //WriteFile(args.CacheItem.Key);
            var encryptedFileName = $"{System.IO.Path.GetFileNameWithoutExtension(filePath)}-{Guid.NewGuid()}{extension}";

            string inputFileName = System.IO.Path.GetFileName(filePath);
            string rootDirectoryPath = new DirectoryInfo(filePath)
           .Parent.Parent.FullName;

            string decryptedDirectoryPath = System.IO.Path.Combine(rootDirectoryPath, DecryptedDirectoryName);

            string decryptedFilePath = System.IO.Path.Combine(decryptedDirectoryPath, encryptedFileName);

            RSAWithCSPKey rSAWithCSPKey = new RSAWithCSPKey();
            return rSAWithCSPKey.DecryptData(filePath, decryptedFilePath);
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
               
                using (var inputFileWatcher = new FileSystemWatcher(directoryToWatch))
                {
                    inputFileWatcher.IncludeSubdirectories = false;
                    inputFileWatcher.InternalBufferSize = 32768; //32 KB 
                    //inputFileWatcher.Filter = "*.*";
                    inputFileWatcher.NotifyFilter = NotifyFilters.FileName 
                        | NotifyFilters.LastWrite | NotifyFilters.LastAccess;
                    inputFileWatcher.Created += FileCreated;
                    inputFileWatcher.Changed += FileChanged;
                    inputFileWatcher.Deleted += FileDeleted;
                    inputFileWatcher.Renamed += FileRenamed;
                    inputFileWatcher.Error += WatcherError;

                    inputFileWatcher.EnableRaisingEvents = true;
                    
                    WriteLine("Press enter to quit. ");
                     ReadLine();
                }
                ProcessExistingFiles(directoryToWatch);
            }
        }

        public void MonitoringEncryptedDirectory(string directory)
        {
            var directoryToWatch = directory;

            if (!Directory.Exists(directoryToWatch))
            {
                WriteLine($"ERROR: {directoryToWatch} does not exist");
            }
            else
            {
                WriteLine($"Watching directory {directoryToWatch} for changes");
                
                using (var inputFileWatcher = new FileSystemWatcher(directoryToWatch))
                {
                    inputFileWatcher.IncludeSubdirectories = false;
                    inputFileWatcher.InternalBufferSize = 32768; //32 KB 
                    //inputFileWatcher.Filter = "*.*";
                    inputFileWatcher.NotifyFilter = NotifyFilters.FileName | 
                        NotifyFilters.LastWrite | NotifyFilters.LastAccess; 
                    inputFileWatcher.Created += EncryptedFileCreated;
                    inputFileWatcher.Changed += EncryptedFileChanged;
                    inputFileWatcher.Deleted += EncryptedFileDeleted;
                    inputFileWatcher.Renamed += EncryptedFileRenamed;
                    inputFileWatcher.Error += EncryptedWatcherError;

                    inputFileWatcher.EnableRaisingEvents = true;
                    WriteLine("Press enter to quit. ");
                     ReadLine();
                
                }
                ProcessEncryptedExistingFiles(directoryToWatch);
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
            
        }
        private void ProcessEncryptedExistingFiles(string inputDirectory)
        {
            WriteLine($"Checking {inputDirectory} for existing files");
            foreach (var filePath in Directory.EnumerateFiles(inputDirectory))
            {
                WriteLine($" -Found {filePath}");
                AddEncryptedFilesToCache(filePath);
            }
        }
        private void EncryptedFileCreated(object sender, FileSystemEventArgs e)
        {
            WriteLine($"* File created: {e.Name} - type: {e.ChangeType}");
            //FileProcessor fileProcessor = new FileProcessor(e.FullPath);
            //fileProcessor.Process();

            AddEncryptedFilesToCache(e.FullPath);
            
        }

        private void EncryptedFileChanged(object sender, FileSystemEventArgs e)
        {
            WriteLine($"* File changed: {e.Name} - type: {e.ChangeType}");
        }

        private void EncryptedFileDeleted(object sender, FileSystemEventArgs e)
        {
            WriteLine($"* File deleted: {e.Name} - type: {e.ChangeType}");
        }

        private void EncryptedFileRenamed(object sender, RenamedEventArgs e)
        {
            WriteLine($"* File renamed: {e.OldName} to {e.Name} - type: {e.ChangeType}");
        }

        private void EncryptedWatcherError(object sender, ErrorEventArgs e)
        {
            WriteLine($"ERROR: file system watching may no longer be active: {e.GetException()}");
        }


        private void FileCreated(object sender, FileSystemEventArgs e)
        {
            WriteLine($"* File created: {e.Name} - type: {e.ChangeType}");
            //FileProcessor fileProcessor = new FileProcessor(e.FullPath);
            //fileProcessor.Process();

            AddToCache(e.FullPath);
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
                string extension = Path.GetExtension(args.CacheItem.Key);
                //WriteFile(args.CacheItem.Key);
                var encryptedFileName = $"{System.IO.Path.GetFileNameWithoutExtension(args.CacheItem.Key)}-{Guid.NewGuid()}{extension}";

                string inputFileName = System.IO.Path.GetFileName(args.CacheItem.Key);
                string rootDirectoryPath = new DirectoryInfo(args.CacheItem.Key)
               .Parent.Parent.FullName;

                string backupDirectoryPath = System.IO.Path.Combine(rootDirectoryPath, EncryptedDirectoryName);

                string encryptedFilePath = System.IO.Path.Combine(backupDirectoryPath, encryptedFileName);
                RSAWithCSPKey rSAWithCSPKey = new RSAWithCSPKey();
                rSAWithCSPKey.EncryptData(args.CacheItem.Key, encryptedFilePath);
            }

        }



        private void ProcessEncryptedFiles(CacheEntryRemovedArguments args)
        {
            WriteLine($"* Cache item removed: {args.CacheItem.Key} because {args.RemovedReason}");
            if (args.RemovedReason == CacheEntryRemovedReason.Expired)
            {
                string extension = Path.GetExtension(args.CacheItem.Key);
                //WriteFile(args.CacheItem.Key);
                var decryptedFileName = $"{System.IO.Path.GetFileNameWithoutExtension(args.CacheItem.Key)}-{Guid.NewGuid()}{extension}";

                string inputFileName = System.IO.Path.GetFileName(args.CacheItem.Key);
                string rootDirectoryPath = new DirectoryInfo(args.CacheItem.Key)
               .Parent.Parent.FullName;

                string decryptedDirectoryPath = System.IO.Path.Combine(rootDirectoryPath, DecryptedDirectoryName);

                string decryptedFilePath = System.IO.Path.Combine(decryptedDirectoryPath, decryptedFileName);
                RSAWithCSPKey rSAWithCSPKey = new RSAWithCSPKey();
                rSAWithCSPKey.DecryptData(args.CacheItem.Key, decryptedFilePath);
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

        private void AddEncryptedFilesToCache(string fullPath)
        {
            var item = new CacheItem(fullPath, fullPath);
            var policy = new CacheItemPolicy
            {
                RemovedCallback = ProcessEncryptedFiles,
                SlidingExpiration = TimeSpan.FromSeconds(2)
            };

            decryptedFilesToProcess.Add(item, policy);
        }


    }



}


