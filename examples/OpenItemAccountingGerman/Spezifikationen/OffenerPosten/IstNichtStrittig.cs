namespace Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Spezifikationen.OffenerPosten;

/// <summary>
/// Erfüllt, wenn ein OffenerPosten nicht strittig ist. Strittige Posten dürfen nicht gemahnt werden.
/// </summary>
public sealed class IstNichtStrittig : Specification<Modelle.OffenerPosten>
{
    public override bool IsSatisfiedBy(Modelle.OffenerPosten kandidat) => !kandidat.IstStrittig;
}
