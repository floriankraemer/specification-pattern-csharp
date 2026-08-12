using Phauthentic.Specification.Examples.ECommerce.Models;

namespace Phauthentic.Specification.Examples.ECommerce.Specifications.Customer;

/// <summary>
/// Specification for checking if an order's customer hasn't used a flash sale recently.
/// </summary>
public sealed class HasNotUsedFlashSaleRecently(int cooldownDays = 7) : Specification<Models.Order>
{
    public override bool IsSatisfiedBy(Models.Order candidate)
    {
        var daysSinceLastFlashSale = candidate.Customer.DaysSinceLastFlashSale();

        // If never used a flash sale, it's okay.
        return daysSinceLastFlashSale is null || daysSinceLastFlashSale >= cooldownDays;
    }
}
