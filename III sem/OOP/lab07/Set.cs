using System;
using System.Collections.Generic;
using System.Linq;

namespace lab07 {

    public interface ICollectionOperations<T>
    {
        void Add(T item);
        bool Remove(T item);
        IEnumerable<T> View();
    }

    public class CollectionType<T> : ICollectionOperations<T> where T : IComparable<T>
    {
        private List<T> _items;

        public CollectionType()
        {
            _items = new List<T>();
        }

        public void Add(T item)
        {
            try
            {
                _items.Add(item);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка добавления элемента: {ex.Message}");
            }
        }

        public bool Remove(T item)
        {
            try
            {
                return _items.Remove(item);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка удаления элемента: {ex.Message}");
                return false;
            }
            finally
            {
                Console.WriteLine("Элемент удалён.");
            }
        }

        public IEnumerable<T> View()
        {
            return _items;
        }

        public T FindByPredicate(Func<T, bool> predicate)
        {
            return _items.FirstOrDefault(predicate);
        }

        public override string ToString()
        {
            return string.Join(", ", _items);
        }
    }

    public static class File<T> where T : IComparable<T>
    {
        public static void SaveToFile(CollectionType<T> collection, string filePath)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (var item in collection.View())
                    {
                        writer.WriteLine(item?.ToString());
                    }
                }
                Console.WriteLine("Данные успешно сохранены в файл.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения в файл: {ex.Message}");
            }
        }

        public static CollectionType<T> LoadFromFile(string filePath, Func<string, T> parser)
        {
            try
            {
                var collection = new CollectionType<T>();
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        T item = parser(line);
                        collection.Add(item);
                    }
                }
                Console.WriteLine("Данные успешно загружены из файла.");
                return collection;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки из файла: {ex.Message}");
                return null;
            }
        }
    }

    public class CustomType : IComparable<CustomType>
    {
        public string Name { get; set; }
        public int Id { get; set; }

        public CustomType(string name, int id)
        {
            Name = name;
            Id = id;
        }

        public int CompareTo(CustomType other)
        {
            if (other == null) return 1;
            return Id.CompareTo(other.Id);
        }

        public override string ToString()
        {
            return $"{Name} (ID: {Id})";
        }
    }










    public class Set {
        private List<int> _elements;
        public List<int> elements {
            get { return _elements; }
            set { _elements = value; }
        }

        public class Production {
            public int Id { get; set; }
            public string OrganizationName { get; set; }

            public Production(int id, string orgName) {
                Id = id;
                OrganizationName = orgName;
            }

            public override string ToString() {
                return $"{OrganizationName} (ID: {Id})";
            }
        }

        public class Developer {
            public string FullName { get; set; }
            public int Id { get; set; }
            public string Department { get; set; }

            public Developer(string fullName, int id, string department) {
                FullName = fullName;
                Id = id;
                Department = department;
            }

            public override string ToString() {
                return $"{FullName}, ID: {Id}, Отдел: {Department}";
            }
        }

        public Production production { get; set; }
        public Developer developer { get; set; }

        public Set(List<int> elements, Production production, Developer developer) {
            this.elements = elements;
            this.production = production;
            this.developer = developer;
        }

        public int this[int index] {
            get { return elements[index]; }
            set { elements[index] = value; }
        }

        public static Set operator +(Set set, int element)
        {
            set.Add(element);
            return set;
        }

        public static Set operator +(Set set1, Set set2) {
            var union = set1.elements.Union(set2.elements).ToList();
            return new Set(union, set1.production, set1.developer);
        }

        public static Set operator -(Set set, int element)
        {
            set.Remove(element);
            return set;
        }

        public static Set operator *(Set set1, Set set2) {
            var intersection = set1.elements.Intersect(set2.elements).ToList();
            return new Set(intersection, set1.production, set1.developer);
        }

        public static explicit operator int(Set set) {
            return set.elements.Count;
        }

        public static bool operator false(Set set) {
            return set.elements.Count >= 5 && set.elements.Count <= 10;
        }

        public static bool operator true(Set set) {
            return set.elements.Count < 5 || set.elements.Count > 10;
        }

        public void Add(int element) {
            if (!elements.Contains(element)) {
                elements.Add(element);
            }
        }

        public void Remove(int element) {
            elements.Remove(element);
        }

        public override string ToString()
        {
            return string.Join(", ", elements);
        }
    }
}
