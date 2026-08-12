using Phauthentic.Specification.Examples.ECommerce.Models;

namespace Phauthentic.Specification.Examples.ECommerce.Specifications.Product;

/// <summary>
/// Specification for checking if a product is not a digital product or gift card.
/// </summary>
public sealed class IsNotDigitalProduct : Specification<Models.Product>
{
    public override bool IsSatisfiedBy(Models.Product candidate) => !candidate.IsDigital && !candidate.IsGiftCard;
}
