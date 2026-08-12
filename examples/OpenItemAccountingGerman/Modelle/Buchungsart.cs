namespace Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Modelle;

/// <summary>
/// Die Art der Buchung, die auf ein Konto gebucht wird.
/// </summary>
public enum Buchungsart
{
    /// <summary>Erzeugt einen neuen OffenerPosten (z. B. eine Kundenrechnung).</summary>
    Rechnung,

    /// <summary>Gleicht einen bestehenden OffenerPosten ganz oder teilweise aus.</summary>
    Zahlung,

    /// <summary>Reduziert einen bestehenden OffenerPosten ohne Zahlungseingang.</summary>
    Gutschrift,

    /// <summary>Entfernt einen OffenerPosten aus dem Mahnwesen (Forderungsausfall).</summary>
    Abschreibung,
}
