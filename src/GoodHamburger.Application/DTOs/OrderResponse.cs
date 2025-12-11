namespace GoodHamburger.Application.DTOs;

public record OrderResponse
{
    public int Id { get; init; }
    public List<OrderItem> Items { get; init; } = [];
    public decimal Total { get; init; }
    public decimal Discount { get; init; }
    public decimal FinalTotal { get; init; }
    public DateTime CreatedAt { get; init; }
}