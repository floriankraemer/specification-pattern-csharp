using Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Modelle;
using Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Spezifikationen.Konto;
using Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Spezifikationen.OffenerPosten;

namespace Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Spezifikationen.Mahnung;

/// <summary>
/// Regel für den Anfall von Mahnzinsen.
/// </summary>
/// <remarks>
/// Geschäftsregeln:
/// <list type="bullet">
/// <item>Der Posten muss bereits mindestens die erste Mahnstufe erreicht haben</item>
/// <item>Der Posten muss offen, nicht strittig und mehr als 45 Tage überfällig sein</item>
/// <item>Der offene Restbetrag muss mindestens 100 betragen</item>
/// <item>Bevorzugte Kunden sind unabhängig davon von Zinsen befreit</item>
/// <item>Das Konto darf nicht für das Mahnwesen gesperrt sein</item>
/// </list>
/// </remarks>
public sealed class BerechnetMahnzinsen : Specification<Modelle.Mahnkandidat>
{
    private const int ZinsKarenztage = 45;
    private const decimal MindestZinsbetrag = 100m;

    public override bool IsSatisfiedBy(Modelle.Mahnkandidat kandidat)
    {
        var postenSpezifikation = new IstOffen()
            .And(new IstNichtStrittig())
            .And(new MindestOffenerBetrag(MindestZinsbetrag))
            .And(new IstUeberfaellig(kandidat.Stichtag, ZinsKarenztage));

        var kontoSpezifikation = new IstNichtFuerMahnwesenGesperrt()
            .AndNot(new Risikoklasse(Kundenrisikoklasse.Bevorzugt));

        return kandidat.OffenerPosten.AktuelleMahnstufe >= Mahnstufe.ErsteMahnung
            && postenSpezifikation.IsSatisfiedBy(kandidat.OffenerPosten)
            && kontoSpezifikation.IsSatisfiedBy(kandidat.Konto);
    }
}
