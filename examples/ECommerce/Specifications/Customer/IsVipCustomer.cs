using Phauthentic.Specification.Examples.ECommerce.Models;

namespace Phauthentic.Specification.Examples.ECommerce.Specifications.Customer;

/// <summary>
/// Specification for checking if an order's customer has VIP tier status.
/// </summary>
public sealed class IsVipCustomer : Specification<Models.Order>
{
    private static readonly string[] DefaultTiers = ["gold", "platinum"];

    private readonly IReadOnlyList<string> _vipTiers;

    public IsVipCustomer(IReadOnlyList<string>? vipTiers = null)
    {
        _vipTiers = vipTiers ?? DefaultTiers;
    }

    public override bool IsSatisfiedBy(Models.Order candidate) => _vipTiers.Contains(candidate.Customer.Tier);
}
