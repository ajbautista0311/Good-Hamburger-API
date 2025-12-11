namespace GoodHamburger.Tests.Application;

using Xunit;
using Moq;
using FluentAssertions;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Repositories;
using GoodHamburger.Domain.Services;
using GoodHamburger.Application.UseCases;
using GoodHamburger.Application.DTOs;

public class CreateOrderUseCaseTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IMenuRepository> _menuRepositoryMock;
    private readonly Mock<OrderService> _orderServiceMock;
    private readonly CreateOrderUseCase _sut;

    public CreateOrderUseCaseTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _menuRepositoryMock = new Mock<IMenuRepository>();
        _orderServiceMock = new Mock<OrderService>();

        _sut = new CreateOrderUseCase(
            _orderRepositoryMock.Object,
            _menuRepositoryMock.Object,
            _orderServiceMock.Object
        );
    }

    [Fact]
    public async Task ExecuteAsync_WhenValidRequest_ShouldCreateOrderSuccessfully()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            Items = new List<int> { 1, 4, 5 } // Sandwich, Fries, Drink
        };

        var menuItems = new List<MenuItem>
        {
            new() { Id = 1, Name = "Sandwich", Price = 5.00m, Type = "sandwich" },
            new() { Id = 4, Name = "Fries", Price = 2.00m, Type = "extra" },
            new() { Id = 5, Name = "Soft drink", Price = 2.50m, Type = "extra" }
        };

        _menuRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(menuItems[0]);
        _menuRepositoryMock.Setup(x => x.GetByIdAsync(4))
            .ReturnsAsync(menuItems[1]);
        _menuRepositoryMock.Setup(x => x.GetByIdAsync(5))
            .ReturnsAsync(menuItems[2]);

        var savedOrder = new Order
        {
            Id = 1,
            Total = 9.50m,
            Discount = 1.90m,
            FinalTotal = 7.60m
        };

        _orderRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<Order>()))
            .ReturnsAsync(savedOrder);

        // Act
        var result = await _sut.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Total.Should().Be(9.50m);
        result.Discount.Should().Be(1.90m);
        result.FinalTotal.Should().Be(7.60m);

        _menuRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Exactly(3));
        _orderRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Order>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenInvalidItemId_ShouldThrowArgumentException()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            Items = new List<int> { 1, 999 } // 999 no existe
        };

        _menuRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new MenuItem { Id = 1, Name = "Sandwich", Price = 5.00m, Type = "sandwich" });
        _menuRepositoryMock.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((MenuItem?)null);

        // Act
        Func<Task> act = async () => await _sut.ExecuteAsync(request);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("One or more invalid item IDs");
    }
}