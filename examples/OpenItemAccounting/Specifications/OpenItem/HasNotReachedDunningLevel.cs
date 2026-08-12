using Phauthentic.Specification.Examples.OpenItemAccounting.Models;

namespace Phauthentic.Specification.Examples.OpenItemAccounting.Specifications.OpenItem;

/// <summary>
/// Satisfied when an open item's current dunning level is below <paramref name="level"/>,
/// i.e. it is still eligible to be escalated to that level.
/// </summary>
public sealed class HasNotReachedDunningLevel(DunningLevel level) : Specification<Models.OpenItem>
{
    public override bool IsSatisfiedBy(Models.OpenItem candidate) => candidate.CurrentDunningLevel < level;
}
