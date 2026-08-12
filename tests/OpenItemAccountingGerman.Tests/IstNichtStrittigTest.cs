using Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Modelle;
using Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Spezifikationen.OffenerPosten;
using Xunit;

namespace Phauthentic.Specification.Beispiele.Offenpostenbuchhaltung.Tests;

public sealed class IstNichtStrittigTest
{
    [Fact]
    public void IsSatisfiedBy_ReturnsTrue_WhenItemIsNotDisputed()
    {
        var stichtag = new DateOnly(2026, 1, 31);
        var konto = new Hauptbuchkonto("ACC-1", "Test Customer", Kundenrisikoklasse.Standard, "EUR");
        konto.Buchen(new Buchung("INV-1", Buchungsart.Rechnung, new Geldbetrag(100m, "EUR"), stichtag.AddDays(-30), "test", faelligkeitsdatum: stichtag));
        var posten = konto.OffenPosten.Single();

        var spec = new IstNichtStrittig();

        Assert.True(spec.IsSatisfiedBy(posten));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenItemIsDisputed()
    {
        var stichtag = new DateOnly(2026, 1, 31);
        var konto = new Hauptbuchkonto("ACC-1", "Test Customer", Kundenrisikoklasse.Standard, "EUR");
        konto.Buchen(new Buchung("INV-1", Buchungsart.Rechnung, new Geldbetrag(100m, "EUR"), stichtag.AddDays(-30), "test", faelligkeitsdatum: stichtag));
        konto.OffenerPostenStrittigSetzen("INV-1", true);
        var posten = konto.OffenPosten.Single();

        var spec = new IstNichtStrittig();

        Assert.False(spec.IsSatisfiedBy(posten));
    }
}
