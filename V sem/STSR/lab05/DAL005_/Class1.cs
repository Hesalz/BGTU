using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace DAL005
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

            string? foundJson = LocateJsonFile(basePath, JSONFileName);

            if (foundJson != null)
            {
                BasePath = Path.GetDirectoryName(foundJson)!;
                try
                {
                    string jsonData = File.ReadAllText(foundJson);
                    _celebrities = System.Text.Json.JsonSerializer.Deserialize<Celebrity[]>(jsonData)
                                   ?? Array.Empty<Celebrity>();
                    Console.WriteLine($"данные получены из {foundJson}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ошибка при чтении JSON: {ex.Message}");
                    _celebrities = Array.Empty<Celebrity>();
                }
            }
            else
            {
                BasePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "DAL005_", basePath));
                try
                {
                    Directory.CreateDirectory(BasePath);
                }
                catch { }

                _celebrities = Array.Empty<Celebrity>();
                Console.WriteLine($"данных нет. Ожидаемый путь: {Path.Combine(BasePath, JSONFileName)}");
            }
        }

        private string? LocateJsonFile(string basePathDir, string jsonFileName)
        {
            string[] simplePaths = {
        Path.Combine("..", "DAL005_", "Celebrities", jsonFileName),
        Path.Combine("..", "..", "DAL005_", "Celebrities", jsonFileName),
        Path.Combine("DAL005_", "Celebrities", jsonFileName),
        Path.Combine("Celebrities", jsonFileName),
        Path.Combine("..", "Celebrities", jsonFileName),
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "DAL005_", "Celebrities", jsonFileName),
        Path.Combine(Directory.GetCurrentDirectory(), "..", "DAL005_", "Celebrities", jsonFileName)
    };

            foreach (var path in simplePaths)
            {
                var fullPath = Path.GetFullPath(path);
                if (File.Exists(fullPath))
                {
                    Console.WriteLine($"Найден файл: {fullPath}");
                    return fullPath;
                }
            }

            Console.WriteLine("Файл не найден. Проверенные пути:");
            foreach (var path in simplePaths)
            {
                Console.WriteLine($"  {Path.GetFullPath(path)}");
            }

            return null;
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

            //if (celebrity.Id > 0 && _celebrities.Any(c => c.Id == celebrity.Id))
            //{
            //    throw new Exception("Celebrity with this ID already exists");
            //}

            string photoFullPath = Path.IsPathRooted(celebrity.PhotoPath)
                ? celebrity.PhotoPath
                : Path.Combine(BasePath, celebrity.PhotoPath);

            if (string.IsNullOrWhiteSpace(celebrity.PhotoPath))// !File.Exists(celebrity.PhotoPath))
            {
                throw new Exception("Invalid photo path");
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
