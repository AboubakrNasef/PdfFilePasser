using PdfFilePasser.Api.Storage;
using PdfFilePasser.Api.Features.Common;

namespace PdfFilePasser.Api.Features.ViewPdf;

public class ViewPdfHandler(IStorage storage, ILogger<ViewPdfHandler> logger)
{
    public async Task<ViewPdfResponse> Handle(string fileId, CancellationToken cancellation)
    {
        logger.LogInformation("Starting PDF view (download bytes): FileId={FileId}", fileId);

        try
        {
            logger.LogDebug("Listing blobs to find FileId={FileId}", fileId);
            var blobs = await storage.ListBlobsAsync(cancellation);
            var blob = blobs.FirstOrDefault(b => b.Name.EndsWith($"_{fileId}.pdf"));

            if (blob == null)
            {
                logger.LogWarning("PDF not found for viewing: FileId={FileId}", fileId);
                throw new PdfNotFoundException($"PDF with ID {fileId} not found");
            }

            logger.LogDebug("Found blob: {BlobName}, Size={Size}", blob.Name, blob.Size);
            var pdfBytes = await blob.ReadAsBytesAsync(cancellation);
            var properties = await blob.GetPropertiesAsync(cancellation);

            logger.LogInformation("PDF view prepared successfully: FileId={FileId}, BlobName={BlobName}, BytesSize={BytesSize}",
                fileId, blob.Name, pdfBytes.Length);

            return new ViewPdfResponse
            {
                PdfBytes = pdfBytes,
                FileName = properties.TryGetValue("OriginalFileName", out var name) ? name : blob.Name,
                ContentType = properties.TryGetValue("ContentType", out var ct) ? ct : "application/pdf"
            };
        }
        catch (PdfNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error viewing PDF: FileId={FileId}", fileId);
            throw;
        }
    }
}
