using Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Modelle;

namespace Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung;

/// <summary>
/// Erstellt die Beispiel-<see cref="Hauptbuchkonto"/>en der Demo, jeweils eine Mahnregel isolierend.
/// </summary>
internal static class Beispielkontenfabrik
{
    private const string Waehrung = "EUR";

    public static List<Hauptbuchkonto> BeispielkontenErstellen(DateOnly stichtag)
    {
        // Konto 1: Standardkunde, 40 Tage überfällig - sollte auf die zweite Mahnung eskalieren.
        var acme = new Hauptbuchkonto(
            kontonummer: "10045",
            kundenname: "Acme Fertigungs GmbH",
            risikoklasse: Kundenrisikoklasse.Standard,
            waehrung: Waehrung);

        acme.Buchen(new Buchung(
            id: "RE-1001",
            art: Buchungsart.Rechnung,
            betrag: new Geldbetrag(500m, Waehrung),
            buchungsdatum: stichtag.AddDays(-45),
            beschreibung: "Lieferung von Industrieteilen",
            faelligkeitsdatum: stichtag.AddDays(-40)));

        // Konto 2: Bevorzugter Kunde, ebenfalls 40 Tage überfällig - Kulanz begrenzt dies auf die erste Mahnung.
        var nordisch = new Hauptbuchkonto(
            kontonummer: "10046",
            kundenname: "Nordische Handelsgruppe",
            risikoklasse: Kundenrisikoklasse.Bevorzugt,
            waehrung: Waehrung);
        nordisch.Buchen(new Buchung(
            id: "RE-1002",
            art: Buchungsart.Rechnung,
            betrag: new Geldbetrag(500m, Waehrung),
            buchungsdatum: stichtag.AddDays(-45),
            beschreibung: "Quartalsweise Lagerauffüllung",
            faelligkeitsdatum: stichtag.AddDays(-40)));

        // Konto 3: Hochrisikokunde, nur 10 Tage überfällig - risikobedingter Zuschlag eskaliert früh.
        var schnellmode = new Hauptbuchkonto(
            kontonummer: "10047",
            kundenname: "Schnellmode Express",
            risikoklasse: Kundenrisikoklasse.Hochrisiko,
            waehrung: Waehrung);
        schnellmode.Buchen(new Buchung(
            id: "RE-1003",
            art: Buchungsart.Rechnung,
            betrag: new Geldbetrag(50m, Waehrung),
            buchungsdatum: stichtag.AddDays(-15),
            beschreibung: "Musterbestellung",
            faelligkeitsdatum: stichtag.AddDays(-10)));

        // Konto 4: Standardkunde, 60 Tage überfällig und strittig - Streitfall stoppt jede Regel.
        var bergmann = new Hauptbuchkonto(
            kontonummer: "10048",
            kundenname: "Bergmann Logistik",
            risikoklasse: Kundenrisikoklasse.Standard,
            waehrung: Waehrung);
        bergmann.Buchen(new Buchung(
            id: "RE-1004",
            art: Buchungsart.Rechnung,
            betrag: new Geldbetrag(1000m, Waehrung),
            buchungsdatum: stichtag.AddDays(-65),
            beschreibung: "Speditionsleistungen",
            faelligkeitsdatum: stichtag.AddDays(-60)));
        bergmann.OffenerPostenStrittigSetzen(offenerPostenId: "RE-1004", strittig: true);

        // Konto 5: Standardkunde bereits auf letzter Mahnstufe - löst Zinsanfall und Rechtsverfahren aus.
        var continental = new Hauptbuchkonto(
            kontonummer: "10049",
            kundenname: "Continental Lebensmittel Ltd",
            risikoklasse: Kundenrisikoklasse.Standard,
            waehrung: Waehrung);
        continental.Buchen(new Buchung(
            id: "RE-1005",
            art: Buchungsart.Rechnung,
            betrag: new Geldbetrag(5000m, Waehrung),
            buchungsdatum: stichtag.AddDays(-60),
            beschreibung: "Rohstofflieferung",
            faelligkeitsdatum: stichtag.AddDays(-55)));
        continental.MahnungEskalieren(offenerPostenId: "RE-1005", stufe: Mahnstufe.LetzteMahnung, laufDatum: stichtag.AddDays(-20));

        // Konto 6: Standardkunde, für Mahnwesen gesperrt (z. B. Insolvenzverfahren) - sperrt jede Regel.
        var vantage = new Hauptbuchkonto(
            kontonummer: "10050",
            kundenname: "Vantage Industriezulieferer",
            risikoklasse: Kundenrisikoklasse.Standard,
            waehrung: Waehrung);
        vantage.Buchen(new Buchung(
            id: "RE-1006",
            art: Buchungsart.Rechnung,
            betrag: new Geldbetrag(2000m, Waehrung),
            buchungsdatum: stichtag.AddDays(-95),
            beschreibung: "Maschinenteile-Bestellung",
            faelligkeitsdatum: stichtag.AddDays(-90)));
        vantage.Sperren();

        return [acme, nordisch, schnellmode, bergmann, continental, vantage];
    }
}
