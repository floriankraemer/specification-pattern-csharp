namespace Phauthentic.Specification.Examples.ECommerce.Specifications.Order;

/// <summary>
/// Specification for checking if an order has a minimum number of items.
/// </summary>
public sealed class MinimumItemCount(int minimumCount) : Specification<Models.Order>
{
    public override bool IsSatisfiedBy(Models.Order candidate) => candidate.ItemCount >= minimumCount;
}
