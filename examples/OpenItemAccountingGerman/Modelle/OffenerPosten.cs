namespace Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Modelle;

/// <summary>
/// Eine noch nicht ausgeglichene Rechnung, die auf einem <see cref="Hauptbuchkonto"/> geführt wird.
/// Zustandsänderungen sind intern — nur das besitzende Aggregat darf den Zustand eines OffenerPostens ändern.
/// </summary>
public sealed class OffenerPosten
{
    public string Id { get; }

    public string RechnungsbuchungId { get; }

    public DateOnly Faelligkeitsdatum { get; }

    public Geldbetrag Ursprungsbetrag { get; }

    public Geldbetrag OffenerBetrag { get; private set; }

    public OffenerPostenstatus Status { get; private set; }

    public Mahnstufe AktuelleMahnstufe { get; private set; }

    public DateOnly? LetztesMahndatum { get; private set; }

    public bool IstStrittig { get; private set; }

    internal OffenerPosten(string id, string rechnungsbuchungId, DateOnly faelligkeitsdatum, Geldbetrag ursprungsbetrag)
    {
        Id = id;
        RechnungsbuchungId = rechnungsbuchungId;
        Faelligkeitsdatum = faelligkeitsdatum;
        Ursprungsbetrag = ursprungsbetrag;
        OffenerBetrag = ursprungsbetrag;
        Status = OffenerPostenstatus.Offen;
        AktuelleMahnstufe = Mahnstufe.Keine;
    }

    internal void ZahlungVerbuchen(Geldbetrag betrag)
    {
        OffenerBetrag = OffenerBetrag.Subtrahieren(betrag);

        if (OffenerBetrag.Betrag <= 0)
        {
            Status = OffenerPostenstatus.Ausgeglichen;
        }
    }

    internal void Abschreiben() => Status = OffenerPostenstatus.Abgeschrieben;

    internal void AlsStrittigMarkieren(bool strittig) => IstStrittig = strittig;

    internal void MahnungEskalieren(Mahnstufe stufe, DateOnly laufDatum)
    {
        AktuelleMahnstufe = stufe;
        LetztesMahndatum = laufDatum;
    }

    public int TageUeberfaellig(DateOnly stichtag) => Math.Max(0, stichtag.DayNumber - Faelligkeitsdatum.DayNumber);
}
