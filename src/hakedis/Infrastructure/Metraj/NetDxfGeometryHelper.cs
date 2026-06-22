using netDxf;
using netDxf.Entities;

namespace Infrastructure.Metraj;

internal sealed class LayerEntityMetrics
{
  public double ClosedArea { get; set; }
  public double LineLength { get; set; }
  public int EntityCount { get; set; }
}

internal static class NetDxfGeometryHelper
{
  public static Dictionary<string, LayerEntityMetrics> ScanDocument(DxfDocument document)
  {
    Dictionary<string, LayerEntityMetrics> metrics = new(StringComparer.OrdinalIgnoreCase);

    foreach (EntityObject entity in document.Entities.All)
    {
      string layer = entity.Layer?.Name ?? "0";
      LayerEntityMetrics bucket = GetOrCreate(metrics, layer);
      bucket.EntityCount++;

      switch (entity)
      {
        case Line line:
          bucket.LineLength += Distance2D(line.StartPoint.X, line.StartPoint.Y, line.EndPoint.X, line.EndPoint.Y);
          break;
        case Polyline2D polyline2D:
          AccumulatePolyline(bucket, polyline2D.Vertexes.Select(v => (v.Position.X, v.Position.Y)).ToList(), polyline2D.IsClosed);
          break;
        case Hatch:
          // Hatch alanı sürüm farkları nedeniyle şimdilik atlanıyor.
          break;
      }
    }

    return metrics;
  }

  private static void AccumulatePolyline(LayerEntityMetrics bucket, IReadOnlyList<(double X, double Y)> points, bool isClosed)
  {
    if (points.Count < 2)
      return;

    double length = 0;
    for (int i = 0; i < points.Count - 1; i++)
      length += Distance2D(points[i].X, points[i].Y, points[i + 1].X, points[i + 1].Y);

    if (isClosed && points.Count > 2)
      length += Distance2D(points[^1].X, points[^1].Y, points[0].X, points[0].Y);

    bucket.LineLength += length;

    if (isClosed && points.Count >= 3)
      bucket.ClosedArea += PolygonArea(points);
  }

  private static double PolygonArea(IReadOnlyList<(double X, double Y)> points)
  {
    double area = 0;
    for (int i = 0; i < points.Count; i++)
    {
      (double x1, double y1) = points[i];
      (double x2, double y2) = points[(i + 1) % points.Count];
      area += x1 * y2 - x2 * y1;
    }

    return Math.Abs(area) * 0.5;
  }

  private static double Distance2D(double x1, double y1, double x2, double y2)
  {
    double dx = x2 - x1;
    double dy = y2 - y1;
    return Math.Sqrt(dx * dx + dy * dy);
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

  public static double SumForPatterns(
    Dictionary<string, LayerEntityMetrics> metrics,
    IEnumerable<string> patterns,
    Func<LayerEntityMetrics, double> selector
  )
  {
    double total = 0;
    foreach ((string layer, LayerEntityMetrics bucket) in metrics)
    {
      if (LayerPatternMatcher.IsMatch(layer, patterns))
        total += selector(bucket);
    }

    return total;
  }
}
