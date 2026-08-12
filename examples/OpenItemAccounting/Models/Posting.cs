namespace Phauthentic.Specification.Examples.OpenItemAccounting.Models;

/// <summary>
/// An immutable, append-only ledger entry on a <see cref="LedgerAccount"/>.
/// </summary>
public sealed class Posting
{
    public string Id { get; }

    public PostingType Type { get; }

    public Money Amount { get; }

    public DateOnly PostingDate { get; }

    /// <summary>Only set for <see cref="PostingType.Invoice"/> postings.</summary>
    public DateOnly? DueDate { get; }

    /// <summary>The open item this posting clears or writes off. Not set for invoices.</summary>
    public string? ReferenceOpenItemId { get; }

    public string Description { get; }

    public Posting(
        string id,
        PostingType type,
        Money amount,
        DateOnly postingDate,
        string description,
        DateOnly? dueDate = null,
        string? referenceOpenItemId = null)
    {
        if (type == PostingType.Invoice && dueDate is null)
        {
            throw new ArgumentException("Invoice postings require a due date.", nameof(dueDate));
        }

        if (type != PostingType.Invoice && referenceOpenItemId is null)
        {
            throw new ArgumentException($"{type} postings require a reference open item.", nameof(referenceOpenItemId));
        }

        Id = id;
        Type = type;
        Amount = amount;
        PostingDate = postingDate;
        Description = description;
        DueDate = dueDate;
        ReferenceOpenItemId = referenceOpenItemId;
    }
}
