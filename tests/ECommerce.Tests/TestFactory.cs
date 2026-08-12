using Phauthentic.Specification.Examples.ECommerce.Models;

namespace Phauthentic.Specification.Examples.ECommerce.Tests;

/// <summary>
/// Builds minimal, valid domain objects for specification tests, with sensible defaults
/// that can be overridden per test.
/// </summary>
internal static class TestFactory
{
    public static Customer CreateCustomer(
        string tier = "bronze",
        int loyaltyPoints = 0,
        DateTimeOffset? accountCreatedAt = null,
        DateTimeOffset? lastFlashSaleUsed = null) =>
        new(
            Id: "customer-1",
            Name: "Test Customer",
            Email: "customer@example.com",
            Tier: tier,
            LoyaltyPoints: loyaltyPoints,
            AccountCreatedAt: accountCreatedAt ?? DateTimeOffset.UtcNow.AddYears(-1),
            LastFlashSaleUsed: lastFlashSaleUsed,
            IsNewsletterSubscriber: false);

    public static Product CreateProduct(
        string category = "electronics",
        decimal price = 10m,
        bool isClearance = false,
        bool isDigital = false,
        string name = "Test Product",
        int stockQuantity = 10) =>
        new(
            Id: "product-1",
            Name: name,
            Category: category,
            Price: price,
            IsClearance: isClearance,
            IsDigital: isDigital,
            StockQuantity: stockQuantity);

    public static Order CreateOrder(
        Customer? customer = null,
        IReadOnlyList<Product>? items = null,
        decimal totalAmount = 100m,
        int itemCount = 1,
        DateTimeOffset? createdAt = null,
        bool isFirstOrder = false) =>
        new(
            Id: "order-1",
            Customer: customer ?? CreateCustomer(),
            Items: items ?? [CreateProduct()],
            TotalAmount: totalAmount,
            ItemCount: itemCount,
            CreatedAt: createdAt ?? DateTimeOffset.UtcNow,
            IsFirstOrder: isFirstOrder);
}
