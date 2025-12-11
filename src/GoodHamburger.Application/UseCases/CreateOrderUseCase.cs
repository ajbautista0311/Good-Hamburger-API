namespace GoodHamburger.Application.UseCases;

using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Repositories;
using GoodHamburger.Domain.Services;
using GoodHamburger.Application.DTOs;
using OrderItem = DTOs.OrderItem;

public class CreateOrderUseCase(
    IOrderRepository orderRepository,
    IMenuRepository menuRepository,
    OrderService orderService)
{
	private readonly IOrderRepository _orderRepository = orderRepository;
	private readonly IMenuRepository _menuRepository = menuRepository;
	private readonly OrderService _orderService = orderService;

    public async Task<OrderResponse> ExecuteAsync(CreateOrderRequest request)
	{
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

		var order = new Order();
		items.ForEach(item => order.AddItem(item));

        OrderService.CalculateOrderTotal(order);

		var savedOrder = await _orderRepository.CreateAsync(order);

		return MapToResponse(savedOrder);
	}

	private static OrderResponse MapToResponse(Order order)
	{
		return new OrderResponse
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
		};
	}
}