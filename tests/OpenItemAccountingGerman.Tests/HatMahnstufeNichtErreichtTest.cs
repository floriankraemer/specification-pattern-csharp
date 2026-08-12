using Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Modelle;
using Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Spezifikationen.OffenerPosten;
using Xunit;

namespace Phauthentic.Specification.Beispiele.Offenpostenbuchhaltung.Tests;

public sealed class HatMahnstufeNichtErreichtTest
{
    [Theory]
    [InlineData(Mahnstufe.Keine, Mahnstufe.ErsteMahnung, true)]
    [InlineData(Mahnstufe.ErsteMahnung, Mahnstufe.ErsteMahnung, false)]
    [InlineData(Mahnstufe.ZweiteMahnung, Mahnstufe.ErsteMahnung, false)]
    public void IsSatisfiedBy_EvaluatesCurrentLevelAgainstTarget(
        Mahnstufe aktuelleStufe, Mahnstufe zielstufe, bool erwartet)
    {
        var stichtag = new DateOnly(2026, 1, 31);
        var konto = new Hauptbuchkonto("ACC-1", "Test Customer", Kundenrisikoklasse.Standard, "EUR");
        konto.Buchen(new Buchung("INV-1", Buchungsart.Rechnung, new Geldbetrag(100m, "EUR"), stichtag.AddDays(-30), "test", faelligkeitsdatum: stichtag));

        if (aktuelleStufe != Mahnstufe.Keine)
        {
            konto.MahnungEskalieren("INV-1", aktuelleStufe, stichtag);
        }

        var posten = konto.OffenPosten.Single();
        var spec = new HatMahnstufeNichtErreicht(zielstufe);

        Assert.Equal(erwartet, spec.IsSatisfiedBy(posten));
    }
}
