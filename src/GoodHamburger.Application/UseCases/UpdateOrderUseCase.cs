namespace GoodHamburger.Application.UseCases;

using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Repositories;
using GoodHamburger.Domain.Services;
using GoodHamburger.Application.DTOs;
using OrderItem = DTOs.OrderItem;

public class UpdateOrderUseCase(
    IOrderRepository orderRepository,
    IMenuRepository menuRepository,
    OrderService orderService)
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IMenuRepository _menuRepository = menuRepository;
    private readonly OrderService _orderService = orderService;

    public async Task<OrderResponse> ExecuteAsync(int orderId, UpdateOrderRequest request)
    {
        var existingOrder = await _orderRepository.GetByIdAsync(orderId);
        if (existingOrder is null)
            throw new KeyNotFoundException("Order not found");

        var items = new List<MenuItem>();

        foreach (var id in request.Items)
        {
            var item = await _menuRepository.GetByIdAsync(id);
            if (item is not null)
            {
                items.Add(item);
            }
        }

        if (items.Count != request.Items.Count)
            throw new ArgumentException("One or more invalid item IDs");

        OrderService.ValidateOrder(items);

        var order = new Order { Id = orderId };
        items.ForEach(item => order.AddItem(item));

        OrderService.CalculateOrderTotal(order);

        var updatedOrder = await _orderRepository.UpdateAsync(orderId, order);

        return new OrderResponse
        {
            Id = updatedOrder!.Id,
            Items = updatedOrder.Items.Select(i => new OrderItem
            {
                Id = i.Id,
                Name = i.Name,
                Price = i.Price,
                Type = i.Type
            }).ToList(),
            Total = updatedOrder.Total,
            Discount = updatedOrder.Discount,
            FinalTotal = updatedOrder.FinalTotal,
            CreatedAt = updatedOrder.CreatedAt
        };
    }
}