using Phauthentic.Specification.Examples.OpenItemAccounting.Models;

namespace Phauthentic.Specification.Examples.OpenItemAccounting.Specifications.OpenItem;

/// <summary>
/// Satisfied when an open item has not yet been cleared or written off.
/// </summary>
public sealed class IsOpen : Specification<Models.OpenItem>
{
    public override bool IsSatisfiedBy(Models.OpenItem candidate) => candidate.Status == OpenItemStatus.Open;
}
