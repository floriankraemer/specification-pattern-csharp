namespace Phauthentic.Specification.Examples.OpenItemAccounting.Models;

/// <summary>
/// Escalation level of a dunning (collections/reminder) process, ordered by severity.
/// </summary>
public enum DunningLevel
{
    None = 0,
    FriendlyReminder = 1,
    FirstDunning = 2,
    SecondDunning = 3,
    FinalDunning = 4,
}
