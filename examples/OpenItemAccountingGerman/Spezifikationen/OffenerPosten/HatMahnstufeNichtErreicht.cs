using Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Modelle;

namespace Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Spezifikationen.OffenerPosten;

/// <summary>
/// Erfüllt, wenn die aktuelle Mahnstufe eines OffenerPostens unter <paramref name="stufe"/> liegt,
/// d. h. er noch auf diese Stufe eskaliert werden kann.
/// </summary>
public sealed class HatMahnstufeNichtErreicht(Mahnstufe stufe) : Specification<Modelle.OffenerPosten>
{
    public override bool IsSatisfiedBy(Modelle.OffenerPosten kandidat) => kandidat.AktuelleMahnstufe < stufe;
}
