namespace Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Spezifikationen.OffenerPosten;

/// <summary>
/// Spezifikation zur Prüfung, ob der offene Restbetrag eines OffenerPostens einen Mindestwert erreicht.
/// </summary>
public sealed class MindestOffenerBetrag(decimal mindestbetrag) : Specification<Modelle.OffenerPosten>
{
    public override bool IsSatisfiedBy(Modelle.OffenerPosten kandidat) => kandidat.OffenerBetrag.Betrag >= mindestbetrag;
}
