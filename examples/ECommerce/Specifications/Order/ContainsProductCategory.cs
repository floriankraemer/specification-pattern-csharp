namespace Phauthentic.Specification.Examples.ECommerce.Specifications.Order;

/// <summary>
/// Specification for checking if an order contains products from specific categories.
/// </summary>
public sealed class ContainsProductCategory(IReadOnlyList<string> requiredCategories)
    : Specification<Models.Order>
{
    public ContainsProductCategory(string category) : this([category])
    {
    }

    public override bool IsSatisfiedBy(Models.Order candidate) =>
        requiredCategories.Any(candidate.ContainsCategory);
}
