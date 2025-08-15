using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

// b) Marker interface
public interface IInventoryEntity { int Id { get; } }

// a) Record
public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

// c) Generic InventoryLogger<T>
public class InventoryLogger<T> where T : IInventoryEntity
{
    private readonly List<T> _log = new();
    private readonly string _filePath;

    public InventoryLogger(string filePath) => _filePath = filePath;

    public void Add(T item) => _log.Add(item);
    public List<T> GetAll() => new(_log);

    public void SaveToFile()
    {
        try
        {
            var json = JsonSerializer.Serialize(_log, new JsonSerializerOptions { WriteIndented = true });
            using var writer = new StreamWriter(_filePath);
            writer.Write(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SaveToFile] {ex.Message}");
        }
    }

    public void LoadFromFile()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine("[LoadFromFile] No file yet.");
                return;
            }
            using var reader = new StreamReader(_filePath);
            var json = reader.ReadToEnd();
            var data = JsonSerializer.Deserialize<List<T>>(json);
            _log.Clear();
            if (data != null) _log.AddRange(data);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LoadFromFile] {ex.Message}");
        }
    }
}

// f) Integration layer
public class InventoryApp
{
    private InventoryLogger<InventoryItem> _logger = new("inventory.json");

    public void SeedSampleData()
    {
        _logger.Add(new InventoryItem(1, "Paper Reams", 20, DateTime.Today));
        _logger.Add(new InventoryItem(2, "Ink Cartridges", 8, DateTime.Today.AddDays(-1)));
        _logger.Add(new InventoryItem(3, "Staplers", 15, DateTime.Today.AddDays(-2)));
        _logger.Add(new InventoryItem(4, "Pens", 50, DateTime.Today));
    }

    public void SaveData() => _logger.SaveToFile();
    public void LoadData() => _logger.LoadFromFile();

    public void PrintAllItems()
    {
        foreach (var item in _logger.GetAll())
            Console.WriteLine($"#{item.Id} {item.Name} Qty={item.Quantity} Added={item.DateAdded:d}");
    }

    public void Run()
    {
        // First session
        SeedSampleData();
        SaveData();

        // "New session": clear memory by re-instantiating
        _logger = new InventoryLogger<InventoryItem>("inventory.json");
        LoadData();
        PrintAllItems();
    }
}

public class Program
{
    public static void Main() => new InventoryApp().Run();
}

