namespace Phauthentic.Specification.Examples.OpenItemAccounting.Models;

/// <summary>
/// A not-yet-settled invoice tracked on a <see cref="LedgerAccount"/>.
/// Mutations are internal — only the owning aggregate may change an open item's state.
/// </summary>
public sealed class OpenItem
{
    public string Id { get; }

    public string InvoicePostingId { get; }

    public DateOnly DueDate { get; }

    public Money OriginalAmount { get; }

    public Money OpenAmount { get; private set; }

    public OpenItemStatus Status { get; private set; }

    public DunningLevel CurrentDunningLevel { get; private set; }

    public DateOnly? LastDunningDate { get; private set; }

    public bool IsDisputed { get; private set; }

    internal OpenItem(string id, string invoicePostingId, DateOnly dueDate, Money originalAmount)
    {
        Id = id;
        InvoicePostingId = invoicePostingId;
        DueDate = dueDate;
        OriginalAmount = originalAmount;
        OpenAmount = originalAmount;
        Status = OpenItemStatus.Open;
        CurrentDunningLevel = DunningLevel.None;
    }

    internal void ApplyPayment(Money amount)
    {
        OpenAmount = OpenAmount.Subtract(amount);

        if (OpenAmount.Amount <= 0)
        {
            Status = OpenItemStatus.Cleared;
        }
    }

    internal void WriteOff() => Status = OpenItemStatus.WrittenOff;

    internal void MarkDisputed(bool disputed) => IsDisputed = disputed;

    internal void EscalateDunning(DunningLevel level, DateOnly runDate)
    {
        CurrentDunningLevel = level;
        LastDunningDate = runDate;
    }

    public int DaysOverdue(DateOnly asOf) => Math.Max(0, asOf.DayNumber - DueDate.DayNumber);
}
