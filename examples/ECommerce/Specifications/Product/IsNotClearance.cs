using Phauthentic.Specification.Examples.ECommerce.Models;

namespace Phauthentic.Specification.Examples.ECommerce.Specifications.Product;

/// <summary>
/// Specification for checking if a product is not on clearance.
/// </summary>
public sealed class IsNotClearance : Specification<Models.Product>
{
    public override bool IsSatisfiedBy(Models.Product candidate) => !candidate.IsClearance;
}
