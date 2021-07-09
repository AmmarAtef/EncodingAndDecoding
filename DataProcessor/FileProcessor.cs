using static System.Console;
using System.IO;
using System.Text;
using System;
using System.Runtime.Caching;
using RSA;

namespace DataProcessor
{
    public class FileProcessor
    {
        private static MemoryCache filesToProcess = MemoryCache.Default;
        private static readonly string BackupDirectoryName = "backup";
        private static readonly string InProgressDirectoryName = "processing";
        private static readonly string CompletedDirectoryName = "complete";

        private string EncryptedContent { get; }

        public FileProcessor(string encryptedContent)
        {
            EncryptedContent = encryptedContent;
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
                    string fileContent = string.Empty;
                    using (StreamReader sr = new StreamReader(filePath))
                    {
                        string line;

                        while ((line = sr.ReadLine()) != null)
                        {
                            fileContent = fileContent + "\n" + line;
                        }

                    }
                    return fileContent;
                }
                catch (Exception e)
                {
                    Console.WriteLine("The file could not be read:");
                    Console.WriteLine(e.Message);
                    return "";
                }
            }
            else
            {
                return "";
            }

        }


        public bool WriteFile(string filePath, string encryptedContent)
        {
            try
            {
                // Determine the type of the file 
                string extension = Path.GetExtension(filePath);

                string rootDirectoryPath = new DirectoryInfo(filePath)
                .Parent.Parent.FullName;

                string backupDirectoryPath = Path.Combine(rootDirectoryPath, BackupDirectoryName);
                if (!Directory.Exists(backupDirectoryPath))
                {
                    WriteLine($"Creating {backupDirectoryPath}");
                    Directory.CreateDirectory(backupDirectoryPath);
                }

                var completedFileName =
                $"{Path.GetFileNameWithoutExtension(filePath)}-{Guid.NewGuid()}{extension}";

                string inputFileName = Path.GetFileName(filePath);
                string backupFilePath = Path.Combine(backupDirectoryPath, completedFileName);
                File.WriteAllText(backupFilePath, encryptedContent);
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
                    ReadLine();
                }
            }
        }

        
        private  void ProcessExistingFiles(string inputDirectory)
        {
            WriteLine($"Checking {inputDirectory} for existing files");
            foreach (var filePath in Directory.EnumerateFiles(inputDirectory))
            {
                WriteLine($" -Found {filePath}");
                AddToCache(filePath);
            }
        }

        private  void FileCreated(object sender, FileSystemEventArgs e)
        {
            WriteLine($"* File created: {e.Name} - type: {e.ChangeType}");
            //FileProcessor fileProcessor = new FileProcessor(e.FullPath);
            //fileProcessor.Process();

            AddToCache(e.FullPath);
        }

        private  void FileChanged(object sender, FileSystemEventArgs e)
        {
            WriteLine($"* File changed: {e.Name} - type: {e.ChangeType}");
        }

        private  void FileDeleted(object sender, FileSystemEventArgs e)
        {
            WriteLine($"* File deleted: {e.Name} - type: {e.ChangeType}");
        }

        private  void FileRenamed(object sender, RenamedEventArgs e)
        {
            WriteLine($"* File renamed: {e.OldName} to {e.Name} - type: {e.ChangeType}");
        }

        private  void WatcherError(object sender, ErrorEventArgs e)
        {
            WriteLine($"ERROR: file system watching may no longer be active: {e.GetException()}");
        }


        private  void ProcessingSingleFile(string filePath)
        {
            var fileProcesser = new FileProcessor(filePath);
            fileProcesser.WriteFile(filePath, EncryptedContent);
        }



        private  void ProcessDirectory(string directoryPath, string fileType)
        {
            // Get all files in directory 
            //var allFiles = Directory.GetFiles(directoryPath);
            switch (fileType)
            {
                case "TEXT":
                    string[] textFiles = Directory.GetFiles(directoryPath, "*.txt");
                    foreach (var textFilePath in textFiles)
                    {
                        var fileProcessor = new FileProcessor(textFilePath);
                        fileProcessor.WriteFile(textFilePath, EncryptedContent);
                    }
                    break;
                default:
                    WriteLine($"ERROR: {fileType} is not supported");
                    return;
            }
        }


        private  void ProcessFiles(CacheEntryRemovedArguments args)
        {
            WriteLine($"* Cache item removed: {args.CacheItem.Key} because {args.RemovedReason}");
            if (args.RemovedReason == CacheEntryRemovedReason.Expired)
            {
                var fileProcessor = new FileProcessor(args.CacheItem.Key);
                fileProcessor.WriteFile(args.CacheItem.Key, EncryptedContent);
            }

        }


        private  void AddToCache(string fullPath)
        {
            var item = new CacheItem(fullPath, fullPath);
            var policy = new CacheItemPolicy
            {
                RemovedCallback = ProcessFiles,
                SlidingExpiration = TimeSpan.FromSeconds(2)
            };

            filesToProcess.Add(item, policy);
        }
        
    }
}