using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Collections.Generic;

namespace HAProject
{
    public class HALog
    {
        private readonly string logFilePath;

        public HALog(string logFilePath)
        {
            this.logFilePath = logFilePath;
            if (!File.Exists(logFilePath))
                File.Create(logFilePath).Close();
        }

        public void WriteLog(string action, string details)
        {
            string logEntry = $"{DateTime.Now}: {action} - {details}";
            using (var writer = new StreamWriter(logFilePath, append: true))
            {
                writer.WriteLine(logEntry);
            }
        }

        public string ReadLog()
        {
            using (var reader = new StreamReader(logFilePath))
            {
                return reader.ReadToEnd();
            }
        }

        public IEnumerable<string> SearchLog(string keyword)
        {
            using (var reader = new StreamReader(logFilePath))
            {
                return reader.ReadToEnd().Split(Environment.NewLine)
                    .Where(line => line.Contains(keyword));
            }
        }

        public IEnumerable<string> SearchByDate(DateTime date)
        {
            using (var reader = new StreamReader(logFilePath))
            {
                return reader.ReadToEnd().Split(Environment.NewLine)
                    .Where(line => line.Contains(date.ToShortDateString()));
            }
        }

        public IEnumerable<string> SearchByTimeRange(DateTime startTime, DateTime endTime)
        {
            using (var reader = new StreamReader(logFilePath))
            {
                return reader.ReadToEnd().Split(Environment.NewLine)
                    .Where(line => DateTime.TryParse(line.Split(':')[0], out var timestamp)
                                && timestamp >= startTime && timestamp <= endTime);
            }
        }

        public void KeepEntriesForCurrentHour()
        {
            var currentHour = DateTime.Now.ToString("yyyy-MM-dd HH");
            IEnumerable<string> filteredLines;

            using (var reader = new StreamReader(logFilePath))
            {
                filteredLines = reader.ReadToEnd()
                    .Split(Environment.NewLine)
                    .Where(line => line.StartsWith(currentHour))
                    .ToList();
            }

            File.WriteAllLines(logFilePath, filteredLines);
        }
        public int CountEntries()
        {
            using (var reader = new StreamReader(logFilePath))
            {
                return reader.ReadToEnd().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Length;
            }
        }
    }

    public class HADiskInfo
    {
        public void DisplayDiskInfo()
        {
            try
            {
                var drives = DriveInfo.GetDrives();

                foreach (var drive in drives.Where(d => d.IsReady))
                {
                    Console.WriteLine($"Имя диска: {drive.Name}");
                    Console.WriteLine($"Метка тома: {drive.VolumeLabel}");
                    Console.WriteLine($"Формат диска: {drive.DriveFormat}");
                    Console.WriteLine($"Общий размер: {drive.TotalSize / 1_000_000} MB");
                    Console.WriteLine($"Свободное место: {drive.AvailableFreeSpace / 1_000_000} MB");
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка получения информации о дисках: {ex.Message}");
            }
        }
    }

    public class HAFileInfo
    {
        public void DisplayFileInfo(string filePath)
        {
            try
            {
                var fileInfo = new FileInfo(filePath);
                if (!fileInfo.Exists)
                {
                    Console.WriteLine("Файл не найден.");
                    return;
                }

                Console.WriteLine($"Полный путь: {fileInfo.FullName}");
                Console.WriteLine($"Размер: {fileInfo.Length} байтов");
                Console.WriteLine($"Расширение: {fileInfo.Extension}");
                Console.WriteLine($"Имя: {fileInfo.Name}");
                Console.WriteLine($"Время создания: {fileInfo.CreationTime}");
                Console.WriteLine($"Время последнего изменения: {fileInfo.LastWriteTime}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка получения информации о файле: {ex.Message}");
            }
        }
    }

    public class HADirInfo
    {
        public void DisplayDirInfo(string dirPath)
        {
            try
            {
                var dirInfo = new DirectoryInfo(dirPath);
                if (!dirInfo.Exists)
                {
                    Console.WriteLine("Директория не найдена.");
                    return;
                }

                Console.WriteLine($"Имя директории: {dirInfo.Name}");
                Console.WriteLine($"Время создания: {dirInfo.CreationTime}");
                Console.WriteLine($"Количество файлов: {dirInfo.GetFiles().Length}");
                Console.WriteLine($"Количество поддиректорий: {dirInfo.GetDirectories().Length}");

                Console.WriteLine("Родительские директории:");
                var parent = dirInfo.Parent;
                while (parent != null)
                {
                    Console.WriteLine(parent.FullName);
                    parent = parent.Parent;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка получения информации о директории: {ex.Message}");
            }
        }
    }

    public class HAFileManager
    {
        public void ManageFiles(string drive)
        {
            try
            {
                string inspectDir = Path.Combine(drive, "HAInspect");
                Directory.CreateDirectory(inspectDir);

                string logFile = Path.Combine(inspectDir, "hadirinfo.txt");
                File.WriteAllText(logFile, "Проверка работоспособности");

                string copiedFile = Path.Combine(inspectDir, "copy.txt");

                if (File.Exists(copiedFile))
                {
                    File.Delete(copiedFile);
                }

                File.Copy(logFile, copiedFile, overwrite: true);
                File.Delete(logFile);

                string filesDir = Path.Combine(drive, "HAFiles");
                Directory.CreateDirectory(filesDir);

                foreach (var file in Directory.GetFiles(drive, "*.txt"))
                {
                    string destinationFile = Path.Combine(filesDir, Path.GetFileName(file));

                    if (File.Exists(destinationFile))
                    {
                        File.Delete(destinationFile);
                    }

                    File.Copy(file, destinationFile, overwrite: true);
                }

                string archive = Path.Combine(inspectDir, "HAFiles.zip");

                if (File.Exists(archive))
                {
                    File.Delete(archive);
                }

                ZipFile.CreateFromDirectory(filesDir, archive);
                Directory.Delete(filesDir, true);

                string extractedDir = Path.Combine(drive, "Извлеченные файлы");
                Directory.CreateDirectory(extractedDir);
                ZipFile.ExtractToDirectory(archive, extractedDir);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка управления файлами: {ex.Message}");
            }
        }

        public void ArchiveAndExtractDirectory(string sourceDir, string archivePath, string extractDir)
        {
            try
            {
                if (Directory.Exists(sourceDir))
                {
                    if (File.Exists(archivePath))
                    {
                        File.Delete(archivePath);
                    }

                    ZipFile.CreateFromDirectory(sourceDir, archivePath);
                    Console.WriteLine($"Архив создан: {archivePath}");

                    if (Directory.Exists(extractDir))
                    {
                        Directory.Delete(extractDir, true);
                    }

                    ZipFile.ExtractToDirectory(archivePath, extractDir);
                    Console.WriteLine($"Файлы извлечены в директорию: {extractDir}");
                }
                else
                {
                    Console.WriteLine("Исходная директория не существует.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при работе с архивами: {ex.Message}");
            }
        }
    }

    class Program
    {
        static void Main()
        {
            string logPath = "halogfile.txt";
            var logger = new HALog(logPath);

            try
            {
                logger.WriteLog("Инициализация", "Программа запущена");
                logger.WriteLog("Действие", "Проверка логов");

                Console.WriteLine("\nЗаписи с ключевым словом 'Действие':");
                foreach (var line in logger.SearchLog("Действие"))
                {
                    Console.WriteLine(line);
                }

                var diskInfo = new HADiskInfo();
                diskInfo.DisplayDiskInfo();

                var fileInfo = new HAFileInfo();
                fileInfo.DisplayFileInfo(logPath);

                var dirInfo = new HADirInfo();
                dirInfo.DisplayDirInfo(".");

                var fileManager = new HAFileManager();
                fileManager.ManageFiles(Path.GetPathRoot(Environment.CurrentDirectory));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }
    }
}
