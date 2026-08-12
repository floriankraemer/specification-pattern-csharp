namespace Phauthentic.Specification.Beispiele.OffenerPostenbuchhaltung.Modelle;

/// <summary>
/// Bewertungskontext, der einen OffenerPosten mit dem zugehörigen Konto zu einem bestimmten Stichtag verbindet.
/// Ermöglicht es Mahnspezifikationen, kontoweite und postenspezifische Regeln gemeinsam zu betrachten,
/// so wie der <c>Order</c> im ECommerce-Beispiel seinen <c>Customer</c> mitführt.
/// </summary>
public sealed record Mahnkandidat(Hauptbuchkonto Konto, OffenerPosten OffenerPosten, DateOnly Stichtag);
