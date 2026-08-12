namespace Phauthentic.Specification.Examples.ECommerce.Specifications.Order;

/// <summary>
/// Specification for checking if this is the customer's first order.
/// </summary>
public sealed class IsFirstOrder : Specification<Models.Order>
{
    public override bool IsSatisfiedBy(Models.Order candidate) => candidate.IsFirstOrder;
}
