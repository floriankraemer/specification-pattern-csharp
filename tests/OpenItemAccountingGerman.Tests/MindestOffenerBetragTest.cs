using Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Modelle;
using Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Spezifikationen.OffenerPosten;
using Xunit;

namespace Phauthentic.Specification.Beispiele.Offenpostenbuchhaltung.Tests;

public sealed class MindestOffenerBetragTest
{
    [Theory]
    [InlineData(100, 0, 100, true)]
    [InlineData(100, 0, 100.01, false)]
    [InlineData(100, 100, 0, true)]
    [InlineData(100, 150, 0, false)]
    [InlineData(100, 150, -100, true)]
    public void IsSatisfiedBy_EvaluatesOpenAmountAgainstThreshold(
        decimal rechnungsbetrag, decimal zahlungsbetrag, decimal mindestbetrag, bool erwartet)
    {
        var stichtag = new DateOnly(2026, 1, 31);
        var konto = new Hauptbuchkonto("ACC-1", "Test Customer", Kundenrisikoklasse.Standard, "EUR");
        konto.Buchen(new Buchung("INV-1", Buchungsart.Rechnung, new Geldbetrag(rechnungsbetrag, "EUR"), stichtag.AddDays(-30), "test", faelligkeitsdatum: stichtag));

        if (zahlungsbetrag != 0)
        {
            konto.Buchen(new Buchung("PAY-1", Buchungsart.Zahlung, new Geldbetrag(zahlungsbetrag, "EUR"), stichtag, "payment", referenzOffenerPostenId: "INV-1"));
        }

        var posten = konto.OffenPosten.Single();
        var spec = new MindestOffenerBetrag(mindestbetrag);

        Assert.Equal(erwartet, spec.IsSatisfiedBy(posten));
    }
}
