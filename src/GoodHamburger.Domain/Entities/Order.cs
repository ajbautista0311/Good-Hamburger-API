namespace GoodHamburger.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public decimal Total { get; set; }
    public decimal Discount { get; set; }
    public decimal FinalTotal { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public void AddItem(MenuItem menuItem)
    {
        Items.Add(new OrderItem
        {
            MenuItemId = menuItem.Id,
            Name = menuItem.Name,
            Price = menuItem.Price,
            Type = menuItem.Type
        });
    }

    public List<OrderItem> GetItemsByType(string type)
        => Items.Where(item => item.Type == type).ToList();

    public bool HasSandwich()
        => GetItemsByType("sandwich").Any();

    public bool HasFries()
        => Items.Any(item => item.Name is "Fries");

    public bool HasSoftDrink()
        => Items.Any(item => item.Name is "Soft drink");

    public void CalculateTotal()
        => Total = Items.Sum(item => item.Price);
}