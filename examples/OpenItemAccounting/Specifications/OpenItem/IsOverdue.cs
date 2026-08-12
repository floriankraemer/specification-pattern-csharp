namespace Phauthentic.Specification.Examples.OpenItemAccounting.Specifications.OpenItem;

/// <summary>
/// Satisfied when an open item is overdue by more than <paramref name="graceDays"/> as of <paramref name="asOf"/>.
/// </summary>
public sealed class IsOverdue(DateOnly asOf, int graceDays) : Specification<Models.OpenItem>
{
    public override bool IsSatisfiedBy(Models.OpenItem candidate) => candidate.DaysOverdue(asOf) > graceDays;
}
