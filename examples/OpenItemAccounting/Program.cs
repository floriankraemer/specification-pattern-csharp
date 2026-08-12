using Phauthentic.Specification.Examples.OpenItemAccounting.Models;
using Phauthentic.Specification.Examples.OpenItemAccounting.Specifications.Dunning;

namespace Phauthentic.Specification.Examples.OpenItemAccounting;

/// <summary>
/// Open-Item Accounting / Dunning (Mahnwesen) Demo.
///
/// This demonstrates the Specification Pattern applied to a real DDD aggregate
/// (<see cref="LedgerAccount"/>) driving a real accounting process: deciding which
/// open items to escalate through a multi-level dunning process, which items accrue
/// interest, and which must be referred to legal action.
/// </summary>
internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        RunDemo();
    }

    private static void RunDemo()
    {
        Console.WriteLine("=== OPEN-ITEM ACCOUNTING / DUNNING RUN ===\n");

        var asOf = new DateOnly(2026, 8, 12);
        var accounts = SampleAccountFactory.CreateSampleAccounts(asOf);
        var engine = new DunningEngine();

        foreach (var account in accounts)
        {
            PrintAccount(account, asOf, engine);
        }

        PrintSummary();
    }

    private static void PrintAccount(LedgerAccount account, DateOnly asOf, DunningEngine engine)
    {
        Console.WriteLine($"Account {account.AccountNumber}: {account.CustomerName}");
        Console.WriteLine($"- Risk Class: {account.RiskClass}");
        Console.WriteLine($"- Blocked For Dunning: {(account.IsBlockedForDunning ? "YES" : "no")}\n");

        foreach (var item in account.OpenItems)
        {
            PrintOpenItem(account, item, asOf, engine);
        }

        Console.WriteLine("---\n");
    }

    private static void PrintOpenItem(LedgerAccount account, OpenItem item, DateOnly asOf, DunningEngine engine)
    {
        var candidate = new DunningCandidate(account, item, asOf);

        Console.WriteLine($"  Open Item {item.Id}: {item.OpenAmount} open of {item.OriginalAmount}");
        Console.WriteLine($"  - Due: {item.DueDate:yyyy-MM-dd} ({item.DaysOverdue(asOf)} days overdue)");
        Console.WriteLine($"  - Disputed: {(item.IsDisputed ? "YES" : "no")}");
        Console.WriteLine($"  - Current Level: {item.CurrentDunningLevel}" +
            (item.LastDunningDate is { } lastRun ? $" (last run {lastRun:yyyy-MM-dd})" : string.Empty));

        Console.WriteLine("  Level Eligibility:");
        PrintLevelEligibility(engine.EvaluateLevels(candidate));

        EscalateIfEligible(account, item, asOf, engine.DetermineNextLevel(candidate));

        Console.WriteLine($"  Accrues Interest: {(engine.AccruesInterest(candidate) ? "YES" : "no")}");
        Console.WriteLine($"  Requires Legal Referral: {(engine.RequiresLegalReferral(candidate) ? "YES" : "no")}");
        Console.WriteLine();
    }

    private static void PrintLevelEligibility(IReadOnlyList<(DunningLevel Level, bool Eligible)> levels)
    {
        foreach (var (level, eligible) in levels)
        {
            Console.WriteLine($"    {(eligible ? "✓" : "✗")} {level}");
        }
    }

    private static void EscalateIfEligible(LedgerAccount account, OpenItem item, DateOnly asOf, DunningLevel? nextLevel)
    {
        if (nextLevel is not { } level)
        {
            Console.WriteLine("  → No escalation this run");
            return;
        }

        account.EscalateDunning(item.Id, level, asOf);
        Console.WriteLine($"  → Escalated to {level}");
    }

    private static void PrintSummary()
    {
        Console.WriteLine("=== SUMMARY ===\n");
        Console.WriteLine("This demo showcases complex, real-world accounting rules implemented using the Specification Pattern:\n");
        Console.WriteLine("- Dunning level escalation: one parameterized specification reused across four escalation levels");
        Console.WriteLine("- Risk-based leniency: the same rule adapts its grace period per customer risk class");
        Console.WriteLine("- Dispute handling: a single atomic specification halts every downstream rule for a disputed item");
        Console.WriteLine("- Interest accrual: an independent rule composed from the same atomic building blocks, using AndNot to exempt preferred customers");
        Console.WriteLine("- Legal referral: a rule that depends on the aggregate's own dunning history (last run date)");
        Console.WriteLine("- Account-level blocking: one specification composed into every rule, so blocking an account halts the entire process without touching level, interest, or legal logic");
    }

}

/// <summary>
/// Evaluates dunning candidates against the level, interest, and legal-referral specifications.
/// </summary>
internal sealed class DunningEngine
{
    private static readonly (DunningLevel Level, int GraceDays, decimal MinimumAmount)[] LevelRules =
    [
        (DunningLevel.FriendlyReminder, 7, 1m),
        (DunningLevel.FirstDunning, 21, 25m),
        (DunningLevel.SecondDunning, 35, 50m),
        (DunningLevel.FinalDunning, 49, 100m),
    ];

    private static readonly AccruesDunningInterest InterestSpec = new();
    private static readonly RequiresLegalActionReferral LegalReferralSpec = new();

    public IReadOnlyList<(DunningLevel Level, bool Eligible)> EvaluateLevels(DunningCandidate candidate) =>
        LevelRules
            .Select(rule => (
                rule.Level,
                Eligible: new DunningLevelEligibility(rule.Level, rule.GraceDays, rule.MinimumAmount)
                    .IsSatisfiedBy(candidate)))
            .ToList();

    public DunningLevel? DetermineNextLevel(DunningCandidate candidate) =>
        EvaluateLevels(candidate)
            .Where(result => result.Eligible)
            .Select(result => (DunningLevel?)result.Level)
            .LastOrDefault();

    public bool AccruesInterest(DunningCandidate candidate) => InterestSpec.IsSatisfiedBy(candidate);

    public bool RequiresLegalReferral(DunningCandidate candidate) => LegalReferralSpec.IsSatisfiedBy(candidate);
}
