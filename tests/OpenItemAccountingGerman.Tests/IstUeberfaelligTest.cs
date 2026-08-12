using Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Modelle;
using Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Spezifikationen.OffenerPosten;
using Xunit;

namespace Phauthentic.Specification.Beispiele.Offenpostenbuchhaltung.Tests;

public sealed class IstUeberfaelligTest
{
    [Theory]
    [InlineData(0, 0, false)]
    [InlineData(1, 0, true)]
    [InlineData(5, 5, false)]
    [InlineData(6, 5, true)]
    public void IsSatisfiedBy_EvaluatesOverdueBeyondGraceDays(int tageUeberfaellig, int karenztage, bool erwartet)
    {
        var stichtag = new DateOnly(2026, 1, 31);
        var faelligkeitsdatum = stichtag.AddDays(-tageUeberfaellig);
        var konto = new Hauptbuchkonto("ACC-1", "Test Customer", Kundenrisikoklasse.Standard, "EUR");
        konto.Buchen(new Buchung("INV-1", Buchungsart.Rechnung, new Geldbetrag(100m, "EUR"), faelligkeitsdatum.AddDays(-30), "test", faelligkeitsdatum: faelligkeitsdatum));
        var posten = konto.OffenPosten.Single();

        var spec = new IstUeberfaellig(stichtag, karenztage);

        Assert.Equal(erwartet, spec.IsSatisfiedBy(posten));
    }
}
