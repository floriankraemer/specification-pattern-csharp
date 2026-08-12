namespace Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Modelle;

/// <summary>
/// Ein unveränderlicher Buchungssatz auf einem <see cref="Hauptbuchkonto"/>.
/// </summary>
public sealed class Buchung
{
    public string Id { get; }

    public Buchungsart Art { get; }

    public Geldbetrag Betrag { get; }

    public DateOnly Buchungsdatum { get; }

    /// <summary>Nur bei <see cref="Buchungsart.Rechnung"/>-Buchungen gesetzt.</summary>
    public DateOnly? Faelligkeitsdatum { get; }

    /// <summary>Der OffenerPosten, den diese Buchung ausgleicht oder abschreibt. Bei Rechnungen nicht gesetzt.</summary>
    public string? ReferenzOffenerPostenId { get; }

    public string Beschreibung { get; }

    public Buchung(
        string id,
        Buchungsart art,
        Geldbetrag betrag,
        DateOnly buchungsdatum,
        string beschreibung,
        DateOnly? faelligkeitsdatum = null,
        string? referenzOffenerPostenId = null)
    {
        if (art == Buchungsart.Rechnung && faelligkeitsdatum is null)
        {
            throw new ArgumentException("Rechnungsbuchungen benötigen ein Fälligkeitsdatum.", nameof(faelligkeitsdatum));
        }

        if (art != Buchungsart.Rechnung && referenzOffenerPostenId is null)
        {
            throw new ArgumentException($"{art}-Buchungen benötigen einen Referenz-OffenerPosten.", nameof(referenzOffenerPostenId));
        }

        Id = id;
        Art = art;
        Betrag = betrag;
        Buchungsdatum = buchungsdatum;
        Beschreibung = beschreibung;
        Faelligkeitsdatum = faelligkeitsdatum;
        ReferenzOffenerPostenId = referenzOffenerPostenId;
    }
}
