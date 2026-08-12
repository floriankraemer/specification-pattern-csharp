using Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Modelle;
using Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Spezifikationen.Konto;
using Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Spezifikationen.OffenerPosten;

namespace Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Spezifikationen.Mahnung;

/// <summary>
/// Regel für die Übergabe an ein Rechtsverfahren (Inkasso/Klage).
/// </summary>
/// <remarks>
/// Geschäftsregeln:
/// <list type="bullet">
/// <item>Der Posten muss bereits die letzte Mahnstufe erreicht haben</item>
/// <item>Seit dem letzten Mahnlauf müssen mehr als 14 Tage vergangen sein</item>
/// <item>Der Posten muss offen, nicht strittig und mindestens 250 offen sein</item>
/// <item>Das Konto darf nicht bereits gesperrt sein (dann liefe es schon im Rechtsverfahren)</item>
/// </list>
/// </remarks>
public sealed class ErfordertRechtsverfahren : Specification<Modelle.Mahnkandidat>
{
    private const int RechtsverfahrenKarenztage = 14;
    private const decimal MindestRechtsverfahrenBetrag = 250m;

    public override bool IsSatisfiedBy(Modelle.Mahnkandidat kandidat)
    {
        var postenSpezifikation = new IstOffen()
            .And(new IstNichtStrittig())
            .And(new MindestOffenerBetrag(MindestRechtsverfahrenBetrag));

        var kontoSpezifikation = new IstNichtFuerMahnwesenGesperrt();

        var istAufLetzterStufe = kandidat.OffenerPosten.AktuelleMahnstufe == Mahnstufe.LetzteMahnung;
        var ueberfaelligSeitLetztemMahnlauf = kandidat.OffenerPosten.LetztesMahndatum is { } letzterLauf
            && kandidat.Stichtag.DayNumber - letzterLauf.DayNumber > RechtsverfahrenKarenztage;

        return istAufLetzterStufe
            && ueberfaelligSeitLetztemMahnlauf
            && postenSpezifikation.IsSatisfiedBy(kandidat.OffenerPosten)
            && kontoSpezifikation.IsSatisfiedBy(kandidat.Konto);
    }
}
