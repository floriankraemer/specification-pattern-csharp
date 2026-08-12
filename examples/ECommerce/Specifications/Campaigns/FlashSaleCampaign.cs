using Phauthentic.Specification.Examples.ECommerce.Models;
using Phauthentic.Specification.Examples.ECommerce.Specifications.Customer;
using Phauthentic.Specification.Examples.ECommerce.Specifications.Order;
using Phauthentic.Specification.Examples.ECommerce.Specifications.Time;

namespace Phauthentic.Specification.Examples.ECommerce.Specifications.Campaigns;

/// <summary>
/// Flash Sale Campaign Specification.
/// </summary>
/// <remarks>
/// Business rules:
/// <list type="bullet">
/// <item>Time-sensitive (valid for specific hours, e.g., 12:00-14:00)</item>
/// <item>Limited to specific product categories (electronics, fashion)</item>
/// <item>Customer must not have used a flash sale in the last 7 days</item>
/// </list>
/// </remarks>
public sealed class FlashSaleCampaign : Specification<Models.Order>
{
    private static readonly TimeRange TimeRangeSpec = new(new TimeOnly(12, 0), new TimeOnly(14, 0));

    private readonly ISpecification<Models.Order> _orderSpecification;

    public FlashSaleCampaign()
    {
        var flashSaleCooldownSpec = new HasNotUsedFlashSaleRecently(7);
        var categorySpec = new ContainsProductCategory(["electronics", "fashion"]);

        _orderSpecification = flashSaleCooldownSpec.And(categorySpec);
    }

    public override bool IsSatisfiedBy(Models.Order candidate) =>
        _orderSpecification.IsSatisfiedBy(candidate) && TimeRangeSpec.IsSatisfiedBy(candidate.CreatedAt);
}
