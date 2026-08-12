namespace Phauthentic.Specification.Examples.OpenItemAccounting.Models;

/// <summary>
/// Aggregate root for a customer's ledger: an append-only stream of <see cref="Posting"/>s
/// and the <see cref="OpenItem"/>s derived from them. All state changes to open items are
/// routed through this aggregate so its invariants (e.g. an item can only be cleared by a
/// posting that references it) always hold.
/// </summary>
public sealed class LedgerAccount
{
    private readonly List<Posting> _postings = [];
    private readonly Dictionary<string, OpenItem> _openItems = [];

    public string AccountNumber { get; }

    public string CustomerName { get; }

    public CustomerRiskClass RiskClass { get; }

    public string Currency { get; }

    public bool IsBlockedForDunning { get; private set; }

    public LedgerAccount(string accountNumber, string customerName, CustomerRiskClass riskClass, string currency)
    {
        AccountNumber = accountNumber;
        CustomerName = customerName;
        RiskClass = riskClass;
        Currency = currency;
    }

    public IReadOnlyList<Posting> Postings => _postings;

    public IReadOnlyCollection<OpenItem> OpenItems => _openItems.Values;

    public void Block() => IsBlockedForDunning = true;

    public void Unblock() => IsBlockedForDunning = false;

    public void Post(Posting posting)
    {
        if (posting.Amount.Currency != Currency)
        {
            throw new InvalidOperationException(
                $"Posting currency {posting.Amount.Currency} does not match account currency {Currency}.");
        }

        _postings.Add(posting);

        switch (posting.Type)
        {
            case PostingType.Invoice:
                var item = new OpenItem(posting.Id, posting.Id, posting.DueDate!.Value, posting.Amount);
                _openItems[item.Id] = item;
                break;

            case PostingType.Payment:
            case PostingType.CreditMemo:
                RequireOpenItem(posting.ReferenceOpenItemId!).ApplyPayment(posting.Amount);
                break;

            case PostingType.WriteOff:
                RequireOpenItem(posting.ReferenceOpenItemId!).WriteOff();
                break;
        }
    }

    public void DisputeOpenItem(string openItemId, bool disputed) =>
        RequireOpenItem(openItemId).MarkDisputed(disputed);

    public void EscalateDunning(string openItemId, DunningLevel level, DateOnly runDate) =>
        RequireOpenItem(openItemId).EscalateDunning(level, runDate);

    private OpenItem RequireOpenItem(string openItemId) =>
        _openItems.TryGetValue(openItemId, out var item)
            ? item
            : throw new InvalidOperationException($"No open item '{openItemId}' on account {AccountNumber}.");
}
