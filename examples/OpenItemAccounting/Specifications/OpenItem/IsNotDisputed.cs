namespace Phauthentic.Specification.Examples.OpenItemAccounting.Specifications.OpenItem;

/// <summary>
/// Satisfied when an open item is not under dispute. Disputed items must not be dunned.
/// </summary>
public sealed class IsNotDisputed : Specification<Models.OpenItem>
{
    public override bool IsSatisfiedBy(Models.OpenItem candidate) => !candidate.IsDisputed;
}
