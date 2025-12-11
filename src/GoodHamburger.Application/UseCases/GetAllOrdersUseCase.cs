namespace GoodHamburger.Application.UseCases;

using GoodHamburger.Domain.Repositories;
using GoodHamburger.Application.DTOs;

public class GetAllOrdersUseCase(IOrderRepository orderRepository)
{
    private readonly IOrderRepository _orderRepository = orderRepository;

    public async Task<List<OrderResponse>> ExecuteAsync()
    {
        var orders = await _orderRepository.GetAllAsync();

        return orders.Select(order => new OrderResponse
        {
            Id = order.Id,
            Items = order.Items.Select(i => new OrderItem
            {
                Id = i.Id,
                Name = i.Name,
                Price = i.Price,
                Type = i.Type
            }).ToList(),
            Total = order.Total,
            Discount = order.Discount,
            FinalTotal = order.FinalTotal,
            CreatedAt = order.CreatedAt
        }).ToList();
    }
}