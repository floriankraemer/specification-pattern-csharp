namespace Phauthentic.Specification.Examples.ECommerce.Specifications.Order;

/// <summary>
/// Specification for checking if an order meets a minimum value requirement.
/// </summary>
public sealed class MinimumOrderValue(decimal minimumValue) : Specification<Models.Order>
{
    public override bool IsSatisfiedBy(Models.Order candidate) => candidate.TotalAmount >= minimumValue;
}
