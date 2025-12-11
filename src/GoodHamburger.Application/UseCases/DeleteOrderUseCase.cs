namespace GoodHamburger.Application.UseCases;

using GoodHamburger.Domain.Repositories;

public class DeleteOrderUseCase(IOrderRepository orderRepository)
{
    private readonly IOrderRepository _orderRepository = orderRepository;

    public async Task ExecuteAsync(int orderId)
    {
        var deleted = await _orderRepository.DeleteAsync(orderId);
        if (!deleted)
            throw new KeyNotFoundException("Order not found");
    }
}