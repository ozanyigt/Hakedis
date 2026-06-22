using netDxf;
using netDxf.Units;

namespace Infrastructure.Metraj;

internal static class DrawingUnitConverter
{
  public static (double lengthToMeters, double areaToSquareMeters, string note) GetScale(DxfDocument document)
  {
    return document.DrawingVariables.InsUnits switch
    {
      DrawingUnits.Millimeters => (0.001, 1e-6, "Çizim birimi mm kabul edildi."),
      DrawingUnits.Centimeters => (0.01, 1e-4, "Çizim birimi cm kabul edildi."),
      DrawingUnits.Meters => (1.0, 1.0, "Çizim birimi m kabul edildi."),
      DrawingUnits.Inches => (0.0254, 0.0254 * 0.0254, "Çizim birimi inch kabul edildi."),
      DrawingUnits.Feet => (0.3048, 0.3048 * 0.3048, "Çizim birimi feet kabul edildi."),
      _ => (0.001, 1e-6, "Birim tanımsız; mm varsayıldı.")
    };
  }
}
