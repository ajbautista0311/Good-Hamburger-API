namespace GoodHamburger.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using GoodHamburger.Application.DTOs;
using GoodHamburger.Application.UseCases;

[ApiController]
[Route("api/[controller]")]
[Tags("Orders")]
public class OrdersController(
    CreateOrderUseCase createOrderUseCase,
    GetAllOrdersUseCase getAllOrdersUseCase,
    UpdateOrderUseCase updateOrderUseCase,
    DeleteOrderUseCase deleteOrderUseCase) : ControllerBase
{
    private readonly CreateOrderUseCase _createOrderUseCase = createOrderUseCase;
    private readonly GetAllOrdersUseCase _getAllOrdersUseCase = getAllOrdersUseCase;
    private readonly UpdateOrderUseCase _updateOrderUseCase = updateOrderUseCase;
    private readonly DeleteOrderUseCase _deleteOrderUseCase = deleteOrderUseCase;

    /// <summary>Create a new order</summary>
    /// <response code="201">Order created successfully</response>
    /// <response code="400">Validation error</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        try
        {
            var order = await _createOrderUseCase.ExecuteAsync(request);
            return CreatedAtAction(
                nameof(CreateOrder),
                new { id = order.Id },
                new { success = true, message = "Order created successfully", data = order });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
        
    }

    /// <summary>Get all orders</summary>
    [HttpGet]
    public async Task<IActionResult> GetAllOrders()
    {
        var orders = await _getAllOrdersUseCase.ExecuteAsync();
        
        if (orders is null || orders.Count is 0)
        {
            return Ok(new { success = false, message = "No orders found." });
        }
        
        return Ok(new { success = true, data = orders });
    }

    /// <summary>Actualizar una orden existente</summary>
    /// <response code="200">Order updated successfully</response>
    /// <response code="404">Order not found</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateOrder(int id, [FromBody] UpdateOrderRequest request)
    {
        try
        {
            var order = await _updateOrderUseCase.ExecuteAsync(id, request);
            return Ok(new { success = true, message = "Order updated successfully", data = order });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { success = false, error = ex.Message });
        }
    }

    /// <summary>Delete an order</summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        try
        {
            await _deleteOrderUseCase.ExecuteAsync(id);
            return Ok(new { success = true, message = "Order deleted successfully" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { success = false, error = ex.Message });
        }
    }
}