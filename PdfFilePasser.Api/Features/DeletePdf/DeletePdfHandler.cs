using PdfFilePasser.Api.Storage;
using PdfFilePasser.Api.Features.Common;

namespace PdfFilePasser.Api.Features.DeletePdf;

public class DeletePdfHandler(IStorage storage, ILogger<DeletePdfHandler> logger)
{
    public async Task Handle(string fileId, CancellationToken cancellation)
    {
        logger.LogInformation("Starting PDF deletion: FileId={FileId}", fileId);

        try
        {
            logger.LogDebug("Listing files to find FileId={FileId}", fileId);
            var blobs = await storage.ListFilesAsync(null, null, cancellation);
            var blob = blobs.FirstOrDefault(b => b.Name.EndsWith($"_{fileId}.pdf"));

            if (blob == null)
            {
                logger.LogWarning("PDF not found for deletion: FileId={FileId}", fileId);
                throw new PdfNotFoundException($"PDF with ID {fileId} not found");
            }

            logger.LogDebug("Deleting blob: {BlobName}", blob.Name);
            await blob.DeleteAsync(cancellation);

            logger.LogInformation("PDF deleted successfully: FileId={FileId}, BlobName={BlobName}", fileId, blob.Name);
        }
        catch (PdfNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting PDF: FileId={FileId}", fileId);
            throw;
        }
    }
}
