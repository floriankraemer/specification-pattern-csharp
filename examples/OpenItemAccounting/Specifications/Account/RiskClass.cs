using Phauthentic.Specification.Examples.OpenItemAccounting.Models;

namespace Phauthentic.Specification.Examples.OpenItemAccounting.Specifications.Account;

/// <summary>
/// Satisfied when an account's risk class is one of the given classes.
/// </summary>
public sealed class RiskClass(params CustomerRiskClass[] riskClasses) : Specification<Models.LedgerAccount>
{
    public override bool IsSatisfiedBy(Models.LedgerAccount candidate) => riskClasses.Contains(candidate.RiskClass);
}
