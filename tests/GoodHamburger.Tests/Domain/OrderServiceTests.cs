namespace GoodHamburger.Tests.Domain;

using Xunit;
using FluentAssertions;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Services;

public class OrderServiceTests
{
    private readonly OrderService _sut; // System Under Test

    public OrderServiceTests()
    {
        _sut = new OrderService();
    }

    [Fact]
    public void ValidateOrder_WhenOrderIsEmpty_ShouldThrowArgumentException()
    {
        // Arrange
        var items = new List<MenuItem>();

        // Act
        Action act = () => OrderService.ValidateOrder(items);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Order must contain at least one item");
    }

    [Fact]
    public void ValidateOrder_WhenOrderIsNull_ShouldThrowArgumentException()
    {
        // Arrange
        List<MenuItem> items = null!;

        // Act
        Action act = () => OrderService.ValidateOrder(items);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Order must contain at least one item");
    }

    [Fact]
    public void ValidateOrder_WhenNoSandwich_ShouldThrowArgumentException()
    {
        // Arrange
        var items = new List<MenuItem>
        {
            new() { Id = 4, Name = "Fries", Price = 2.00m, Type = "extra" }
        };

        // Act
        Action act = () => OrderService.ValidateOrder(items);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Order must contain at least one sandwich");
    }

    [Fact]
    public void ValidateOrder_WhenDuplicateItems_ShouldThrowArgumentException()
    {
        // Arrange
        var sandwich = new MenuItem { Id = 1, Name = "Sandwich", Price = 5.00m, Type = "sandwich" };
        var items = new List<MenuItem> { sandwich, sandwich };

        // Act
        Action act = () => OrderService.ValidateOrder(items);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Order can contain only one sandwich");
    }

    [Fact]
    public void ValidateOrder_WhenMultipleSandwiches_ShouldThrowArgumentException()
    {
        // Arrange
        var items = new List<MenuItem>
        {
            new() { Id = 1, Name = "Sandwich", Price = 5.00m, Type = "sandwich" },
            new() { Id = 2, Name = "Egg", Price = 4.50m, Type = "sandwich" }
        };

        // Act
        Action act = () => OrderService.ValidateOrder(items);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Order can contain only one sandwich");
    }

    [Fact]
    public void ValidateOrder_WhenValidOrder_ShouldNotThrowException()
    {
        // Arrange
        var items = new List<MenuItem>
        {
            new() { Id = 1, Name = "Sandwich", Price = 5.00m, Type = "sandwich" },
            new() { Id = 4, Name = "Fries", Price = 2.00m, Type = "extra" }
        };

        // Act
        Action act = () => OrderService.ValidateOrder(items);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void CalculateDiscount_WhenSandwichFriesAndDrink_ShouldApply20PercentDiscount()
    {
        // Arrange
        var order = new Order
        {
            Items = new List<OrderItem>
            {
                new() { Name = "Sandwich", Price = 5.00m, Type = "sandwich" },
                new() { Name = "Fries", Price = 2.00m, Type = "extra" },
                new() { Name = "Soft drink", Price = 2.50m, Type = "extra" }
            }
        };
        order.CalculateTotal();

        // Act
        var (percentage, amount) = OrderService.CalculateDiscount(order);

        // Assert
        percentage.Should().Be(20);
        amount.Should().Be(1.90m);
    }

    [Fact]
    public void CalculateDiscount_WhenSandwichAndDrink_ShouldApply15PercentDiscount()
    {
        // Arrange
        var order = new Order
        {
            Items = new List<OrderItem>
            {
                new() { Name = "Sandwich", Price = 5.00m, Type = "sandwich" },
                new() { Name = "Soft drink", Price = 2.50m, Type = "extra" }
            }
        };
        order.CalculateTotal();

        // Act
        var (percentage, amount) = OrderService.CalculateDiscount(order);

        // Assert
        percentage.Should().Be(15);
        amount.Should().Be(1.125m);
    }

    [Fact]
    public void CalculateDiscount_WhenSandwichAndFries_ShouldApply10PercentDiscount()
    {
        // Arrange
        var order = new Order
        {
            Items = new List<OrderItem>
            {
                new() { Name = "Sandwich", Price = 5.00m, Type = "sandwich" },
                new() { Name = "Fries", Price = 2.00m, Type = "extra" }
            }
        };
        order.CalculateTotal();

        // Act
        var (percentage, amount) = OrderService.CalculateDiscount(order);

        // Assert
        percentage.Should().Be(10);
        amount.Should().Be(0.70m);
    }

    [Fact]
    public void CalculateDiscount_WhenOnlySandwich_ShouldApplyNoDiscount()
    {
        // Arrange
        var order = new Order
        {
            Items = new List<OrderItem>
            {
                new() { Name = "Sandwich", Price = 5.00m, Type = "sandwich" }
            }
        };
        order.CalculateTotal();

        // Act
        var (percentage, amount) = OrderService.CalculateDiscount(order);

        // Assert
        percentage.Should().Be(0);
        amount.Should().Be(0);
    }

    [Fact]
    public void CalculateOrderTotal_WhenValidOrder_ShouldCalculateCorrectTotals()
    {
        // Arrange
        var order = new Order
        {
            Items = new List<OrderItem>
            {
                new() { Name = "Sandwich", Price = 5.00m, Type = "sandwich" },
                new() { Name = "Fries", Price = 2.00m, Type = "extra" },
                new() { Name = "Soft drink", Price = 2.50m, Type = "extra" }
            }
        };

        // Act
        OrderService.CalculateOrderTotal(order);

        // Assert
        order.Total.Should().Be(9.50m);
        order.Discount.Should().Be(1.90m);
        order.FinalTotal.Should().Be(7.60m);
    }

    [Theory]
    [InlineData(5.00, 2.00, 2.50, 20, 1.90, 7.60)] // Sandwich + Fries + Drink
    [InlineData(5.00, 0, 2.50, 15, 1.125, 6.375)]   // Sandwich + Drink
    [InlineData(5.00, 2.00, 0, 10, 0.70, 6.30)]     // Sandwich + Fries
    [InlineData(5.00, 0, 0, 0, 0, 5.00)]            // Only Sandwich
    public void CalculateOrderTotal_WithDifferentCombinations_ShouldCalculateCorrectly(
        decimal sandwichPrice,
        decimal friesPrice,
        decimal drinkPrice,
        int expectedDiscountPercentage,
        decimal expectedDiscount,
        decimal expectedFinalTotal)
    {
        // Arrange
        var order = new Order();

        order.Items.Add(new OrderItem
        {
            Name = "Sandwich",
            Price = sandwichPrice,
            Type = "sandwich"
        });

        if (friesPrice > 0)
            order.Items.Add(new OrderItem
            {
                Name = "Fries",
                Price = friesPrice,
                Type = "extra"
            });

        if (drinkPrice > 0)
            order.Items.Add(new OrderItem
            {
                Name = "Soft drink",
                Price = drinkPrice,
                Type = "extra"
            });

        // Act
        OrderService.CalculateOrderTotal(order);

        // Assert
        order.Discount.Should().Be(expectedDiscount);
        order.FinalTotal.Should().Be(expectedFinalTotal);
    }
}