using Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Modelle;
using Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Spezifikationen.Mahnung;

namespace Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung;

/// <summary>
/// OffenerPostenbuchhaltung / Mahnwesen-Demo.
///
/// Diese Demo zeigt das Specification Pattern angewendet auf ein echtes DDD-Aggregat
/// (<see cref="Hauptbuchkonto"/>), das einen echten Buchhaltungsprozess steuert: welche
/// OffenerPosten in einem mehrstufigen Mahnprozess eskaliert werden, welche Posten Zinsen
/// tragen und welche an ein Rechtsverfahren übergeben werden müssen.
/// </summary>
internal static class Programm
{
    private static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        DemoAusfuehren();
    }

    private static void DemoAusfuehren()
    {
        Console.WriteLine("=== OFFENPOSTENBUCHHALTUNG / MAHNLAUF ===\n");

        var stichtag = new DateOnly(2026, 8, 12);
        var konten = Beispielkontenfabrik.BeispielkontenErstellen(stichtag);
        var mahnprozessor = new Mahnprozessor();

        foreach (var konto in konten)
        {
            KontoAusgeben(konto, stichtag, mahnprozessor);
        }

        ZusammenfassungAusgeben();
    }

    private static void KontoAusgeben(Hauptbuchkonto konto, DateOnly stichtag, Mahnprozessor mahnprozessor)
    {
        Console.WriteLine($"Konto {konto.Kontonummer}: {konto.Kundenname}");
        Console.WriteLine($"- Risikoklasse: {konto.Risikoklasse}");
        Console.WriteLine($"- Für Mahnwesen gesperrt: {(konto.IstFuerMahnwesenGesperrt ? "JA" : "nein")}\n");

        foreach (var posten in konto.OffenPosten)
        {
            OffenerPostenAusgeben(konto, posten, stichtag, mahnprozessor);
        }

        Console.WriteLine("---\n");
    }

    private static void OffenerPostenAusgeben(Hauptbuchkonto konto, OffenerPosten posten, DateOnly stichtag, Mahnprozessor mahnprozessor)
    {
        var kandidat = new Mahnkandidat(konto, posten, stichtag);

        Console.WriteLine($"  OffenerPosten {posten.Id}: {posten.OffenerBetrag} offen von {posten.Ursprungsbetrag}");
        Console.WriteLine($"  - Fällig: {posten.Faelligkeitsdatum:yyyy-MM-dd} ({posten.TageUeberfaellig(stichtag)} Tage überfällig)");
        Console.WriteLine($"  - Strittig: {(posten.IstStrittig ? "JA" : "nein")}");
        Console.WriteLine($"  - Aktuelle Stufe: {posten.AktuelleMahnstufe}" +
            (posten.LetztesMahndatum is { } letzterLauf ? $" (letzter Lauf {letzterLauf:yyyy-MM-dd})" : string.Empty));

        Console.WriteLine("  Stufenberechtigung:");
        StufenberechtigungAusgeben(mahnprozessor.StufenAuswerten(kandidat));

        BeiBerechtigungEskalieren(konto, posten, stichtag, mahnprozessor.NaechsteStufeErmitteln(kandidat));

        Console.WriteLine($"  Zinsen fällig: {(mahnprozessor.ZinsenFaellig(kandidat) ? "JA" : "nein")}");
        Console.WriteLine($"  Rechtsverfahren erforderlich: {(mahnprozessor.RechtsverfahrenErforderlich(kandidat) ? "JA" : "nein")}");
        Console.WriteLine();
    }

    private static void StufenberechtigungAusgeben(IReadOnlyList<(Mahnstufe Stufe, bool Berechtigt)> stufen)
    {
        foreach (var (stufe, berechtigt) in stufen)
        {
            Console.WriteLine($"    {(berechtigt ? "✓" : "✗")} {stufe}");
        }
    }

    private static void BeiBerechtigungEskalieren(Hauptbuchkonto konto, OffenerPosten posten, DateOnly stichtag, Mahnstufe? naechsteStufe)
    {
        if (naechsteStufe is not { } stufe)
        {
            Console.WriteLine("  → Keine Eskalation in diesem Lauf");
            return;
        }

        konto.MahnungEskalieren(posten.Id, stufe, stichtag);
        Console.WriteLine($"  → Eskaliert auf {stufe}");
    }

    private static void ZusammenfassungAusgeben()
    {
        Console.WriteLine("=== ZUSAMMENFASSUNG ===\n");
        Console.WriteLine("Diese Demo zeigt komplexe, praxisnahe Buchhaltungsregeln, umgesetzt mit dem Specification Pattern:\n");
        Console.WriteLine("- Mahnstufeneskalation: eine parametrisierte Spezifikation, wiederverwendet über vier Eskalationsstufen");
        Console.WriteLine("- Risikoabhängige Kulanz: dieselbe Regel passt ihre Karenzzeit je nach Kundenrisikoklasse an");
        Console.WriteLine("- Umgang mit Streitfällen: eine einzelne atomare Spezifikation stoppt für strittige Posten jede nachgelagerte Regel");
        Console.WriteLine("- Zinsanfall: eine eigenständige Regel aus denselben atomaren Bausteinen, die bevorzugte Kunden über AndNot ausnimmt");
        Console.WriteLine("- Rechtsverfahren: eine Regel, die von der eigenen Mahnhistorie des Aggregats abhängt (letzter Lauf)");
        Console.WriteLine("- Kontosperrung: eine Spezifikation, die in jede Regel einfließt, sodass eine Kontosperre den gesamten Prozess stoppt, ohne Stufen-, Zins- oder Rechtslogik anzufassen");
    }

}

/// <summary>
/// Wertet Mahnkandidaten anhand der Stufen-, Zins- und Rechtsverfahrensspezifikationen aus.
/// </summary>
internal sealed class Mahnprozessor
{
    private static readonly (Mahnstufe Stufe, int Karenztage, decimal Mindestbetrag)[] Stufenregeln =
    [
        (Mahnstufe.FreundlicheErinnerung, 7, 1m),
        (Mahnstufe.ErsteMahnung, 21, 25m),
        (Mahnstufe.ZweiteMahnung, 35, 50m),
        (Mahnstufe.LetzteMahnung, 49, 100m),
    ];

    private static readonly BerechnetMahnzinsen ZinsSpezifikation = new();
    private static readonly ErfordertRechtsverfahren RechtsverfahrenSpezifikation = new();

    public IReadOnlyList<(Mahnstufe Stufe, bool Berechtigt)> StufenAuswerten(Mahnkandidat kandidat) =>
        Stufenregeln
            .Select(regel => (
                regel.Stufe,
                Berechtigt: new MahnstufenBerechtigung(regel.Stufe, regel.Karenztage, regel.Mindestbetrag)
                    .IsSatisfiedBy(kandidat)))
            .ToList();

    public Mahnstufe? NaechsteStufeErmitteln(Mahnkandidat kandidat) =>
        StufenAuswerten(kandidat)
            .Where(ergebnis => ergebnis.Berechtigt)
            .Select(ergebnis => (Mahnstufe?)ergebnis.Stufe)
            .LastOrDefault();

    public bool ZinsenFaellig(Mahnkandidat kandidat) => ZinsSpezifikation.IsSatisfiedBy(kandidat);

    public bool RechtsverfahrenErforderlich(Mahnkandidat kandidat) => RechtsverfahrenSpezifikation.IsSatisfiedBy(kandidat);
}
