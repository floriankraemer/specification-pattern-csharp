using Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Modelle;
using Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Spezifikationen.OffenerPosten;
using Xunit;

namespace Phauthentic.Specification.Beispiele.Offenpostenbuchhaltung.Tests;

public sealed class IstOffenTest
{
    [Fact]
    public void IsSatisfiedBy_ReturnsTrue_ForOpenItem()
    {
        var stichtag = new DateOnly(2026, 1, 31);
        var konto = new Hauptbuchkonto("ACC-1", "Test Customer", Kundenrisikoklasse.Standard, "EUR");
        konto.Buchen(new Buchung("INV-1", Buchungsart.Rechnung, new Geldbetrag(100m, "EUR"), stichtag.AddDays(-30), "test", faelligkeitsdatum: stichtag));
        var posten = konto.OffenPosten.Single();

        var spec = new IstOffen();

        Assert.True(spec.IsSatisfiedBy(posten));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_ForClearedItem()
    {
        var stichtag = new DateOnly(2026, 1, 31);
        var konto = new Hauptbuchkonto("ACC-1", "Test Customer", Kundenrisikoklasse.Standard, "EUR");
        konto.Buchen(new Buchung("INV-1", Buchungsart.Rechnung, new Geldbetrag(100m, "EUR"), stichtag.AddDays(-30), "test", faelligkeitsdatum: stichtag));
        konto.Buchen(new Buchung("PAY-1", Buchungsart.Zahlung, new Geldbetrag(100m, "EUR"), stichtag, "payment", referenzOffenerPostenId: "INV-1"));
        var posten = konto.OffenPosten.Single();

        var spec = new IstOffen();

        Assert.False(spec.IsSatisfiedBy(posten));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_ForWrittenOffItem()
    {
        var stichtag = new DateOnly(2026, 1, 31);
        var konto = new Hauptbuchkonto("ACC-1", "Test Customer", Kundenrisikoklasse.Standard, "EUR");
        konto.Buchen(new Buchung("INV-1", Buchungsart.Rechnung, new Geldbetrag(100m, "EUR"), stichtag.AddDays(-30), "test", faelligkeitsdatum: stichtag));
        konto.Buchen(new Buchung("WO-1", Buchungsart.Abschreibung, new Geldbetrag(100m, "EUR"), stichtag, "write off", referenzOffenerPostenId: "INV-1"));
        var posten = konto.OffenPosten.Single();

        var spec = new IstOffen();

        Assert.False(spec.IsSatisfiedBy(posten));
    }
}
