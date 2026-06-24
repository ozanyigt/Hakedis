using Domain.Enums;

namespace Domain;

public static class MeasurementUnitDefaults
{
    public static MeasurementUnit GetDefaultForKalemType(MetrajKalemType kalemType) =>
        kalemType switch
        {
            MetrajKalemType.Duvar => MeasurementUnit.M2,
            MetrajKalemType.Siva => MeasurementUnit.M2,
            MetrajKalemType.Boya => MeasurementUnit.M2,
            MetrajKalemType.DisCepheMantolama => MeasurementUnit.M2,
            MetrajKalemType.SapBeton => MeasurementUnit.M2,
            MetrajKalemType.Kalip => MeasurementUnit.M2,
            _ => MeasurementUnit.M2
        };

    public static IReadOnlyList<MeasurementUnit> GetAllowedForKalemType(MetrajKalemType kalemType) =>
        kalemType switch
        {
            MetrajKalemType.SapBeton => [MeasurementUnit.M2, MeasurementUnit.M3],
            MetrajKalemType.Kalip => [MeasurementUnit.M2, MeasurementUnit.M3],
            _ => [MeasurementUnit.M2, MeasurementUnit.M3, MeasurementUnit.M, MeasurementUnit.Adet]
        };

    public static string GetLabel(MeasurementUnit unit) =>
        unit switch
        {
            MeasurementUnit.M2 => "m²",
            MeasurementUnit.M3 => "m³",
            MeasurementUnit.M => "m",
            MeasurementUnit.Kg => "kg",
            MeasurementUnit.Ton => "ton",
            MeasurementUnit.Adet => "adet",
            MeasurementUnit.Takim => "takım",
            _ => unit.ToString()
        };
}
