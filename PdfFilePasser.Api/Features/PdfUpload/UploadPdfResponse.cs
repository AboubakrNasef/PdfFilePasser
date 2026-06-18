namespace PdfFilePasser.Api.Features.PdfUpload;

public class UploadPdfResponse
{
    public required string FileId { get; set; }
    public required string FileName { get; set; }
    public DateTime UploadedAt { get; set; }
    public long FileSizeBytes { get; set; }
}
