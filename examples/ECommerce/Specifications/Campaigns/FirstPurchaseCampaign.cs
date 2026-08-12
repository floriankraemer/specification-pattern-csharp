using Phauthentic.Specification.Examples.ECommerce.Models;
using Phauthentic.Specification.Examples.ECommerce.Specifications.Customer;
using Phauthentic.Specification.Examples.ECommerce.Specifications.Order;

namespace Phauthentic.Specification.Examples.ECommerce.Specifications.Campaigns;

/// <summary>
/// First Purchase Discount Campaign Specification.
/// </summary>
/// <remarks>
/// Business rules:
/// <list type="bullet">
/// <item>New customer (account &lt; 30 days)</item>
/// <item>First order ever</item>
/// <item>Minimum order value $50</item>
/// <item>Excludes gift cards and digital products</item>
/// </list>
/// </remarks>
public sealed class FirstPurchaseCampaign : Specification<Models.Order>
{
    private readonly ISpecification<Models.Order> _specification;

    public FirstPurchaseCampaign()
    {
        var newCustomerSpec = new IsNewCustomer(30);
        var firstOrderSpec = new IsFirstOrder();
        var minValueSpec = new MinimumOrderValue(50m);

        _specification = newCustomerSpec
            .And(firstOrderSpec)
            .And(minValueSpec);
    }

    public override bool IsSatisfiedBy(Models.Order candidate)
    {
        // Excludes digital products and gift cards (checked at Order level).
        if (candidate.HasDigitalItems || candidate.HasGiftCards)
        {
            return false;
        }

        return _specification.IsSatisfiedBy(candidate);
    }
}
