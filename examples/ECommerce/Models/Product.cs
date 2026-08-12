namespace Phauthentic.Specification.Examples.ECommerce.Models;

/// <summary>
/// Product entity representing a product in the e-commerce system.
/// </summary>
public sealed record Product(
    string Id,
    string Name,
    string Category, // electronics, fashion, home, books, etc.
    decimal Price,
    bool IsClearance,
    bool IsDigital,
    int StockQuantity)
{
    public bool IsInStock => StockQuantity > 0;

    public bool IsGiftCard =>
        Name.Contains("gift card", StringComparison.OrdinalIgnoreCase) ||
        Category.Contains("gift", StringComparison.OrdinalIgnoreCase);
}
