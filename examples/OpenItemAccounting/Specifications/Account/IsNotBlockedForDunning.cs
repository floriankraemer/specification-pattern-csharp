namespace Phauthentic.Specification.Examples.OpenItemAccounting.Specifications.Account;

/// <summary>
/// Satisfied when an account is not blocked from dunning (e.g. insolvency, legal hold).
/// </summary>
public sealed class IsNotBlockedForDunning : Specification<Models.LedgerAccount>
{
    public override bool IsSatisfiedBy(Models.LedgerAccount candidate) => !candidate.IsBlockedForDunning;
}
