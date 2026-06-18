using PdfFilePasser.Api.Features.Common;

namespace PdfFilePasser.Api.Features.DownloadPdf;

public class DownloadPdfResponse
{
    public required Stream Stream { get; set; }
    public required string FileName { get; set; }
    public required string ContentType { get; set; }
}

public class DownloadPdfHandler(IStorageFolder pdfFolder)
{
    public async Task<DownloadPdfResponse> Handle(string fileId, CancellationToken cancellation)
    {
        var files = await pdfFolder.ListFilesAsync(null, null, cancellation);
        var file = files.FirstOrDefault(f => f.Name.Contains(fileId));

        if (file == null)
            throw new PdfNotFoundException($"PDF with ID {fileId} not found");

        var stream = await file.OpenReadAsync(cancellation);
        var properties = await file.GetPropertiesAsync(cancellation);

        return new DownloadPdfResponse
        {
            Stream = stream,
            FileName = properties.TryGetValue("OriginalFileName", out var name) ? name : file.Name,
            ContentType = properties.TryGetValue("ContentType", out var ct) ? ct : "application/pdf"
        };
    }
}
