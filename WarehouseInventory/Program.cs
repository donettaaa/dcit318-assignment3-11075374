using System;
using System.Collections.Generic;
using System.Linq;

// a) Marker interface
public interface IInventoryItem
{
    int Id { get; }
    string Name { get; }
    int Quantity { get; set; }
}

// b) ElectronicItem
public class ElectronicItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public string Brand { get; }
    public int WarrantyMonths { get; }

    public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
        => (Id, Name, Quantity, Brand, WarrantyMonths) = (id, name, quantity, brand, warrantyMonths);

    public override string ToString() => $"[E#{Id}] {Name} ({Brand}), Qty={Quantity}, Warranty={WarrantyMonths}m";
}

// c) GroceryItem
public class GroceryItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; }

    public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
        => (Id, Name, Quantity, ExpiryDate) = (id, name, quantity, expiryDate);

    public override string ToString() => $"[G#{Id}] {Name}, Qty={Quantity}, Expires={ExpiryDate:d}";
}

// e) Custom exceptions
public class DuplicateItemException : Exception { public DuplicateItemException(string msg) : base(msg) { } }
public class ItemNotFoundException : Exception { public ItemNotFoundException(string msg) : base(msg) { } }
public class InvalidQuantityException : Exception { public InvalidQuantityException(string msg) : base(msg) { } }

// d) Generic inventory repo
public class InventoryRepository<T> where T : IInventoryItem
{
    private readonly Dictionary<int, T> _items = new();

    public void AddItem(T item)
    {
        if (_items.ContainsKey(item.Id))
            throw new DuplicateItemException($"Item with ID={item.Id} already exists.");
        _items[item.Id] = item;
    }

    public T GetItemById(int id)
    {
        if (!_items.TryGetValue(id, out var item))
            throw new ItemNotFoundException($"Item with ID={id} not found.");
        return item;
    }

    public void RemoveItem(int id)
    {
        if (!_items.Remove(id))
            throw new ItemNotFoundException($"Item with ID={id} not found.");
    }

    public List<T> GetAllItems() => _items.Values.ToList();

    public void UpdateQuantity(int id, int newQuantity)
    {
        if (newQuantity < 0) throw new InvalidQuantityException("Quantity cannot be negative.");
        var item = GetItemById(id);
        item.Quantity = newQuantity;
    }
}

// f) Manager
public class WareHouseManager
{
    private readonly InventoryRepository<ElectronicItem> _electronics = new();
    private readonly InventoryRepository<GroceryItem> _groceries = new();

    public void SeedData()
    {
        _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 24));
        _electronics.AddItem(new ElectronicItem(2, "Headphones", 25, "Sony", 12));
        _groceries.AddItem(new GroceryItem(101, "Rice 5kg", 50, DateTime.Today.AddMonths(12)));
        _groceries.AddItem(new GroceryItem(102, "Milk 1L", 30, DateTime.Today.AddDays(10)));
    }

    public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
    {
        foreach (var item in repo.GetAllItems())
            Console.WriteLine(item);
    }

    public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
    {
        try
        {
            var current = repo.GetItemById(id);
            repo.UpdateQuantity(id, current.Quantity + quantity);
            Console.WriteLine($"Increased stock for #{id} by {quantity}.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[IncreaseStock] {ex.Message}");
        }
    }

    public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
    {
        try
        {
            repo.RemoveItem(id);
            Console.WriteLine($"Removed item #{id}.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[RemoveItem] {ex.Message}");
        }
    }

    public void DemoExceptions()
    {
        try { _electronics.AddItem(new ElectronicItem(1, "Phone", 5, "Apple", 12)); }
        catch (Exception ex) { Console.WriteLine($"[Add duplicate] {ex.Message}"); }

        RemoveItemById(_groceries, 999);
        try { _electronics.UpdateQuantity(2, -5); }
        catch (Exception ex) { Console.WriteLine($"[Invalid qty] {ex.Message}"); }
    }

    public void Run()
    {
        SeedData();

        Console.WriteLine("Groceries:");
        PrintAllItems(_groceries);

        Console.WriteLine("\nElectronics:");
        PrintAllItems(_electronics);

        Console.WriteLine("\nDemoing error scenarios:");
        DemoExceptions();
    }
}

public class Program
{
    public static void Main() => new WareHouseManager().Run();
}

