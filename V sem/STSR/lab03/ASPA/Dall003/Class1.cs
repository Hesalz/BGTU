using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using Newtonsoft.Json;
using System.Reflection;
using System.Text.Json;
using System;

namespace Dall003
{
    public interface IRepository : IDisposable
    {
        string BasePath { get; }
        Celebrity[] getAllCelebrities();
        Celebrity? getCelebrityById(int id);
        Celebrity[] getCelebritiesBySurname(string name);
        string? getPhotoPathId(int id);
    }

    public record Celebrity(int Id, string Firstname, string Surname, string PhotoPath);
    public class Repository : IRepository
    {
        public static string? JSONFileName { get; set; }
        public string BasePath { get; }
        private Celebrity[] _celebrities;

        public Repository(string basePath)
        {

            BasePath = Path.Combine(@"D:\БГТУ\V сем\СТСР\lab03\ASPA\Dall003", basePath);
            string jsonPath = Path.Combine(BasePath, JSONFileName);
            if (File.Exists(jsonPath))
            {    
                string jsonData = File.ReadAllText(jsonPath);
                _celebrities = System.Text.Json.JsonSerializer.Deserialize<Celebrity[]>(jsonData) ?? Array.Empty<Celebrity>();
                Console.WriteLine("Данные загружены");
            }
            else
            {
                _celebrities = Array.Empty<Celebrity>();
                Console.WriteLine("Данных нет");
            }
        }
        public Celebrity? getCelebrityById(int id) => _celebrities.FirstOrDefault(c => c.Id == id);
        public Celebrity[] getAllCelebrities() => _celebrities;
        public Celebrity[] getCelebritiesBySurname(string surname) => _celebrities.Where(c => c.Surname == surname).ToArray();
        public string? getPhotoPathId(int id) => _celebrities.Where(c => c.Id == id).FirstOrDefault()?.PhotoPath;
        public void Dispose(){}
    }
}