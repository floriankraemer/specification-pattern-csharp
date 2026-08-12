using Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Modelle;
using Xunit;

namespace Phauthentic.Specification.Beispiele.Offenpostenbuchhaltung.Tests;

public sealed class HauptbuchkontoTest
{
    private static readonly DateOnly Stichtag = new(2026, 1, 31);

    [Fact]
    public void Buchen_Rechnung_CreatesOpenOpenItem()
    {
        var konto = new Hauptbuchkonto("ACC-1", "Test Customer", Kundenrisikoklasse.Standard, "EUR");

        konto.Buchen(new Buchung("INV-1", Buchungsart.Rechnung, new Geldbetrag(100m, "EUR"), Stichtag.AddDays(-10), "test", faelligkeitsdatum: Stichtag));

        var posten = Assert.Single(konto.OffenPosten);
        Assert.Equal("INV-1", posten.Id);
        Assert.Equal(OffenerPostenstatus.Offen, posten.Status);
        Assert.Equal(Mahnstufe.Keine, posten.AktuelleMahnstufe);
        Assert.Equal(new Geldbetrag(100m, "EUR"), posten.OffenerBetrag);
        Assert.Equal(Stichtag, posten.Faelligkeitsdatum);
    }

    [Fact]
    public void Buchen_FullPayment_ClearsOpenItem()
    {
        var konto = new Hauptbuchkonto("ACC-1", "Test Customer", Kundenrisikoklasse.Standard, "EUR");
        konto.Buchen(new Buchung("INV-1", Buchungsart.Rechnung, new Geldbetrag(100m, "EUR"), Stichtag.AddDays(-10), "test", faelligkeitsdatum: Stichtag));

        konto.Buchen(new Buchung("PAY-1", Buchungsart.Zahlung, new Geldbetrag(100m, "EUR"), Stichtag, "payment", referenzOffenerPostenId: "INV-1"));

        var posten = Assert.Single(konto.OffenPosten);
        Assert.Equal(OffenerPostenstatus.Ausgeglichen, posten.Status);
        Assert.Equal(new Geldbetrag(0m, "EUR"), posten.OffenerBetrag);
    }

    [Fact]
    public void Buchen_PartialPayment_ReducesOpenAmountButKeepsItemOpen()
    {
        var konto = new Hauptbuchkonto("ACC-1", "Test Customer", Kundenrisikoklasse.Standard, "EUR");
        konto.Buchen(new Buchung("INV-1", Buchungsart.Rechnung, new Geldbetrag(100m, "EUR"), Stichtag.AddDays(-10), "test", faelligkeitsdatum: Stichtag));

        konto.Buchen(new Buchung("PAY-1", Buchungsart.Zahlung, new Geldbetrag(40m, "EUR"), Stichtag, "payment", referenzOffenerPostenId: "INV-1"));

        var posten = Assert.Single(konto.OffenPosten);
        Assert.Equal(OffenerPostenstatus.Offen, posten.Status);
        Assert.Equal(new Geldbetrag(60m, "EUR"), posten.OffenerBetrag);
    }

    [Fact]
    public void Buchen_CreditMemo_ReducesOpenAmount()
    {
        var konto = new Hauptbuchkonto("ACC-1", "Test Customer", Kundenrisikoklasse.Standard, "EUR");
        konto.Buchen(new Buchung("INV-1", Buchungsart.Rechnung, new Geldbetrag(100m, "EUR"), Stichtag.AddDays(-10), "test", faelligkeitsdatum: Stichtag));

        konto.Buchen(new Buchung("CM-1", Buchungsart.Gutschrift, new Geldbetrag(30m, "EUR"), Stichtag, "credit memo", referenzOffenerPostenId: "INV-1"));

        var posten = Assert.Single(konto.OffenPosten);
        Assert.Equal(new Geldbetrag(70m, "EUR"), posten.OffenerBetrag);
    }

    [Fact]
    public void Buchen_WriteOff_SetsStatusWrittenOff()
    {
        var konto = new Hauptbuchkonto("ACC-1", "Test Customer", Kundenrisikoklasse.Standard, "EUR");
        konto.Buchen(new Buchung("INV-1", Buchungsart.Rechnung, new Geldbetrag(100m, "EUR"), Stichtag.AddDays(-10), "test", faelligkeitsdatum: Stichtag));

        konto.Buchen(new Buchung("WO-1", Buchungsart.Abschreibung, new Geldbetrag(100m, "EUR"), Stichtag, "write off", referenzOffenerPostenId: "INV-1"));

        var posten = Assert.Single(konto.OffenPosten);
        Assert.Equal(OffenerPostenstatus.Abgeschrieben, posten.Status);
    }

    [Fact]
    public void Buchen_WithMismatchedCurrency_Throws()
    {
        var konto = new Hauptbuchkonto("ACC-1", "Test Customer", Kundenrisikoklasse.Standard, "EUR");

        Assert.Throws<InvalidOperationException>(() =>
            konto.Buchen(new Buchung("INV-1", Buchungsart.Rechnung, new Geldbetrag(100m, "USD"), Stichtag.AddDays(-10), "test", faelligkeitsdatum: Stichtag)));
    }

    [Fact]
    public void Buchen_PaymentForUnknownOpenItem_Throws()
    {
        var konto = new Hauptbuchkonto("ACC-1", "Test Customer", Kundenrisikoklasse.Standard, "EUR");

        Assert.Throws<InvalidOperationException>(() =>
            konto.Buchen(new Buchung("PAY-1", Buchungsart.Zahlung, new Geldbetrag(100m, "EUR"), Stichtag, "payment", referenzOffenerPostenId: "INV-1")));
    }

    [Fact]
    public void OffenerPostenStrittigSetzen_SetsIsDisputed()
    {
        var konto = new Hauptbuchkonto("ACC-1", "Test Customer", Kundenrisikoklasse.Standard, "EUR");
        konto.Buchen(new Buchung("INV-1", Buchungsart.Rechnung, new Geldbetrag(100m, "EUR"), Stichtag.AddDays(-10), "test", faelligkeitsdatum: Stichtag));

        konto.OffenerPostenStrittigSetzen("INV-1", true);
        Assert.True(konto.OffenPosten.Single().IstStrittig);

        konto.OffenerPostenStrittigSetzen("INV-1", false);
        Assert.False(konto.OffenPosten.Single().IstStrittig);
    }

    [Fact]
    public void OffenerPostenStrittigSetzen_WithUnknownOpenItemId_Throws()
    {
        var konto = new Hauptbuchkonto("ACC-1", "Test Customer", Kundenrisikoklasse.Standard, "EUR");

        Assert.Throws<InvalidOperationException>(() => konto.OffenerPostenStrittigSetzen("INV-1", true));
    }

    [Fact]
    public void MahnungEskalieren_SetsLevelAndLastDunningDate()
    {
        var konto = new Hauptbuchkonto("ACC-1", "Test Customer", Kundenrisikoklasse.Standard, "EUR");
        konto.Buchen(new Buchung("INV-1", Buchungsart.Rechnung, new Geldbetrag(100m, "EUR"), Stichtag.AddDays(-10), "test", faelligkeitsdatum: Stichtag));

        konto.MahnungEskalieren("INV-1", Mahnstufe.ZweiteMahnung, Stichtag);

        var posten = konto.OffenPosten.Single();
        Assert.Equal(Mahnstufe.ZweiteMahnung, posten.AktuelleMahnstufe);
        Assert.Equal(Stichtag, posten.LetztesMahndatum);
    }

    [Fact]
    public void MahnungEskalieren_WithUnknownOpenItemId_Throws()
    {
        var konto = new Hauptbuchkonto("ACC-1", "Test Customer", Kundenrisikoklasse.Standard, "EUR");

        Assert.Throws<InvalidOperationException>(() => konto.MahnungEskalieren("INV-1", Mahnstufe.ErsteMahnung, Stichtag));
    }

    [Fact]
    public void Sperren_SetsIsBlockedForDunning()
    {
        var konto = new Hauptbuchkonto("ACC-1", "Test Customer", Kundenrisikoklasse.Standard, "EUR");

        konto.Sperren();

        Assert.True(konto.IstFuerMahnwesenGesperrt);
    }

    [Fact]
    public void Entsperren_ClearsIsBlockedForDunning()
    {
        var konto = new Hauptbuchkonto("ACC-1", "Test Customer", Kundenrisikoklasse.Standard, "EUR");
        konto.Sperren();

        konto.Entsperren();

        Assert.False(konto.IstFuerMahnwesenGesperrt);
    }
}
