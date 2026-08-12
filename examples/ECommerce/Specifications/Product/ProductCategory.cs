using Phauthentic.Specification.Examples.ECommerce.Models;

namespace Phauthentic.Specification.Examples.ECommerce.Specifications.Product;

/// <summary>
/// Specification for checking if a product belongs to one of the allowed categories.
/// </summary>
public sealed class ProductCategory(IReadOnlyList<string> allowedCategories) : Specification<Models.Product>
{
    public override bool IsSatisfiedBy(Models.Product candidate) => allowedCategories.Contains(candidate.Category);
}
