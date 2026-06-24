namespace Application.Services.Export;

public class ExportFileResponse
{
    public byte[] FileContent { get; set; } = [];
    public string FileName { get; set; } = "export.xlsx";
    public string ContentType { get; set; } = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
}
