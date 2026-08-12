using Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Modelle;
using Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Spezifikationen.Mahnung;
using Xunit;

namespace Phauthentic.Specification.Beispiele.Offenpostenbuchhaltung.Tests;

public sealed class ErfordertRechtsverfahrenTest
{
    private static readonly DateOnly Stichtag = new(2026, 1, 31);

    [Fact]
    public void IsSatisfiedBy_ReturnsTrue_WhenAllConditionsAreMet()
    {
        var kandidat = ErzeugeKandidat(betrag: 250m, tageSeitLetztemMahnlauf: 15);

        Assert.True(new ErfordertRechtsverfahren().IsSatisfiedBy(kandidat));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenNotAtFinalDunningLevel()
    {
        var kandidat = ErzeugeKandidat(betrag: 250m, tageSeitLetztemMahnlauf: 15, aktuelleStufe: Mahnstufe.ZweiteMahnung);

        Assert.False(new ErfordertRechtsverfahren().IsSatisfiedBy(kandidat));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenNotYetOverdueSinceLastDunningRun()
    {
        var kandidat = ErzeugeKandidat(betrag: 250m, tageSeitLetztemMahnlauf: 14);

        Assert.False(new ErfordertRechtsverfahren().IsSatisfiedBy(kandidat));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenAmountBelowMinimum()
    {
        var kandidat = ErzeugeKandidat(betrag: 249.99m, tageSeitLetztemMahnlauf: 15);

        Assert.False(new ErfordertRechtsverfahren().IsSatisfiedBy(kandidat));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenItemIsDisputed()
    {
        var kandidat = ErzeugeKandidat(betrag: 250m, tageSeitLetztemMahnlauf: 15, strittig: true);

        Assert.False(new ErfordertRechtsverfahren().IsSatisfiedBy(kandidat));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenItemIsNotOpen()
    {
        var kandidat = ErzeugeKandidat(betrag: 250m, tageSeitLetztemMahnlauf: 15, abschreiben: true);

        Assert.False(new ErfordertRechtsverfahren().IsSatisfiedBy(kandidat));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenAccountIsBlockedForDunning()
    {
        var kandidat = ErzeugeKandidat(betrag: 250m, tageSeitLetztemMahnlauf: 15, gesperrt: true);

        Assert.False(new ErfordertRechtsverfahren().IsSatisfiedBy(kandidat));
    }

    private static Mahnkandidat ErzeugeKandidat(
        decimal betrag,
        int tageSeitLetztemMahnlauf,
        Mahnstufe aktuelleStufe = Mahnstufe.LetzteMahnung,
        bool strittig = false,
        bool gesperrt = false,
        bool abschreiben = false)
    {
        var konto = new Hauptbuchkonto("ACC-1", "Test Customer", Kundenrisikoklasse.Standard, "EUR");
        konto.Buchen(new Buchung("INV-1", Buchungsart.Rechnung, new Geldbetrag(betrag, "EUR"), Stichtag.AddDays(-90), "test", faelligkeitsdatum: Stichtag.AddDays(-80)));
        konto.MahnungEskalieren("INV-1", aktuelleStufe, Stichtag.AddDays(-tageSeitLetztemMahnlauf));

        if (strittig)
        {
            konto.OffenerPostenStrittigSetzen("INV-1", true);
        }

        if (abschreiben)
        {
            konto.Buchen(new Buchung("WO-1", Buchungsart.Abschreibung, new Geldbetrag(betrag, "EUR"), Stichtag, "write off", referenzOffenerPostenId: "INV-1"));
        }

        if (gesperrt)
        {
            konto.Sperren();
        }

        var posten = konto.OffenPosten.Single();

        return new Mahnkandidat(konto, posten, Stichtag);
    }
}
