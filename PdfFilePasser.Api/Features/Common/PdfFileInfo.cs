namespace PdfFilePasser.Api.Features.Common;

public class PdfFileInfo
{
    public required string FileId { get; set; }
    public required string FileName { get; set; }
    public DateTimeOffset? UploadedAt { get; set; }
    public long FileSizeBytes { get; set; }
    public string? Description { get; set; }
}
