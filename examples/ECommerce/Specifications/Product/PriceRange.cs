using Phauthentic.Specification.Examples.ECommerce.Models;

namespace Phauthentic.Specification.Examples.ECommerce.Specifications.Product;

/// <summary>
/// Specification for checking if a product's price is within a range.
/// </summary>
public sealed class PriceRange(decimal? minPrice = null, decimal? maxPrice = null) : Specification<Models.Product>
{
    public override bool IsSatisfiedBy(Models.Product candidate) =>
        (minPrice is null || candidate.Price >= minPrice) &&
        (maxPrice is null || candidate.Price <= maxPrice);
}
