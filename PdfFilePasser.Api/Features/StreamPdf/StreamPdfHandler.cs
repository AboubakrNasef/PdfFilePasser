using PdfFilePasser.Api.Storage;
using PdfFilePasser.Api.Features.Common;

namespace PdfFilePasser.Api.Features.StreamPdf;

public class StreamPdfHandler(IStorage storage, ILogger<StreamPdfHandler> logger)
{
    public async Task<StreamPdfResponse> Handle(string fileId, CancellationToken cancellation)
    {
        logger.LogInformation("Starting PDF stream: FileId={FileId}", fileId);

        try
        {
            logger.LogDebug("Listing blobs to find FileId={FileId}", fileId);
            var blobs = await storage.ListBlobsAsync(cancellation);
            var blob = blobs.FirstOrDefault(b => b.Name.EndsWith($"_{fileId}.pdf"));

            if (blob == null)
            {
                logger.LogWarning("PDF not found for streaming: FileId={FileId}", fileId);
                throw new PdfNotFoundException($"PDF with ID {fileId} not found");
            }

            logger.LogDebug("Found blob: {BlobName}, Size={Size}", blob.Name, blob.Size);
            var properties = await blob.GetPropertiesAsync(cancellation);

            logger.LogInformation("PDF stream prepared: FileId={FileId}, BlobName={BlobName}, FullPath={FullPath}",
                fileId, blob.Name, blob.FullPath);

            return new StreamPdfResponse
            {
                StreamUrl = blob.FullPath,
                FileName = properties.TryGetValue("OriginalFileName", out var name) ? name : blob.Name,
                FileSize = blob.Size ?? 0,
                ContentType = properties.TryGetValue("ContentType", out var ct) ? ct : "application/pdf"
            };
        }
        catch (PdfNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error streaming PDF: FileId={FileId}", fileId);
            throw;
        }
    }
}
