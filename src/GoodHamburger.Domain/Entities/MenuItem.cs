namespace GoodHamburger.Domain.Entities;

public class MenuItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Type { get; set; } = string.Empty;

    public static List<MenuItem> GetInitialSandwiches() => new()
    {
        new() { Id = 1, Name = "Sandwich", Price = 5.00m, Type = "sandwich" },
        new() { Id = 2, Name = "Egg", Price = 4.50m, Type = "sandwich" },
        new() { Id = 3, Name = "Bacon", Price = 7.00m, Type = "sandwich" }
    };

    public static List<MenuItem> GetInitialExtras() => new()
    {
        new() { Id = 4, Name = "Fries", Price = 2.00m, Type = "extra" },
        new() { Id = 5, Name = "Soft drink", Price = 2.50m, Type = "extra" }
    };

    public static List<MenuItem> GetAllInitialItems()
        => GetInitialSandwiches().Concat(GetInitialExtras()).ToList();
}