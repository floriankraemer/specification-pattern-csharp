namespace Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Spezifikationen.Konto;

/// <summary>
/// Erfüllt, wenn ein Konto nicht vom Mahnwesen gesperrt ist (z. B. Insolvenz, Rechtssperre).
/// </summary>
public sealed class IstNichtFuerMahnwesenGesperrt : Specification<Modelle.Hauptbuchkonto>
{
    public override bool IsSatisfiedBy(Modelle.Hauptbuchkonto kandidat) => !kandidat.IstFuerMahnwesenGesperrt;
}
