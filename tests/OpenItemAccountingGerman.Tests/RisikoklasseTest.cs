using Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Modelle;
using Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Spezifikationen.Konto;
using Xunit;

namespace Phauthentic.Specification.Beispiele.Offenpostenbuchhaltung.Tests;

public sealed class RisikoklasseTest
{
    [Fact]
    public void IsSatisfiedBy_ReturnsTrue_WhenAccountMatchesOneOfTheGivenClasses()
    {
        var konto = new Hauptbuchkonto("ACC-1", "Test Customer", Kundenrisikoklasse.Bevorzugt, "EUR");

        var spec = new Risikoklasse(Kundenrisikoklasse.Standard, Kundenrisikoklasse.Bevorzugt);

        Assert.True(spec.IsSatisfiedBy(konto));
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenAccountMatchesNoneOfTheGivenClasses()
    {
        var konto = new Hauptbuchkonto("ACC-1", "Test Customer", Kundenrisikoklasse.Hochrisiko, "EUR");

        var spec = new Risikoklasse(Kundenrisikoklasse.Standard, Kundenrisikoklasse.Bevorzugt);

        Assert.False(spec.IsSatisfiedBy(konto));
    }
}
