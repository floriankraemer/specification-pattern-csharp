namespace Phauthentic.Specification.Examples.OpenItemAccounting.Specifications.OpenItem;

/// <summary>
/// Specification for checking if an open item's remaining balance meets a minimum threshold.
/// </summary>
public sealed class MinimumOpenAmount(decimal minimumAmount) : Specification<Models.OpenItem>
{
    public override bool IsSatisfiedBy(Models.OpenItem candidate) => candidate.OpenAmount.Amount >= minimumAmount;
}
