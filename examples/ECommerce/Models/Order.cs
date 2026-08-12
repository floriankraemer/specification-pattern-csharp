namespace Phauthentic.Specification.Examples.ECommerce.Models;

/// <summary>
/// Order entity representing an order in the e-commerce system.
/// </summary>
public sealed record Order(
    string Id,
    Customer Customer,
    IReadOnlyList<Product> Items,
    decimal TotalAmount,
    int ItemCount,
    DateTimeOffset CreatedAt,
    bool IsFirstOrder)
{
    /// <summary>
    /// Category counts across all items in the order.
    /// </summary>
    public IReadOnlyDictionary<string, int> GetCategoryCounts() =>
        Items
            .GroupBy(product => product.Category)
            .ToDictionary(group => group.Key, group => group.Count());

    public bool ContainsCategory(string category) =>
        Items.Any(product => product.Category == category);

    public bool HasClearanceItems => Items.Any(product => product.IsClearance);

    public bool HasDigitalItems => Items.Any(product => product.IsDigital);

    public bool HasGiftCards => Items.Any(product => product.IsGiftCard);
}
