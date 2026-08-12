using Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Modelle;
using Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Spezifikationen.Mahnung;
using Xunit;

namespace Phauthentic.Specification.Beispiele.Offenpostenbuchhaltung.Tests;

public sealed class BerechnetMahnzinsenTest
{
    private static readonly DateOnly Stichtag = new(2026, 1, 31);

    [Fact]
    public void IsSatisfiedBy_ReturnsTrue_WhenAllConditionsAreMet()
    {
        var kandidat = ErzeugeKandidat(tageUeberfaellig: 46, betrag: 100m, aktuelleStufe: Mahnstufe.ErsteMahnung);

        Assert.True(new BerechnetMahnzinsen().IsSatisfiedBy(kandidat));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenBelowFirstDunningLevel()
    {
        var kandidat = ErzeugeKandidat(tageUeberfaellig: 46, betrag: 100m, aktuelleStufe: Mahnstufe.Keine);

        Assert.False(new BerechnetMahnzinsen().IsSatisfiedBy(kandidat));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenNotYetOverdueBeyondGrace()
    {
        var kandidat = ErzeugeKandidat(tageUeberfaellig: 45, betrag: 100m, aktuelleStufe: Mahnstufe.ErsteMahnung);

        Assert.False(new BerechnetMahnzinsen().IsSatisfiedBy(kandidat));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenAmountBelowMinimum()
    {
        var kandidat = ErzeugeKandidat(tageUeberfaellig: 46, betrag: 99.99m, aktuelleStufe: Mahnstufe.ErsteMahnung);

        Assert.False(new BerechnetMahnzinsen().IsSatisfiedBy(kandidat));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenItemIsDisputed()
    {
        var kandidat = ErzeugeKandidat(tageUeberfaellig: 46, betrag: 100m, aktuelleStufe: Mahnstufe.ErsteMahnung, strittig: true);

        Assert.False(new BerechnetMahnzinsen().IsSatisfiedBy(kandidat));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenItemIsNotOpen()
    {
        var kandidat = ErzeugeKandidat(tageUeberfaellig: 46, betrag: 100m, aktuelleStufe: Mahnstufe.ErsteMahnung, abschreiben: true);

        Assert.False(new BerechnetMahnzinsen().IsSatisfiedBy(kandidat));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_ForPreferredCustomer_EvenWhenOtherwiseEligible()
    {
        var kandidat = ErzeugeKandidat(tageUeberfaellig: 46, betrag: 100m, aktuelleStufe: Mahnstufe.ErsteMahnung, risikoklasse: Kundenrisikoklasse.Bevorzugt);

        Assert.False(new BerechnetMahnzinsen().IsSatisfiedBy(kandidat));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenAccountIsBlockedForDunning()
    {
        var kandidat = ErzeugeKandidat(tageUeberfaellig: 46, betrag: 100m, aktuelleStufe: Mahnstufe.ErsteMahnung, gesperrt: true);

        Assert.False(new BerechnetMahnzinsen().IsSatisfiedBy(kandidat));
    }

    private static Mahnkandidat ErzeugeKandidat(
        int tageUeberfaellig,
        decimal betrag,
        Mahnstufe aktuelleStufe,
        bool strittig = false,
        bool gesperrt = false,
        bool abschreiben = false,
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
