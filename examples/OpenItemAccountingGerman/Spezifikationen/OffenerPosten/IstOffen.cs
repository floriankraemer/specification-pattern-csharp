using Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Modelle;

namespace Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Spezifikationen.OffenerPosten;

/// <summary>
/// Erfüllt, wenn ein OffenerPosten noch nicht ausgeglichen oder abgeschrieben wurde.
/// </summary>
public sealed class IstOffen : Specification<Modelle.OffenerPosten>
{
    public override bool IsSatisfiedBy(Modelle.OffenerPosten kandidat) => kandidat.Status == OffenerPostenstatus.Offen;
}
