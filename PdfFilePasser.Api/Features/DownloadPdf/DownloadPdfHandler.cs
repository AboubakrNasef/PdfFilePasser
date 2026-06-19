using PdfFilePasser.Api.Storage;
using PdfFilePasser.Api.Features.Common;

namespace PdfFilePasser.Api.Features.DownloadPdf;

public class DownloadPdfResponse
{
    public required Stream Stream { get; set; }
    public required string FileName { get; set; }
    public required string ContentType { get; set; }
}

public class DownloadPdfHandler(IStorage storage, ILogger<DownloadPdfHandler> logger)
{
    public async Task<DownloadPdfResponse> Handle(string fileId, CancellationToken cancellation)
    {
        logger.LogInformation("Starting PDF download: FileId={FileId}", fileId);

        try
        {
            logger.LogDebug("Listing blobs to find FileId={FileId}", fileId);
            var blobs = await storage.ListBlobsAsync(cancellation);
            var blob = blobs.FirstOrDefault(b => b.Name.EndsWith($"_{fileId}.pdf"));

            if (blob == null)
            {
                logger.LogWarning("PDF not found: FileId={FileId}", fileId);
                throw new PdfNotFoundException($"PDF with ID {fileId} not found");
            }

            logger.LogDebug("Found blob: {BlobName}, Size={Size}", blob.Name, blob.Size);
            var stream = await blob.OpenReadAsync(cancellation);
            var properties = await blob.GetPropertiesAsync(cancellation);

            var response = new DownloadPdfResponse
            {
                Stream = stream,
                FileName = properties.TryGetValue("OriginalFileName", out var name) ? name : blob.Name,
                ContentType = properties.TryGetValue("ContentType", out var ct) ? ct : "application/pdf"
            };

            logger.LogInformation("PDF download prepared successfully: FileId={FileId}, BlobName={BlobName}", fileId, blob.Name);
            return response;
        }
        catch (PdfNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error downloading PDF: FileId={FileId}", fileId);
            throw;
        }
    }
}
