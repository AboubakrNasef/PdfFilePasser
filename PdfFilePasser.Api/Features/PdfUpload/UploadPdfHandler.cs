using PdfFilePasser.Api.Storage;

namespace PdfFilePasser.Api.Features.PdfUpload;

public class UploadPdfHandler(IStorage storage, ILogger<UploadPdfHandler> logger)
{
    public async Task<UploadPdfResponse> Handle(UploadPdfRequest request, CancellationToken cancellation)
    {
        var fileId = Guid.NewGuid().ToString();
        var fileName = Path.GetFileNameWithoutExtension(request.File.FileName);
        var blobName = $"{fileName}_{fileId}.pdf";

        logger.LogInformation("Starting PDF upload: FileId={FileId}, OriginalFileName={FileName}, FileSize={FileSize}",
            fileId, request.File.FileName, request.File.Length);

        try
        {
            var stream = request.File.OpenReadStream();
            logger.LogDebug("Stream opened for upload: {BlobName}", blobName);

            var blob = await storage.WriteAsync(blobName, stream, true, cancellation);
            logger.LogInformation("Blob written to storage: {BlobName}", blobName);

            var properties = new Dictionary<string, string>
            {
                { "OriginalFileName", request.File.FileName },
                { "ContentType", request.File.ContentType ?? "application/pdf" },
                { "Description", request.Description ?? string.Empty }
            };
            await blob.SetPropertiesAsync(properties, cancellation);
            logger.LogDebug("Metadata set for blob: {BlobName}", blobName);

            var response = new UploadPdfResponse
            {
                FileId = fileId,
                FileName = fileName,
                UploadedAt = DateTime.UtcNow,
                FileSizeBytes = request.File.Length
            };

            logger.LogInformation("PDF upload completed successfully: FileId={FileId}", fileId);
            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error uploading PDF: FileId={FileId}, OriginalFileName={FileName}", fileId, request.File.FileName);
            throw;
        }
    }
}
