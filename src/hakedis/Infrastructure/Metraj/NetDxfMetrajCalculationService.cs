using Application.Services.MetrajCalculation;
using Domain;
using Domain.Enums;
using netDxf;

namespace Infrastructure.Metraj;

public class NetDxfMetrajCalculationService : IMetrajCalculationService
{
  public Task<DrawingLayersDiscoveryResultDto> DiscoverLayersAsync(
    string filePath,
    string fileExtension,
    CancellationToken cancellationToken = default
  )
  {
    if (!DxfLayerMetricsLoader.TryLoad(filePath, fileExtension, out Dictionary<string, LayerEntityMetrics> metrics, out string? errorMessage))
    {
      return Task.FromResult(
        new DrawingLayersDiscoveryResultDto { Success = false, ErrorMessage = errorMessage ?? "Katman listesi okunamadı." }
      );
    }

    IList<DrawingLayerItemDto> layers = metrics
      .Where(pair => pair.Value.EntityCount > 0)
      .OrderByDescending(pair => pair.Value.ClosedArea + pair.Value.LineLength)
      .ThenBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase)
      .Select(pair => new DrawingLayerItemDto
      {
        Name = pair.Key,
        EntityCount = pair.Value.EntityCount,
        HasClosedArea = pair.Value.ClosedArea > 0,
        HasLines = pair.Value.LineLength > 0,
      })
      .ToList();

    return Task.FromResult(
      new DrawingLayersDiscoveryResultDto
      {
        Success = true,
        Layers = layers,
      }
    );
  }

  public Task<MetrajCalculationResultDto> CalculateAsync(
    MetrajCalculationRequest request,
    CancellationToken cancellationToken = default
  )
  {
    string extension = request.FileExtension.Trim().TrimStart('.').ToLowerInvariant();

    if (extension == "dwg")
    {
      return Task.FromResult(
        new MetrajCalculationResultDto
        {
          Success = false,
          ErrorMessage =
            "DWG dosyaları için netDxf desteklenmiyor. DXF yükleyin veya ileride APS Automation entegrasyonu ekleyin."
        }
      );
    }

    if (extension != "dxf")
    {
      return Task.FromResult(
        new MetrajCalculationResultDto
        {
          Success = false,
          ErrorMessage = "Yalnızca DXF dosyaları hesaplanabilir."
        }
      );
    }

    if (!File.Exists(request.FilePath))
    {
      return Task.FromResult(
        new MetrajCalculationResultDto { Success = false, ErrorMessage = "Çizim dosyası bulunamadı." }
      );
    }

    try
    {
      MetrajCalculationResultDto? result = TryCalculateWithNetDxf(request);
      if (result is not null)
        return Task.FromResult(result);

      result = TryCalculateWithLegacyParser(request);
      if (result is not null)
        return Task.FromResult(result);

      return Task.FromResult(
        new MetrajCalculationResultDto
        {
          Success = false,
          ErrorMessage =
            "DXF dosyası okunamadı. AutoCAD R14 veya daha eski format olabilir. "
              + "AutoCAD / LibreCAD / DWG TrueView ile 'AutoCAD 2000 DXF' veya üzeri olarak yeniden kaydedin."
        }
      );
    }
    catch (Exception ex)
    {
      return Task.FromResult(
        new MetrajCalculationResultDto { Success = false, ErrorMessage = FormatDxfError(ex) }
      );
    }
  }

  private static MetrajCalculationResultDto? TryCalculateWithNetDxf(MetrajCalculationRequest request)
  {
    if (DxfLegacyLoader.IsLegacyVersion(request.FilePath))
      return null;

    try
    {
      DxfDocument document = DxfDocument.Load(request.FilePath);
      return CalculateFromDocument(document, request);
    }
    catch
    {
      return null;
    }
  }

  private static MetrajCalculationResultDto? TryCalculateWithLegacyParser(MetrajCalculationRequest request)
  {
    try
    {
      Dictionary<string, LayerEntityMetrics> layerMetrics = DxfEntitiesOnlyParser.Parse(request.FilePath);
      if (layerMetrics.Count == 0)
        return null;

      int insUnits = DxfEntitiesOnlyParser.GetInsUnits(request.FilePath);
      (double lengthToMeters, double areaToSquareMeters, string unitNote) =
        DxfEntitiesOnlyParser.GetScaleFromInsUnits(insUnits);

      return CalculateFromLayerMetrics(layerMetrics, request, lengthToMeters, areaToSquareMeters, unitNote);
    }
    catch
    {
      return null;
    }
  }

  private static MetrajCalculationResultDto CalculateFromDocument(
    DxfDocument document,
    MetrajCalculationRequest request
  )
  {
    try
    {
      (double lengthToMeters, double areaToSquareMeters, string unitNote) = DrawingUnitConverter.GetScale(document);
      Dictionary<string, LayerEntityMetrics> layerMetrics = NetDxfGeometryHelper.ScanDocument(document);
      return CalculateFromLayerMetrics(layerMetrics, request, lengthToMeters, areaToSquareMeters, unitNote);
    }
    catch (Exception ex)
    {
      return new MetrajCalculationResultDto { Success = false, ErrorMessage = FormatDxfError(ex) };
    }
  }

  private static MetrajCalculationResultDto CalculateFromLayerMetrics(
    Dictionary<string, LayerEntityMetrics> layerMetrics,
    MetrajCalculationRequest request,
    double lengthToMeters,
    double areaToSquareMeters,
    string unitNote
  )
  {
    try
    {

      double openingAreaM2 = NetDxfGeometryHelper.SumForPatterns(
        layerMetrics,
        MetrajCalculationDefaults.OpeningLayerPatterns,
        m => m.ClosedArea * areaToSquareMeters
      );

      // Eksen çizgisi minha için: kapı/pencere layer'ındaki çizgi × varsayılan yükseklik
      openingAreaM2 += NetDxfGeometryHelper.SumForPatterns(
        layerMetrics,
        MetrajCalculationDefaults.OpeningLayerPatterns,
        m => m.LineLength * lengthToMeters * (double)MetrajCalculationDefaults.DefaultWallHeightMeters
      );

      List<MetrajCalculationItemDto> items = [];
      IReadOnlyList<MetrajKalemRule> rules =
        request.Rules.Count > 0 ? request.Rules : MetrajCalculationDefaults.GetDefaultRules();

      decimal? sharedWallNetM2 = null;

      foreach (MetrajKalemRule rule in rules.OrderBy(r => (int)r.KalemType))
      {
        decimal quantity = CalculateQuantity(
          rule,
          layerMetrics,
          lengthToMeters,
          areaToSquareMeters,
          (decimal)openingAreaM2,
          ref sharedWallNetM2
        );

        if (quantity <= 0)
          continue;

        items.Add(
          new MetrajCalculationItemDto
          {
            KalemType = rule.KalemType,
            Unit = rule.Unit,
            Quantity = Math.Round(quantity, 3),
            FloorName = request.FloorName,
            SpaceName = request.SpaceName,
            Notes = BuildNote(rule, quantity, (decimal)openingAreaM2)
          }
        );
      }

      if (items.Count == 0)
      {
        return new MetrajCalculationResultDto
        {
          Success = false,
          ErrorMessage =
            "Çizimde eşleşen layer bulunamadı. Proje katman eşlemesini kontrol edin veya DUVAR, SIVA, BOYA, MANTO, SAP, KALIP layer isimlerini doğrulayın.",
          DrawingUnitNote = unitNote
        };
      }

      return new MetrajCalculationResultDto
      {
        Success = true,
        DrawingUnitNote = unitNote,
        Items = items
      };
    }
    catch (Exception ex)
    {
      return new MetrajCalculationResultDto { Success = false, ErrorMessage = FormatDxfError(ex) };
    }
  }

  private static string FormatDxfError(Exception ex)
  {
    if (ex is InvalidOperationException)
      return ex.Message;

    string message = ex.Message;
    if (message.Contains("version not supported", StringComparison.OrdinalIgnoreCase)
        || message.Contains("AutoCad14", StringComparison.OrdinalIgnoreCase)
        || message.Contains("AC1014", StringComparison.OrdinalIgnoreCase))
    {
      return "DXF dosyası eski AutoCAD R14 formatında. Sistem otomatik yükseltmeyi denedi ancak başarısız oldu. "
        + "AutoCAD veya LibreCAD ile 'AutoCAD 2000 DXF' veya üzeri olarak yeniden kaydedip tekrar yükleyin.";
    }

    return $"DXF okuma hatası: {message}";
  }

  private static decimal CalculateQuantity(
    MetrajKalemRule rule,
    Dictionary<string, LayerEntityMetrics> layerMetrics,
    double lengthToMeters,
    double areaToSquareMeters,
    decimal openingAreaM2,
    ref decimal? sharedWallNetM2
  )
  {
    double closedAreaM2 =
      NetDxfGeometryHelper.SumForPatterns(layerMetrics, rule.LayerPatterns, m => m.ClosedArea * areaToSquareMeters);

    double lineLengthM =
      NetDxfGeometryHelper.SumForPatterns(layerMetrics, rule.LayerPatterns, m => m.LineLength * lengthToMeters);

    decimal height = rule.DefaultHeight ?? MetrajCalculationDefaults.DefaultWallHeightMeters;
    decimal grossM2 = (decimal)closedAreaM2;

    switch (rule.Method)
    {
      case MetrajCalculationMethod.ClosedArea:
      case MetrajCalculationMethod.FormworkArea:
        grossM2 = (decimal)closedAreaM2;
        if (rule.Method == MetrajCalculationMethod.FormworkArea && grossM2 == 0 && lineLengthM > 0)
        {
          decimal thickness = rule.DefaultThickness ?? 0.20m;
          grossM2 = (decimal)lineLengthM * height * 2 + (decimal)lineLengthM * thickness;
        }
        break;

      case MetrajCalculationMethod.LengthTimesHeight:
        grossM2 = (decimal)closedAreaM2;
        if (lineLengthM > 0)
          grossM2 += (decimal)lineLengthM * height;
        break;

      case MetrajCalculationMethod.WallNetAreaTimesSides:
        if (!sharedWallNetM2.HasValue)
        {
          MetrajKalemRule duvarRule = MetrajCalculationDefaults
            .GetDefaultRules()
            .First(r => r.KalemType == MetrajKalemType.Duvar);
          decimal duvarGross = CalculateGrossWallM2(duvarRule, layerMetrics, lengthToMeters, areaToSquareMeters, height);
          sharedWallNetM2 = Math.Max(0, duvarGross - (duvarRule.DeductOpenings ? openingAreaM2 : 0));
        }

        decimal baseNet = sharedWallNetM2.Value;
        if (baseNet == 0)
          baseNet = CalculateGrossWallM2(rule, layerMetrics, lengthToMeters, areaToSquareMeters, height);

        grossM2 = baseNet * rule.SideCount;
        return rule.DeductOpenings && rule.SideCount <= 1
          ? Math.Max(0, grossM2 - openingAreaM2)
          : grossM2;
    }

    if (rule.DeductOpenings)
      grossM2 = Math.Max(0, grossM2 - openingAreaM2);

    return grossM2;
  }

  private static decimal CalculateGrossWallM2(
    MetrajKalemRule rule,
    Dictionary<string, LayerEntityMetrics> layerMetrics,
    double lengthToMeters,
    double areaToSquareMeters,
    decimal height
  )
  {
    double closedAreaM2 =
      NetDxfGeometryHelper.SumForPatterns(layerMetrics, rule.LayerPatterns, m => m.ClosedArea * areaToSquareMeters);
    double lineLengthM =
      NetDxfGeometryHelper.SumForPatterns(layerMetrics, rule.LayerPatterns, m => m.LineLength * lengthToMeters);

    return (decimal)closedAreaM2 + (decimal)lineLengthM * height;
  }

  private static string BuildNote(MetrajKalemRule rule, decimal quantity, decimal openingAreaM2)
  {
    string openingPart = rule.DeductOpenings && openingAreaM2 > 0 ? $" Minha düşümü: {openingAreaM2:F3} m²." : string.Empty;
    return $"{rule.Description} Hesaplanan: {quantity:F3} {MeasurementUnitDefaults.GetLabel(rule.Unit)}.{openingPart}";
  }
}
