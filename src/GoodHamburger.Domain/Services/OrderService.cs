namespace GoodHamburger.Domain.Services;

using GoodHamburger.Domain.Entities;

public class OrderService
{
    public static void ValidateOrder(List<MenuItem> items)
    {
        if (items is null || items.Count is 0)
            throw new ArgumentException("Order must contain at least one item");

        var sandwiches = items.Where(item => item.Type is "sandwich").ToList();

        if (sandwiches.Count is 0)
            throw new ArgumentException("Order must contain at least one sandwich");

        if (sandwiches.Count > 1)
            throw new ArgumentException("Order can contain only one sandwich");

        var duplicates = items
            .GroupBy(x => x.Id)
            .Where(g => g.Count() > 1)
            .Select(g => g.First().Name)
            .ToList();

        if (duplicates.Count is not 0)
            throw new ArgumentException(
                $"Duplicate item detected: {duplicates.First()}. " +
                "Each item can only be added once.");
    }

    public static (int Percentage, decimal Amount) CalculateDiscount(Order order)
    {
        var hasSandwich = order.HasSandwich();
        var hasFries = order.HasFries();
        var hasSoftDrink = order.HasSoftDrink();

        int discountPercentage = (hasSandwich, hasFries, hasSoftDrink) switch
        {
            (true, true, true) => 20,
            (true, false, true) => 15,
            (true, true, false) => 10,
            _ => 0
        };

        var discountAmount = order.Total * discountPercentage / 100;
        return (discountPercentage, discountAmount);
    }

    public static void CalculateOrderTotal(Order order)
    {
        order.CalculateTotal();
        var (_, amount) = CalculateDiscount(order);
        order.Discount = amount;
        order.FinalTotal = order.Total - order.Discount;
    }
}