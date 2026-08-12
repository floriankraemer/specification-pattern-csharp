namespace Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Spezifikationen.OffenerPosten;

/// <summary>
/// Erfüllt, wenn ein OffenerPosten zum Stichtag <paramref name="stichtag"/> um mehr als
/// <paramref name="karenztage"/> Tage überfällig ist.
/// </summary>
public sealed class IstUeberfaellig(DateOnly stichtag, int karenztage) : Specification<Modelle.OffenerPosten>
{
    public override bool IsSatisfiedBy(Modelle.OffenerPosten kandidat) => kandidat.TageUeberfaellig(stichtag) > karenztage;
}
