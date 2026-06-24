using Domain.Enums;

namespace Application.Services.MetrajCalculation;

/// <summary>
/// Türkiye inşaat metraj pratiğine göre varsayılan kurallar.
/// Kaynak: duvar/sıva/boya m² (brüt − minha), şap/mantolama/kalıp m².
/// </summary>
public static class MetrajCalculationDefaults
{
  public const decimal DefaultWallHeightMeters = 2.40m;
  public const decimal DefaultFloorHeightMeters = 3.00m;

  public static readonly string[] OpeningLayerPatterns =
  [
    "KAPI*",
    "KAPILAR*",
    "KAP*",
    "PENC*",
    "PEN*",
    "MINHA*",
    "OPENING*",
    "BOŞLUK*",
    "BOSLUK*",
    "HOLE*"
  ];

  public static readonly string[] OpeningEntityTypes = ["LwPolyline", "Polyline2D", "Hatch", "Line"];

  public static IReadOnlyList<MetrajKalemRule> GetDefaultRules() =>
  [
    new()
    {
      KalemType = MetrajKalemType.Duvar,
      Unit = MeasurementUnit.M2,
      Description =
        "Net duvar alanı: (kapalı alan veya eksen uzunluğu × 2,40 m) − kapı/pencere minha alanları.",
      LayerPatterns =
      [
        "DUVAR*",
        "DUV*",
        "_DUV*",
        "*DUVAR*",
        "WALL*",
        "B_DUVAR*",
        "M_DUVAR*",
        "A-DUV*",
        "A-WALL*",
        "*DIS*DUVAR*",
        "DIS DUVAR*"
      ],
      EntityTypes = ["LwPolyline", "Polyline2D", "Hatch", "Line"],
      Method = MetrajCalculationMethod.LengthTimesHeight,
      DefaultHeight = DefaultWallHeightMeters,
      DeductOpenings = true,
      SideCount = 1
    },
    new()
    {
      KalemType = MetrajKalemType.Siva,
      Unit = MeasurementUnit.M2,
      Description =
        "Sıva alanı: net duvar alanı × 2 yüz (iç mahal). Minha alanları düşülür.",
      LayerPatterns =
      [
        "SIVA*",
        "Sıva*",
        "SIV*",
        "PLASTER*",
        "A-SIV*",
        "IC_SIVA*"
      ],
      EntityTypes = ["LwPolyline", "Polyline2D", "Hatch", "Line"],
      Method = MetrajCalculationMethod.WallNetAreaTimesSides,
      DefaultHeight = DefaultWallHeightMeters,
      DeductOpenings = true,
      SideCount = 2
    },
    new()
    {
      KalemType = MetrajKalemType.Boya,
      Unit = MeasurementUnit.M2,
      Description = "Boya alanı: sıvalı/net duvar yüzeyi (tek yüz). Minha düşülür.",
      LayerPatterns = ["BOYA*", "BOY*", "BOYAMA*", "PAINT*", "A-BOY*", "BADANA*"],
      EntityTypes = ["LwPolyline", "Polyline2D", "Hatch", "Line"],
      Method = MetrajCalculationMethod.WallNetAreaTimesSides,
      DefaultHeight = DefaultWallHeightMeters,
      DeductOpenings = true,
      SideCount = 1
    },
    new()
    {
      KalemType = MetrajKalemType.DisCepheMantolama,
      Unit = MeasurementUnit.M2,
      Description = "Dış cephe mantolama: dış yüzey alanı (m²), pencere minhası düşülür.",
      LayerPatterns =
      [
        "MANTO*",
        "MANTOLAMA*",
        "YALITIM*",
        "DIS_CEPHE*",
        "DIŞ_CEPHE*",
        "EPS*",
        "XPS*",
        "A-MANT*"
      ],
      EntityTypes = ["LwPolyline", "Polyline2D", "Hatch", "Line"],
      Method = MetrajCalculationMethod.LengthTimesHeight,
      DefaultHeight = DefaultFloorHeightMeters,
      DeductOpenings = true,
      SideCount = 1
    },
    new()
    {
      KalemType = MetrajKalemType.SapBeton,
      Unit = MeasurementUnit.M2,
      Description = "Şap beton: döşeme/mahal kapalı alanlarının toplamı (m²).",
      LayerPatterns =
      [
        "SAP*",
        "ŞAP*",
        "SCH*",
        "DOSEME*",
        "DÖŞEME*",
        "_D*SEME*",
        "ZEMIN*",
        "SERAMIK_ALT*",
        "A-SAP*",
        "A-FLOR*",
        "*SLAB*"
      ],
      EntityTypes = ["LwPolyline", "Polyline2D", "Hatch"],
      Method = MetrajCalculationMethod.ClosedArea,
      DeductOpenings = false,
      SideCount = 1
    },
    new()
    {
      KalemType = MetrajKalemType.Kalip,
      Unit = MeasurementUnit.M2,
      Description =
        "Kalıp: beton temas yüzeyi — kapalı döşeme/kiriş/kolon hatlarından alan (m²).",
      LayerPatterns =
      [
        "KALIP*",
        "FORM*",
        "FORMWORK*",
        "BETON*",
        "A-KAL*",
        "A-BET*",
        "DOSEME*",
        "KIRIS*",
        "KIRIŞ*",
        "_KIR*",
        "KOLON*",
        "_KOL*",
        "BEAM*",
        "COLUMN*",
        "*SLAB*"
      ],
      EntityTypes = ["LwPolyline", "Polyline2D", "Hatch"],
      Method = MetrajCalculationMethod.FormworkArea,
      DefaultHeight = DefaultWallHeightMeters,
      DefaultThickness = 0.20m,
      DeductOpenings = false,
      SideCount = 1
    }
  ];

  public static MetrajKalemRule FromTemplate(Domain.Entities.MetrajRuleTemplate template) =>
    new()
    {
      KalemType = template.KalemType,
      Unit = template.Unit,
      LayerPatterns = template.LayerPatterns.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
      EntityTypes = template.EntityTypes.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
      Method = MapMethod(template.KalemType),
      DefaultHeight = template.DefaultHeight ?? DefaultWallHeightMeters,
      DefaultThickness = template.DefaultThickness,
      DeductOpenings = template.DeductOpenings,
      SideCount = template.KalemType == MetrajKalemType.Siva ? 2 : 1,
      Description = template.Name
    };

  private static MetrajCalculationMethod MapMethod(MetrajKalemType kalemType) =>
    kalemType switch
    {
      MetrajKalemType.Duvar => MetrajCalculationMethod.LengthTimesHeight,
      MetrajKalemType.Siva => MetrajCalculationMethod.WallNetAreaTimesSides,
      MetrajKalemType.Boya => MetrajCalculationMethod.WallNetAreaTimesSides,
      MetrajKalemType.DisCepheMantolama => MetrajCalculationMethod.LengthTimesHeight,
      MetrajKalemType.SapBeton => MetrajCalculationMethod.ClosedArea,
      MetrajKalemType.Kalip => MetrajCalculationMethod.FormworkArea,
      _ => MetrajCalculationMethod.ClosedArea
    };
}
