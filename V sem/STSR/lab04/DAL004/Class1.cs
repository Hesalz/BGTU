using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace DAL004
{
    public class UpdateCelebrityException : Exception
    {
        public UpdateCelebrityException(string message) : base($"UpdateCelebrity error: {message}") { }
    }
    public class DeleteCelebrityException : Exception
    {
        public DeleteCelebrityException(string message) : base($"DeleteCelebrity error: {message}") { }
    }
    public class FoundByIdException : Exception
    {
        public FoundByIdException(string message) : base($"Found by Id: {message}") { }
    }
    public class SaveException : Exception
    {
        public SaveException(string message) : base($"SaveChanges error:{message}") { }
    }
    public class AddCelebrityException : Exception
    {
        public AddCelebrityException(string message) : base($"AddCelebrityException error: {message}") { }
    }

    public interface IRepository : IDisposable
    {
        string BasePath { get; }
        Celebrity[] getAllCelebrities();
        Celebrity? getCelebrityById(int id);
        Celebrity[] getCelebritiesBySurname(string name);
        string? getPhotoPathId(int id);
        int? addCelebrity(Celebrity celebrity);
        bool delCelebrityById(int Id);
        int? updCelebrityById(int Id, Celebrity celebrity);
        int SaveChanges();
    }

    public record Celebrity(int Id, string Firstname, string Surname, string PhotoPath);

    public class Repository : IRepository
    {
        public static string? JSONFileName { get; set; }

        public string BasePath { get; private set; } = string.Empty;
        private Celebrity[] _celebrities;

        public Repository(string basePath)
        {
            JSONFileName ??= "Celebrities.json";

            BasePath = FindCelebritiesFolder();

            string jsonPath = Path.Combine(BasePath, JSONFileName);

            Console.WriteLine($"BasePath: {BasePath}");
            Console.WriteLine($"JSON path: {jsonPath}");

            if (File.Exists(jsonPath))
            {
                try
                {
                    string jsonData = File.ReadAllText(jsonPath);
                    _celebrities = System.Text.Json.JsonSerializer.Deserialize<Celebrity[]>(jsonData)
                                   ?? Array.Empty<Celebrity>();
                    Console.WriteLine($"данные получены из {jsonPath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ошибка при чтении JSON: {ex.Message}");
                    _celebrities = Array.Empty<Celebrity>();
                }
            }
            else
            {
                try
                {
                    Directory.CreateDirectory(BasePath);
                }
                catch { }

                _celebrities = Array.Empty<Celebrity>();
                Console.WriteLine($"данных нет. Ожидаемый путь: {jsonPath}");
            }
        }

        private string FindCelebritiesFolder()
        {
            string targetPath = @"D:\БГТУ\V сем\СТСР\lab04\DAL004\Celebrities";

            if (Directory.Exists(targetPath))
            {
                return targetPath;
            }

            var searchPaths = new List<string>
            {
                targetPath,
                Path.Combine(Directory.GetCurrentDirectory(), "DAL004", "Celebrities"),
                Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "DAL004", "Celebrities"),
                Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "DAL004", "Celebrities"),
                Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "DAL004", "Celebrities"),
                Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "DAL004", "Celebrities")),
                Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "DAL004", "Celebrities"))
            };

            foreach (var path in searchPaths)
            {
                try
                {
                    string fullPath = Path.GetFullPath(path);
                    if (Directory.Exists(fullPath))
                    {
                        Console.WriteLine($"Найдена папка Celebrities: {fullPath}");
                        return fullPath;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при проверке пути {path}: {ex.Message}");
                }
            }

            string fallbackPath = Path.Combine(Directory.GetCurrentDirectory(), "Celebrities");
            Console.WriteLine($"Папка Celebrities не найдена, используем: {fallbackPath}");
            return fallbackPath;
        }

        public Celebrity? getCelebrityById(int id) => _celebrities.FirstOrDefault(c => c.Id == id);

        public Celebrity[] getAllCelebrities() => _celebrities;

        public Celebrity[] getCelebritiesBySurname(string surname) => _celebrities.Where(c => c.Surname == surname).ToArray();

        public string? getPhotoPathId(int id) => _celebrities.Where(c => c.Id == id).FirstOrDefault()?.PhotoPath;

        public int? addCelebrity(Celebrity celebrity)
        {
            if (celebrity == null) throw new ArgumentNullException(nameof(celebrity), "Celebrity cannot be null");

            if (string.IsNullOrWhiteSpace(celebrity.Firstname) || string.IsNullOrWhiteSpace(celebrity.Surname))
            {
                throw new ArgumentException("First name and surname are required");
            }

            if (celebrity.Id > 0 && _celebrities.Any(c => c.Id == celebrity.Id))
            {
                throw new Exception("Celebrity with this ID already exists");
            }

            if (!string.IsNullOrWhiteSpace(celebrity.PhotoPath))
            {
                string photoFullPath = Path.Combine(BasePath, celebrity.PhotoPath);

                Console.WriteLine($"Looking for photo: {photoFullPath}");

                if (!File.Exists(photoFullPath))
                {
                    throw new FileNotFoundException($"Could not find file '{photoFullPath}'");
                }
            }

            int newId = _celebrities.Length > 0 ? _celebrities.Max(c => c.Id) + 1 : 1;
            var newCelebrity = celebrity with { Id = newId };

            var newArrayOfCelebrities = _celebrities.Append(newCelebrity).ToArray();
            _celebrities = newArrayOfCelebrities;

            return newCelebrity.Id;
        }

        public bool delCelebrityById(int Id)
        {
            var celebrityToDelete = _celebrities.FirstOrDefault(c => c.Id == Id);
            if (celebrityToDelete == null) return false;

            _celebrities = _celebrities.Where(_c => _c.Id != Id).ToArray();
            return true;
        }

        public int? updCelebrityById(int Id, Celebrity celebrity)
        {
            var celebrityToChange = _celebrities.FirstOrDefault(c => c.Id == Id);
            if (celebrityToChange == null) return null;

            var updated = celebrity with { Id = Id };

            var list = _celebrities.ToList();
            int idx = list.IndexOf(celebrityToChange);
            if (idx >= 0)
            {
                list[idx] = updated;
                _celebrities = list.ToArray();
                return updated.Id;
            }
            return null;
        }

        public int SaveChanges()
        {
            if (string.IsNullOrWhiteSpace(BasePath))
            {
                Console.WriteLine("BasePath не задан. Сохранение невозможно.");
                return 0;
            }

            string targetDir = BasePath;
            string jsonPath = Path.Combine(targetDir, JSONFileName!);

            try
            {
                Directory.CreateDirectory(targetDir);

                var serCeleb = System.Text.Json.JsonSerializer.Serialize(_celebrities, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(jsonPath, serCeleb);
                Console.WriteLine($"данные сохранены в {jsonPath}");
                return _celebrities.Length;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"данные не сохранены: {ex.Message}");
                return 0;
            }
        }

        public void Dispose() { }
    }
}