namespace GoodHamburger.Application.DTOs;

public record UpdateOrderRequest
{
    public List<int> Items { get; init; } = new();
}