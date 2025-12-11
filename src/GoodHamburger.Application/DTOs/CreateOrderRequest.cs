namespace GoodHamburger.Application.DTOs;

public record CreateOrderRequest
{
    public List<int> Items { get; init; } = new();
}