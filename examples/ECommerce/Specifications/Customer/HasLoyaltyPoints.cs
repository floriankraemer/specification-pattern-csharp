using Phauthentic.Specification.Examples.ECommerce.Models;

namespace Phauthentic.Specification.Examples.ECommerce.Specifications.Customer;

/// <summary>
/// Specification for checking if an order's customer has sufficient loyalty points.
/// </summary>
public sealed class HasLoyaltyPoints(int minimumPoints) : Specification<Models.Order>
{
    public override bool IsSatisfiedBy(Models.Order candidate) => candidate.Customer.LoyaltyPoints >= minimumPoints;
}
