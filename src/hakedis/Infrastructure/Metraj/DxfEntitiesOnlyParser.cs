namespace Infrastructure.Metraj;

/// <summary>
/// netDxf/IxMilia desteklemediği eski DXF dosyaları için yalnızca ENTITIES bölümünü okur.
/// </summary>
internal static class DxfEntitiesOnlyParser
{
  public static Dictionary<string, LayerEntityMetrics> Parse(string filePath)
  {
    Dictionary<string, LayerEntityMetrics> metrics = new(StringComparer.OrdinalIgnoreCase);

    bool inEntities = false;
    string? entityType = null;
    string? layer = null;
    List<(double X, double Y)> vertices = [];
    bool isClosed = false;
    double? x1 = null, y1 = null, x2 = null, y2 = null;
    bool collectingPolylineVertices = false;
    string? polylineLayer = null;
    bool polylineClosed = false;

    using StreamReader reader = new(filePath);
    while (ReadPair(reader, out int code, out string value))
    {
      if (code == 0 && value == "SECTION")
      {
        if (ReadPair(reader, out int sectionCode, out string sectionName) && sectionCode == 2)
        {
          if (sectionName == "ENTITIES")
            inEntities = true;
          else if (inEntities)
            break;
        }

        continue;
      }

      if (!inEntities)
        continue;

      if (code == 0 && value == "ENDSEC")
        break;

      if (code == 0)
      {
        if (collectingPolylineVertices && entityType == "VERTEX")
        {
          if (polylineLayer is not null && vertices.Count >= 2)
            AddPolyline(metrics, polylineLayer, vertices, polylineClosed);
          vertices = [];
          collectingPolylineVertices = false;
        }

        FlushLine(metrics, entityType, layer, x1, y1, x2, y2);
        FlushLwPolyline(metrics, entityType, layer, vertices, isClosed);

        entityType = value;
        layer = null;
        vertices = [];
        isClosed = false;
        x1 = y1 = x2 = y2 = null;

        if (entityType == "POLYLINE")
        {
          collectingPolylineVertices = true;
          polylineLayer = null;
          polylineClosed = false;
          vertices = [];
        }
        else if (entityType == "SEQEND")
        {
          if (polylineLayer is not null && vertices.Count >= 2)
            AddPolyline(metrics, polylineLayer, vertices, polylineClosed);
          collectingPolylineVertices = false;
          vertices = [];
          entityType = null;
        }

        continue;
      }

      if (entityType is null)
        continue;

      if (entityType == "LINE")
      {
        switch (code)
        {
          case 8: layer = value; break;
          case 10: x1 = ParseDouble(value); break;
          case 20: y1 = ParseDouble(value); break;
          case 11: x2 = ParseDouble(value); break;
          case 21: y2 = ParseDouble(value); break;
        }
      }
      else if (entityType == "LWPOLYLINE")
      {
        switch (code)
        {
          case 8: layer = value; break;
          case 70: isClosed = (ParseInt(value) & 1) == 1; break;
          case 10: vertices.Add((ParseDouble(value), 0)); break;
          case 20:
            if (vertices.Count > 0)
            {
              (double lastX, _) = vertices[^1];
              vertices[^1] = (lastX, ParseDouble(value));
            }
            break;
        }
      }
      else if (collectingPolylineVertices)
      {
        switch (code)
        {
          case 8 when entityType == "POLYLINE" || entityType == "VERTEX":
            if (entityType == "POLYLINE")
              polylineLayer = value;
            else
              layer = value;
            break;
          case 70 when entityType == "POLYLINE":
            polylineClosed = (ParseInt(value) & 1) == 1;
            break;
          case 10 when entityType == "VERTEX":
            vertices.Add((ParseDouble(value), 0));
            break;
          case 20 when entityType == "VERTEX":
            if (vertices.Count > 0)
            {
              (double lastX, _) = vertices[^1];
              vertices[^1] = (lastX, ParseDouble(value));
            }
            break;
        }
      }
    }

    if (collectingPolylineVertices && polylineLayer is not null && vertices.Count >= 2)
      AddPolyline(metrics, polylineLayer, vertices, polylineClosed);

    FlushLine(metrics, entityType, layer, x1, y1, x2, y2);
    FlushLwPolyline(metrics, entityType, layer, vertices, isClosed);

    return metrics;
  }

  public static (double lengthToMeters, double areaToSquareMeters, string note) GetScaleFromInsUnits(int insUnits) =>
    insUnits switch
    {
      4 => (0.001, 1e-6, "Çizim birimi mm kabul edildi (eski DXF)."),
      5 => (0.01, 1e-4, "Çizim birimi cm kabul edildi (eski DXF)."),
      6 => (1.0, 1.0, "Çizim birimi m kabul edildi (eski DXF)."),
      1 => (0.0254, 0.0254 * 0.0254, "Çizim birimi inch kabul edildi (eski DXF)."),
      2 => (0.3048, 0.3048 * 0.3048, "Çizim birimi feet kabul edildi (eski DXF)."),
      _ => (0.001, 1e-6, "Birim tanımsız; mm varsayıldı (eski DXF).")
    };

  private static int ReadInsUnits(string filePath)
  {
    using StreamReader reader = new(filePath);
    bool inHeader = false;
    while (ReadPair(reader, out int code, out string value))
    {
      if (code == 0 && value == "SECTION")
      {
        if (ReadPair(reader, out int sectionCode, out string sectionName) && sectionCode == 2)
          inHeader = sectionName == "HEADER";
        continue;
      }

      if (inHeader && code == 9 && value == "$INSUNITS")
      {
        if (ReadPair(reader, out int unitCode, out string unitValue) && unitCode == 70)
          return ParseInt(unitValue);
      }

      if (inHeader && code == 0 && value == "ENDSEC")
        break;
    }

    return 0;
  }

  public static int GetInsUnits(string filePath) => ReadInsUnits(filePath);

  private static void FlushLine(
    Dictionary<string, LayerEntityMetrics> metrics,
    string? entityType,
    string? layer,
    double? x1,
    double? y1,
    double? x2,
    double? y2
  )
  {
    if (entityType != "LINE" || string.IsNullOrWhiteSpace(layer))
      return;
    if (!x1.HasValue || !y1.HasValue || !x2.HasValue || !y2.HasValue)
      return;

    LayerEntityMetrics bucket = GetOrCreate(metrics, layer);
    bucket.LineLength += Distance(x1.Value, y1.Value, x2.Value, y2.Value);
    bucket.EntityCount++;
  }

  private static void FlushLwPolyline(
    Dictionary<string, LayerEntityMetrics> metrics,
    string? entityType,
    string? layer,
    List<(double X, double Y)> vertices,
    bool isClosed
  )
  {
    if (entityType != "LWPOLYLINE" || string.IsNullOrWhiteSpace(layer) || vertices.Count < 2)
      return;

    AddPolyline(metrics, layer, vertices, isClosed);
  }

  private static void AddPolyline(
    Dictionary<string, LayerEntityMetrics> metrics,
    string layer,
    IReadOnlyList<(double X, double Y)> points,
    bool isClosed
  )
  {
    LayerEntityMetrics bucket = GetOrCreate(metrics, layer);
    AccumulatePolyline(bucket, points, isClosed);
    bucket.EntityCount++;
  }

  private static void AccumulatePolyline(
    LayerEntityMetrics bucket,
    IReadOnlyList<(double X, double Y)> points,
    bool isClosed
  )
  {
    if (points.Count < 2)
      return;

    double length = 0;
    for (int i = 0; i < points.Count - 1; i++)
      length += Distance(points[i].X, points[i].Y, points[i + 1].X, points[i + 1].Y);

    if (isClosed && points.Count > 2)
      length += Distance(points[^1].X, points[^1].Y, points[0].X, points[0].Y);

    bucket.LineLength += length;

    if (isClosed && points.Count >= 3)
    {
      double area = 0;
      for (int i = 0; i < points.Count; i++)
      {
        (double px1, double py1) = points[i];
        (double px2, double py2) = points[(i + 1) % points.Count];
        area += px1 * py2 - px2 * py1;
      }

      bucket.ClosedArea += Math.Abs(area) * 0.5;
    }
  }

  private static bool ReadPair(StreamReader reader, out int code, out string value)
  {
    code = 0;
    value = string.Empty;

    string? codeLine = reader.ReadLine();
    if (codeLine is null)
      return false;

    string? valueLine = reader.ReadLine();
    if (valueLine is null)
      return false;

    if (!int.TryParse(codeLine.Trim(), out code))
      return false;

    value = valueLine.Trim();
    return true;
  }

  private static LayerEntityMetrics GetOrCreate(Dictionary<string, LayerEntityMetrics> metrics, string layer)
  {
    if (!metrics.TryGetValue(layer, out LayerEntityMetrics? bucket))
    {
      bucket = new LayerEntityMetrics();
      metrics[layer] = bucket;
    }

    return bucket;
  }

  private static double Distance(double x1, double y1, double x2, double y2)
  {
    double dx = x2 - x1;
    double dy = y2 - y1;
    return Math.Sqrt(dx * dx + dy * dy);
  }

  private static double ParseDouble(string value) =>
    double.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double result)
      ? result
      : 0;

  private static int ParseInt(string value) =>
    int.TryParse(value, out int result) ? result : 0;
}
