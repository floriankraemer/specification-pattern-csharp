using Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Modelle;
using Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Spezifikationen.Mahnung;
using Xunit;

namespace Phauthentic.Specification.Beispiele.Offenpostenbuchhaltung.Tests;

public sealed class MahnstufenBerechtigungTest
{
    private static readonly DateOnly Stichtag = new(2026, 1, 31);

    public static TheoryData<Mahnstufe, int, decimal> Stufenregeln => new()
    {
        { Mahnstufe.FreundlicheErinnerung, 7, 1m },
        { Mahnstufe.ErsteMahnung, 21, 25m },
        { Mahnstufe.ZweiteMahnung, 35, 50m },
        { Mahnstufe.LetzteMahnung, 49, 100m },
    };

    [Theory]
    [MemberData(nameof(Stufenregeln))]
    public void IsSatisfiedBy_ReturnsTrue_WhenOverdueBeyondGraceWithSufficientAmount(
        Mahnstufe stufe, int karenztage, decimal mindestbetrag)
    {
        var kandidat = ErzeugeKandidat(tageUeberfaellig: karenztage + 1, betrag: mindestbetrag);
        var spec = new MahnstufenBerechtigung(stufe, karenztage, mindestbetrag);

        Assert.True(spec.IsSatisfiedBy(kandidat));
    }

    [Theory]
    [MemberData(nameof(Stufenregeln))]
    public void IsSatisfiedBy_ReturnsFalse_WhenNotYetOverdueBeyondGrace(
        Mahnstufe stufe, int karenztage, decimal mindestbetrag)
    {
        var kandidat = ErzeugeKandidat(tageUeberfaellig: karenztage, betrag: mindestbetrag);
        var spec = new MahnstufenBerechtigung(stufe, karenztage, mindestbetrag);

        Assert.False(spec.IsSatisfiedBy(kandidat));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenAmountBelowMinimum()
    {
        var kandidat = ErzeugeKandidat(tageUeberfaellig: 22, betrag: 24.99m);
        var spec = new MahnstufenBerechtigung(Mahnstufe.ErsteMahnung, 21, 25m);

        Assert.False(spec.IsSatisfiedBy(kandidat));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenItemIsDisputed()
    {
        var kandidat = ErzeugeKandidat(tageUeberfaellig: 22, betrag: 25m, strittig: true);
        var spec = new MahnstufenBerechtigung(Mahnstufe.ErsteMahnung, 21, 25m);

        Assert.False(spec.IsSatisfiedBy(kandidat));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenItemAlreadyAtTargetLevel()
    {
        var kandidat = ErzeugeKandidat(tageUeberfaellig: 22, betrag: 25m, aktuelleStufe: Mahnstufe.ErsteMahnung);
        var spec = new MahnstufenBerechtigung(Mahnstufe.ErsteMahnung, 21, 25m);

        Assert.False(spec.IsSatisfiedBy(kandidat));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenItemIsNotOpen()
    {
        var kandidat = ErzeugeKandidat(tageUeberfaellig: 22, betrag: 25m, abschreiben: true);
        var spec = new MahnstufenBerechtigung(Mahnstufe.ErsteMahnung, 21, 25m);

        Assert.False(spec.IsSatisfiedBy(kandidat));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenAccountIsBlockedForDunning()
    {
        var kandidat = ErzeugeKandidat(tageUeberfaellig: 22, betrag: 25m, gesperrt: true);
        var spec = new MahnstufenBerechtigung(Mahnstufe.ErsteMahnung, 21, 25m);

        Assert.False(spec.IsSatisfiedBy(kandidat));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_ForPreferredCustomer_WithinExtendedGracePeriod()
    {
        var kandidat = ErzeugeKandidat(tageUeberfaellig: 25, betrag: 25m, risikoklasse: Kundenrisikoklasse.Bevorzugt);
        var spec = new MahnstufenBerechtigung(Mahnstufe.ErsteMahnung, 21, 25m);

        Assert.False(spec.IsSatisfiedBy(kandidat));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsTrue_ForHighRiskCustomer_BeforeStandardGracePeriodEnds()
    {
        var kandidat = ErzeugeKandidat(tageUeberfaellig: 15, betrag: 25m, risikoklasse: Kundenrisikoklasse.Hochrisiko);
        var spec = new MahnstufenBerechtigung(Mahnstufe.ErsteMahnung, 21, 25m);

        Assert.True(spec.IsSatisfiedBy(kandidat));
    }

    [Theory]
    [InlineData(14, false)]
    [InlineData(15, true)]
    public void IsSatisfiedBy_ForHighRiskCustomer_RespectsAdjustedGracePeriodBoundary(
        int tageUeberfaellig, bool erwartet)
    {
        var kandidat = ErzeugeKandidat(tageUeberfaellig: tageUeberfaellig, betrag: 25m, risikoklasse: Kundenrisikoklasse.Hochrisiko);
        var spec = new MahnstufenBerechtigung(Mahnstufe.ErsteMahnung, 21, 25m);

        Assert.Equal(erwartet, spec.IsSatisfiedBy(kandidat));
    }

    private static Mahnkandidat ErzeugeKandidat(
        int tageUeberfaellig,
        decimal betrag,
        bool strittig = false,
        bool gesperrt = false,
        bool abschreiben = false,
        Mahnstufe aktuelleStufe = Mahnstufe.Keine,
        Kundenrisikoklasse risikoklasse = Kundenrisikoklasse.Standard)
    {
        var faelligkeitsdatum = Stichtag.AddDays(-tageUeberfaellig);
        var konto = new Hauptbuchkonto("ACC-1", "Test Customer", risikoklasse, "EUR");
        konto.Buchen(new Buchung("INV-1", Buchungsart.Rechnung, new Geldbetrag(betrag, "EUR"), faelligkeitsdatum.AddDays(-30), "test", faelligkeitsdatum: faelligkeitsdatum));

        if (strittig)
        {
            konto.OffenerPostenStrittigSetzen("INV-1", true);
        }

        if (abschreiben)
        {
            konto.Buchen(new Buchung("WO-1", Buchungsart.Abschreibung, new Geldbetrag(betrag, "EUR"), Stichtag, "write off", referenzOffenerPostenId: "INV-1"));
        }

        if (aktuelleStufe != Mahnstufe.Keine)
        {
            konto.MahnungEskalieren("INV-1", aktuelleStufe, Stichtag.AddDays(-1));
        }

        if (gesperrt)
        {
            konto.Sperren();
        }

        var posten = konto.OffenPosten.Single();

        return new Mahnkandidat(konto, posten, Stichtag);
    }
}
