namespace Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Modelle;

/// <summary>
/// Wertobjekt, das einen Geldbetrag in einer bestimmten Währung darstellt.
/// </summary>
public readonly record struct Geldbetrag(decimal Betrag, string Waehrung)
{
    public static Geldbetrag Null(string waehrung) => new(0m, waehrung);

    public Geldbetrag Addieren(Geldbetrag andere) => Verrechnen(andere, static (a, b) => a + b);

    public Geldbetrag Subtrahieren(Geldbetrag andere) => Verrechnen(andere, static (a, b) => a - b);

    public static bool operator <(Geldbetrag links, Geldbetrag rechts) => Vergleichen(links, rechts) < 0;

    public static bool operator >(Geldbetrag links, Geldbetrag rechts) => Vergleichen(links, rechts) > 0;

    public static bool operator <=(Geldbetrag links, Geldbetrag rechts) => Vergleichen(links, rechts) <= 0;

    public static bool operator >=(Geldbetrag links, Geldbetrag rechts) => Vergleichen(links, rechts) >= 0;

    private Geldbetrag Verrechnen(Geldbetrag andere, Func<decimal, decimal, decimal> operation)
    {
        WaehrungMussUebereinstimmen(andere);

        return new Geldbetrag(operation(Betrag, andere.Betrag), Waehrung);
    }

    private static int Vergleichen(Geldbetrag links, Geldbetrag rechts)
    {
        links.WaehrungMussUebereinstimmen(rechts);

        return links.Betrag.CompareTo(rechts.Betrag);
    }

    private void WaehrungMussUebereinstimmen(Geldbetrag andere)
    {
        if (Waehrung != andere.Waehrung)
        {
            throw new InvalidOperationException($"Währungskonflikt: {Waehrung} vs {andere.Waehrung}");
        }
    }

    public override string ToString() => $"{Betrag:F2} {Waehrung}";
}
