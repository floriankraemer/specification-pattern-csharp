namespace Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Modelle;

/// <summary>
/// Aggregatwurzel für das Konto eines Kunden: eine nur anfügbare Folge von <see cref="Buchung"/>en
/// und die daraus abgeleiteten <see cref="OffenerPosten"/>. Alle Zustandsänderungen an OffenerPosten
/// laufen über dieses Aggregat, damit seine Invarianten (z. B. ein OffenerPosten kann nur durch eine
/// Buchung ausgeglichen werden, die auf ihn verweist) stets gelten.
/// </summary>
public sealed class Hauptbuchkonto
{
    private readonly List<Buchung> _buchungen = [];
    private readonly Dictionary<string, OffenerPosten> _offenPosten = [];

    public string Kontonummer { get; }

    public string Kundenname { get; }

    public Kundenrisikoklasse Risikoklasse { get; }

    public string Waehrung { get; }

    public bool IstFuerMahnwesenGesperrt { get; private set; }

    public Hauptbuchkonto(string kontonummer, string kundenname, Kundenrisikoklasse risikoklasse, string waehrung)
    {
        Kontonummer = kontonummer;
        Kundenname = kundenname;
        Risikoklasse = risikoklasse;
        Waehrung = waehrung;
    }

    public IReadOnlyList<Buchung> Buchungen => _buchungen;

    public IReadOnlyCollection<OffenerPosten> OffenPosten => _offenPosten.Values;

    public void Sperren() => IstFuerMahnwesenGesperrt = true;

    public void Entsperren() => IstFuerMahnwesenGesperrt = false;

    public void Buchen(Buchung buchung)
    {
        if (buchung.Betrag.Waehrung != Waehrung)
        {
            throw new InvalidOperationException(
                $"Buchungswährung {buchung.Betrag.Waehrung} entspricht nicht der Kontowährung {Waehrung}.");
        }

        _buchungen.Add(buchung);

        switch (buchung.Art)
        {
            case Buchungsart.Rechnung:
                var offenerPosten = new OffenerPosten(buchung.Id, buchung.Id, buchung.Faelligkeitsdatum!.Value, buchung.Betrag);
                _offenPosten[offenerPosten.Id] = offenerPosten;
                break;

            case Buchungsart.Zahlung:
            case Buchungsart.Gutschrift:
                OffenerPostenAnfordern(buchung.ReferenzOffenerPostenId!).ZahlungVerbuchen(buchung.Betrag);
                break;

            case Buchungsart.Abschreibung:
                OffenerPostenAnfordern(buchung.ReferenzOffenerPostenId!).Abschreiben();
                break;
        }
    }

    public void OffenerPostenStrittigSetzen(string offenerPostenId, bool strittig) =>
        OffenerPostenAnfordern(offenerPostenId).AlsStrittigMarkieren(strittig);

    public void MahnungEskalieren(string offenerPostenId, Mahnstufe stufe, DateOnly laufDatum) =>
        OffenerPostenAnfordern(offenerPostenId).MahnungEskalieren(stufe, laufDatum);

    private OffenerPosten OffenerPostenAnfordern(string offenerPostenId) =>
        _offenPosten.TryGetValue(offenerPostenId, out var offenerPosten)
            ? offenerPosten
            : throw new InvalidOperationException($"Kein OffenerPosten '{offenerPostenId}' auf Konto {Kontonummer}.");
}
