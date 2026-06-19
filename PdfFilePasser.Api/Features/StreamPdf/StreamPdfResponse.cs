namespace PdfFilePasser.Api.Features.StreamPdf;

public class StreamPdfResponse
{
    public required string StreamUrl { get; set; }
    public required string FileName { get; set; }
    public required long FileSize { get; set; }
    public required string ContentType { get; set; }
}
