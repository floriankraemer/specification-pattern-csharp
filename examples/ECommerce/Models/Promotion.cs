namespace Phauthentic.Specification.Examples.ECommerce.Models;

/// <summary>
/// Promotion entity representing a promotional campaign.
/// </summary>
public sealed record Promotion(
    string Id,
    string Name,
    string Description,
    int DiscountPercentage,
    ISpecification<Order> EligibilitySpecification)
{
    public bool IsEligible(Order candidate) => EligibilitySpecification.IsSatisfiedBy(candidate);

    public decimal CalculateDiscount(decimal originalAmount) => originalAmount * (DiscountPercentage / 100m);

    public decimal CalculateFinalPrice(decimal originalAmount) => originalAmount - CalculateDiscount(originalAmount);
}
