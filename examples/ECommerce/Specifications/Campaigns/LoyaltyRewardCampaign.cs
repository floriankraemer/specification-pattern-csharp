using Phauthentic.Specification.Examples.ECommerce.Models;
using Phauthentic.Specification.Examples.ECommerce.Specifications.Customer;
using Phauthentic.Specification.Examples.ECommerce.Specifications.Order;

namespace Phauthentic.Specification.Examples.ECommerce.Specifications.Campaigns;

/// <summary>
/// Loyalty Reward Campaign Specification.
/// </summary>
/// <remarks>
/// Business rules:
/// <list type="bullet">
/// <item>Customer has 1000+ loyalty points</item>
/// <item>Not a new customer (account &gt; 30 days)</item>
/// <item>Order contains at least 3 items</item>
/// <item>Excludes clearance products</item>
/// </list>
/// </remarks>
public sealed class LoyaltyRewardCampaign : Specification<Models.Order>
{
    private readonly ISpecification<Models.Order> _specification;

    public LoyaltyRewardCampaign()
    {
        var loyaltySpec = new HasLoyaltyPoints(1000);
        var accountAgeSpec = new AccountAge(30, AccountAgeComparison.Minimum);
        var itemCountSpec = new MinimumItemCount(3);

        _specification = loyaltySpec
            .And(accountAgeSpec)
            .And(itemCountSpec);
    }

    public override bool IsSatisfiedBy(Models.Order candidate)
    {
        if (candidate.HasClearanceItems)
        {
            return false;
        }

        return _specification.IsSatisfiedBy(candidate);
    }
}
