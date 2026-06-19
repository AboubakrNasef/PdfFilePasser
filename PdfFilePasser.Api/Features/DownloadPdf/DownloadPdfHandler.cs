using PdfFilePasser.Api.Storage;
using PdfFilePasser.Api.Features.Common;

namespace PdfFilePasser.Api.Features.DownloadPdf;

public class DownloadPdfResponse
{
    public required Stream Stream { get; set; }
    public required string FileName { get; set; }
    public required string ContentType { get; set; }
}

public class DownloadPdfHandler(IStorage storage)
{
    public async Task<DownloadPdfResponse> Handle(string fileId, CancellationToken cancellation)
    {
        var blobs = await storage.ListBlobsAsync(cancellation);
        var blob = blobs.FirstOrDefault(b => b.Name.EndsWith($"_{fileId}.pdf"));

        if (blob == null)
            throw new PdfNotFoundException($"PDF with ID {fileId} not found");

        var stream = await blob.OpenReadAsync(cancellation);
        var properties = await blob.GetPropertiesAsync(cancellation);

        return new DownloadPdfResponse
        {
            Stream = stream,
            FileName = properties.TryGetValue("OriginalFileName", out var name) ? name : blob.Name,
            ContentType = properties.TryGetValue("ContentType", out var ct) ? ct : "application/pdf"
        };
    }
}
