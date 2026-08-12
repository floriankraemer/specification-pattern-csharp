using Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Modelle;
using Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Spezifikationen.Konto;
using Xunit;

namespace Phauthentic.Specification.Beispiele.Offenpostenbuchhaltung.Tests;

public sealed class IstNichtFuerMahnwesenGesperrtTest
{
    [Fact]
    public void IsSatisfiedBy_ReturnsTrue_WhenAccountIsNotBlocked()
    {
        var konto = new Hauptbuchkonto("ACC-1", "Test Customer", Kundenrisikoklasse.Standard, "EUR");

        var spec = new IstNichtFuerMahnwesenGesperrt();

        Assert.True(spec.IsSatisfiedBy(konto));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenAccountIsBlocked()
    {
        var konto = new Hauptbuchkonto("ACC-1", "Test Customer", Kundenrisikoklasse.Standard, "EUR");
        konto.Sperren();

        var spec = new IstNichtFuerMahnwesenGesperrt();

        Assert.False(spec.IsSatisfiedBy(konto));
    }
}
