namespace Application.Services.MetrajCalculation;

public enum MetrajCalculationMethod
{
  /// <summary>Kapalı polyline / hatch alanı (m²).</summary>
  ClosedArea = 1,

  /// <summary>Duvar ekseni: toplam çizgi uzunluğu × varsayılan yükseklik (m²).</summary>
  LengthTimesHeight = 2,

  /// <summary>Sıva/boya: duvar net alanı × yüz sayısı.</summary>
  WallNetAreaTimesSides = 3,

  /// <summary>Kalıp: kapalı alan veya çevre × yükseklik (m²).</summary>
  FormworkArea = 4
}
