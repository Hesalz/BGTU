using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

public class Product : IOrderedDictionary
{
    private readonly ConcurrentDictionary<int, object> _items = new();
    private int _orderCounter = 0;
    public object this[int index] { get => GetByIndex(index); set => UpdateAtIndex(index, value); }
    public object this[object key] { get => _items[(int)key]; set => _items[(int)key] = value; }
    public ICollection Keys => (ICollection)_items.Keys;
    public ICollection Values => (ICollection)_items.Values;
    public bool IsReadOnly => false;
    public bool IsFixedSize => false;

    public int Count => _items.Count;
    public object SyncRoot => this;
    public bool IsSynchronized => false;

    public void Add(object key, object value)
    {
        _items.TryAdd((int)key, value);
    }

    public void Clear()
    {
        _items.Clear();
    }

    public bool Contains(object key)
    {
        return _items.ContainsKey((int)key);
    }

    public IDictionaryEnumerator GetEnumerator()
    {
        return _items.GetEnumerator() as IDictionaryEnumerator;
    }

    public void Insert(int index, object key, object value)
    {
        if (_items.TryAdd((int)key, value))
            _orderCounter++;
    }

    public void Remove(object key)
    {
        _items.TryRemove((int)key, out _);
    }

    public void RemoveAt(int index)
    {
        int key = GetKeyByIndex(index);
        _items.TryRemove(key, out _);
    }

    private int GetKeyByIndex(int index)
    {
        int counter = 0;
        foreach (var key in _items.Keys)
        {
            if (counter == index) return key;
            counter++;
        }
        throw new IndexOutOfRangeException();
    }

    private object GetByIndex(int index)
    {
        int key = GetKeyByIndex(index);
        return _items[key];
    }

    private void UpdateAtIndex(int index, object value)
    {
        int key = GetKeyByIndex(index);
        _items[key] = value;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    public void CopyTo(Array array, int index)
    {
        throw new NotImplementedException();
    }
}

class Program
{
    static void Main()
    {
        var productCollection = new Product();
        productCollection.Add(1, "Ноутбук");
        productCollection.Add(2, "Телефон");
        productCollection.Add(3, "Планшет");

        Console.WriteLine("Коллекция:");
        foreach (var key in productCollection.Keys)
            Console.WriteLine($"Ключ: {key}, Значение: {productCollection[key]}");

        productCollection.Remove(2);
        Console.WriteLine("\nПосле удаления ключа 2:");
        foreach (var key in productCollection.Keys)
            Console.WriteLine($"Ключ: {key}, Значение: {productCollection[key]}");

        var intList = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        Console.WriteLine("\nИсходный список:");
        Console.WriteLine(string.Join(", ", intList));

        int n = 3;
        intList.RemoveRange(2, n);
        Console.WriteLine("\nПосле удаления 3-х элементов:");
        Console.WriteLine(string.Join(", ", intList));

        intList.Add(11);
        intList.Insert(2, 15);
        Console.WriteLine("\nПосле добавления элементов в список:");
        Console.WriteLine(string.Join(", ", intList));

        var intDictionary = new Dictionary<int, int>();
        for (int i = 0; i < intList.Count; i++)
        {
            intDictionary[i] = intList[i];
        }

        Console.WriteLine("\nОбобщённая коллекция:");
        foreach (var kvp in intDictionary)
            Console.WriteLine($"Ключ: {kvp.Key}, Значение: {kvp.Value}");

        int searchValue = 15;
        var found = intDictionary.ContainsValue(searchValue);
        Console.WriteLine($"\nЗначение: {searchValue} Найдено: {found}");

        var products = new ObservableCollection<ProductItem>
            {
                new ProductItem { Id = 1, Name = "Ноутбук" },
                new ProductItem { Id = 2, Name = "Телефон" }
            };
        products.CollectionChanged += Products_CollectionChanged;

        products.Add(new ProductItem { Id = 3, Name = "Планшет" });
        products.RemoveAt(0);

        products[0] = new ProductItem { Id = 4, Name = "Монитор" };
    }

    private static void Products_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                Console.WriteLine("Добавлено:");
                foreach (ProductItem newItem in e.NewItems)
                    Console.WriteLine(newItem);
                break;
            case NotifyCollectionChangedAction.Remove:
                Console.WriteLine("Удалено:");
                foreach (ProductItem oldItem in e.OldItems)
                    Console.WriteLine(oldItem);
                break;
            case NotifyCollectionChangedAction.Replace:
                Console.WriteLine("Замена:");
                foreach (ProductItem newItem in e.NewItems)
                    Console.WriteLine($"Новое: {newItem}");
                foreach (ProductItem oldItem in e.OldItems)
                    Console.WriteLine($"Старое: {oldItem}");
                break;
        }
    }
}

public class ProductItem
{
    public int Id { get; set; }
    public string Name { get; set; }

    public override string ToString() => $"ID: {Id}, Имя: {Name}";
}