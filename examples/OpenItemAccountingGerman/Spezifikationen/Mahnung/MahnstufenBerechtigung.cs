using Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Modelle;
using Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Spezifikationen.Konto;
using Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Spezifikationen.OffenerPosten;

namespace Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Spezifikationen.Mahnung;

/// <summary>
/// Regel für die Eskalation auf eine Mahnstufe.
/// </summary>
/// <remarks>
/// Geschäftsregeln:
/// <list type="bullet">
/// <item>Der Posten muss noch offen und nicht strittig sein</item>
/// <item>Der Posten darf die Zielstufe noch nicht erreicht haben</item>
/// <item>Der offene Restbetrag muss den Mindestbetrag der Stufe erreichen</item>
/// <item>Der Posten muss die Karenzzeit der Stufe überschritten haben</item>
/// <item>Bevorzugte Kunden erhalten 14 zusätzliche Karenztage, Hochrisikokunden 7 weniger</item>
/// <item>Das Konto darf nicht für das Mahnwesen gesperrt sein</item>
/// </list>
/// Eine parametrisierte Klasse ersetzt vier nahezu identische Klassen je Mahnstufe.
/// </remarks>
public sealed class MahnstufenBerechtigung(Mahnstufe zielstufe, int karenztage, decimal mindestbetrag)
    : Specification<Modelle.Mahnkandidat>
{
    public override bool IsSatisfiedBy(Modelle.Mahnkandidat kandidat)
    {
        var risikoAnpassung = kandidat.Konto.Risikoklasse switch
        {
            Kundenrisikoklasse.Bevorzugt => 14,
            Kundenrisikoklasse.Hochrisiko => -7,
            _ => 0,
        };

        var postenSpezifikation = new IstOffen()
            .And(new IstNichtStrittig())
            .And(new HatMahnstufeNichtErreicht(zielstufe))
            .And(new MindestOffenerBetrag(mindestbetrag))
            .And(new IstUeberfaellig(kandidat.Stichtag, karenztage + risikoAnpassung));

        var kontoSpezifikation = new IstNichtFuerMahnwesenGesperrt();

        return postenSpezifikation.IsSatisfiedBy(kandidat.OffenerPosten) && kontoSpezifikation.IsSatisfiedBy(kandidat.Konto);
    }
}
