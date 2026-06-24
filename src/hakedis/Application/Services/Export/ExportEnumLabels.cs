using Domain.Enums;

namespace Application.Services.Export;

internal static class ExportEnumLabels
{
    public static string GetMetrajKalemLabel(MetrajKalemType value) =>
        value switch
        {
            MetrajKalemType.Duvar => "Duvar",
            MetrajKalemType.Siva => "Sıva",
            MetrajKalemType.Boya => "Boya",
            MetrajKalemType.DisCepheMantolama => "Dış Cephe Mantolama",
            MetrajKalemType.SapBeton => "Şap / Beton",
            MetrajKalemType.Kalip => "Kalıp",
            _ => value.ToString()
        };

    public static string GetMeasurementUnitLabel(MeasurementUnit value) =>
        value switch
        {
            MeasurementUnit.M2 => "m²",
            MeasurementUnit.M3 => "m³",
            MeasurementUnit.M => "m",
            MeasurementUnit.Kg => "kg",
            MeasurementUnit.Ton => "ton",
            MeasurementUnit.Adet => "adet",
            MeasurementUnit.Takim => "takım",
            _ => value.ToString()
        };

    public static string GetWorkTypeLabel(WorkType value) =>
        value switch
        {
            WorkType.Gunduz => "Gündüz",
            WorkType.Gece => "Gece",
            WorkType.HaftaSonu => "Hafta Sonu",
            WorkType.ResmiTatil => "Resmi Tatil",
            WorkType.Izin => "İzin",
            WorkType.Rapor => "Rapor",
            _ => value.ToString()
        };

    public static string GetPuantajStatusLabel(PuantajStatus value) =>
        value switch
        {
            PuantajStatus.Draft => "Taslak",
            PuantajStatus.Submitted => "Gönderildi",
            PuantajStatus.Approved => "Onaylandı",
            PuantajStatus.Rejected => "Reddedildi",
            _ => value.ToString()
        };

    public static string GetHakedisStatusLabel(HakedisStatus value) =>
        value switch
        {
            HakedisStatus.Draft => "Taslak",
            HakedisStatus.Submitted => "Gönderildi",
            HakedisStatus.Approved => "Onaylandı",
            HakedisStatus.Rejected => "Reddedildi",
            _ => value.ToString()
        };

    public static string GetProjectStatusLabel(ProjectStatus value) =>
        value switch
        {
            ProjectStatus.Active => "Aktif",
            ProjectStatus.Completed => "Tamamlandı",
            ProjectStatus.Suspended => "Askıda",
            _ => value.ToString()
        };
}
