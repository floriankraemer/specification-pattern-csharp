namespace Phauthentic.Specification.Examples.ECommerce.Models;

/// <summary>
/// Customer entity representing a customer in the e-commerce system.
/// </summary>
public sealed record Customer(
    string Id,
    string Name,
    string Email,
    string Tier, // bronze, silver, gold, platinum
    int LoyaltyPoints,
    DateTimeOffset AccountCreatedAt,
    DateTimeOffset? LastFlashSaleUsed,
    bool IsNewsletterSubscriber)
{
    public int GetAccountAgeInDays() => (int)(DateTimeOffset.UtcNow - AccountCreatedAt).TotalDays;

    public int? DaysSinceLastFlashSale() =>
        LastFlashSaleUsed is null ? null : (int)(DateTimeOffset.UtcNow - LastFlashSaleUsed.Value).TotalDays;
}
