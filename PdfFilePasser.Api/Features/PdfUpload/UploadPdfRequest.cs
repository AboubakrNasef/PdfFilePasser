namespace PdfFilePasser.Api.Features.PdfUpload;

public class UploadPdfRequest
{
    public required IFormFile File { get; set; }
    public string? Description { get; set; }
}
