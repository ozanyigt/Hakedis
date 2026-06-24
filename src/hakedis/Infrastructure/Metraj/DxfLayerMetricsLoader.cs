using netDxf;

namespace Infrastructure.Metraj;

internal static class DxfLayerMetricsLoader
{
  public static bool TryLoad(string filePath, string fileExtension, out Dictionary<string, LayerEntityMetrics> metrics, out string? errorMessage)
  {
    metrics = new Dictionary<string, LayerEntityMetrics>(StringComparer.OrdinalIgnoreCase);
    errorMessage = null;

    string extension = fileExtension.Trim().TrimStart('.').ToLowerInvariant();
    if (extension == "dwg")
    {
      errorMessage =
        "DWG dosyaları için katman listesi çıkarılamıyor. DXF yükleyin veya AutoCAD ile DXF olarak export edin.";
      return false;
    }

    if (extension != "dxf")
    {
      errorMessage = "Yalnızca DXF dosyalarından katman listesi çıkarılabilir.";
      return false;
    }

    if (!File.Exists(filePath))
    {
      errorMessage = "Çizim dosyası bulunamadı.";
      return false;
    }

    if (!DxfLegacyLoader.IsLegacyVersion(filePath))
    {
      try
      {
        DxfDocument document = DxfDocument.Load(filePath);
        metrics = NetDxfGeometryHelper.ScanDocument(document);
        return true;
      }
      catch
      {
        // fall through to legacy parser
      }
    }

    try
    {
      metrics = DxfEntitiesOnlyParser.Parse(filePath);
      return true;
    }
    catch (Exception ex)
    {
      errorMessage = ex.Message;
      return false;
    }
  }
}
