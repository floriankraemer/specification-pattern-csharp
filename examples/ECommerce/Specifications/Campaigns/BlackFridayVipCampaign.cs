using Phauthentic.Specification.Examples.ECommerce.Models;
using Phauthentic.Specification.Examples.ECommerce.Specifications.Customer;
using Phauthentic.Specification.Examples.ECommerce.Specifications.Order;
using Phauthentic.Specification.Examples.ECommerce.Specifications.Time;

namespace Phauthentic.Specification.Examples.ECommerce.Specifications.Campaigns;

/// <summary>
/// Black Friday VIP Campaign Specification.
/// </summary>
/// <remarks>
/// Business rules:
/// <list type="bullet">
/// <item>Customer must be VIP (gold/platinum tier)</item>
/// <item>Order total must be at least $100</item>
/// <item>Must include products from electronics OR fashion categories</item>
/// <item>Customer account must be older than 6 months</item>
/// <item>Promotion valid only during November 20-30</item>
/// </list>
/// </remarks>
public sealed class BlackFridayVipCampaign : Specification<Models.Order>
{
    private static readonly DateRange DateRangeSpec = new(
        new DateOnly(2025, 11, 20), new DateOnly(2025, 11, 30));

    private readonly ISpecification<Models.Order> _orderSpecification;

    public BlackFridayVipCampaign()
    {
        var vipSpec = new IsVipCustomer(["gold", "platinum"]);
        var minValueSpec = new MinimumOrderValue(100m);
        var categorySpec = new ContainsProductCategory("electronics")
            .Or(new ContainsProductCategory("fashion"));
        var accountAgeSpec = new AccountAge(180, AccountAgeComparison.Minimum);

        _orderSpecification = vipSpec
            .And(minValueSpec)
            .And(categorySpec)
            .And(accountAgeSpec);
    }

    public override bool IsSatisfiedBy(Models.Order candidate) =>
        _orderSpecification.IsSatisfiedBy(candidate) && DateRangeSpec.IsSatisfiedBy(candidate.CreatedAt);
}
