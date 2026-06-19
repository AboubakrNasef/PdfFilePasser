namespace PdfFilePasser.Api.Features.ViewPdf;

public class ViewPdfResponse
{
    public required byte[] PdfBytes { get; set; }
    public required string FileName { get; set; }
    public required string ContentType { get; set; }
}
