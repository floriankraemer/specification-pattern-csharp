using Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Modelle;

namespace Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Spezifikationen.Konto;

/// <summary>
/// Erfüllt, wenn die Risikoklasse eines Kontos einer der angegebenen Klassen entspricht.
/// </summary>
public sealed class Risikoklasse(params Kundenrisikoklasse[] risikoklassen) : Specification<Modelle.Hauptbuchkonto>
{
    public override bool IsSatisfiedBy(Modelle.Hauptbuchkonto kandidat) => risikoklassen.Contains(kandidat.Risikoklasse);
}
