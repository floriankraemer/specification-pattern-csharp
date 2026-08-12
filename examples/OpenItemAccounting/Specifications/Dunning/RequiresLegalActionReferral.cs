using Phauthentic.Specification.Examples.OpenItemAccounting.Models;
using Phauthentic.Specification.Examples.OpenItemAccounting.Specifications.Account;
using Phauthentic.Specification.Examples.OpenItemAccounting.Specifications.OpenItem;

namespace Phauthentic.Specification.Examples.OpenItemAccounting.Specifications.Dunning;

/// <summary>
/// Legal action referral rule.
/// </summary>
/// <remarks>
/// Business rules:
/// <list type="bullet">
/// <item>The item must already be at Final Dunning level</item>
/// <item>More than 14 days must have passed since the last dunning run</item>
/// <item>The item must be open, not disputed, and have a remaining balance of at least 250</item>
/// <item>The account must not already be blocked for dunning (it would already be in legal handling)</item>
/// </list>
/// </remarks>
public sealed class RequiresLegalActionReferral : Specification<Models.DunningCandidate>
{
    private const int LegalActionGraceDays = 14;
    private const decimal MinimumLegalActionAmount = 250m;

    public override bool IsSatisfiedBy(Models.DunningCandidate candidate)
    {
        var itemSpec = new IsOpen()
            .And(new IsNotDisputed())
            .And(new MinimumOpenAmount(MinimumLegalActionAmount));

        var accountSpec = new IsNotBlockedForDunning();

        var isAtFinalLevel = candidate.OpenItem.CurrentDunningLevel == DunningLevel.FinalDunning;
        var overdueSinceLastDunningRun = candidate.OpenItem.LastDunningDate is { } lastRunDate
            && candidate.AsOf.DayNumber - lastRunDate.DayNumber > LegalActionGraceDays;

        return isAtFinalLevel
            && overdueSinceLastDunningRun
            && itemSpec.IsSatisfiedBy(candidate.OpenItem)
            && accountSpec.IsSatisfiedBy(candidate.Account);
    }
}
