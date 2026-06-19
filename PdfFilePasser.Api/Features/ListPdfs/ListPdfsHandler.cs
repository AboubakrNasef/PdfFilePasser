using PdfFilePasser.Api.Storage;
using PdfFilePasser.Api.Features.Common;

namespace PdfFilePasser.Api.Features.ListPdfs;

public class ListPdfsHandler(IStorage storage, ILogger<ListPdfsHandler> logger)
{
    public async Task<IEnumerable<PdfFileInfo>> Handle(CancellationToken cancellation)
    {
        logger.LogInformation("Starting PDF list operation");

        try
        {
            logger.LogDebug("Fetching all blobs from storage");
            var blobs = await storage.ListFilesAsync(null, null, cancellation);
            logger.LogDebug("Retrieved {BlobCount} blobs", blobs.Count);

            var pdfs = new List<PdfFileInfo>();
            foreach (var blob in blobs)
            {
                try
                {
                    var props = await blob.GetPropertiesAsync(cancellation);
                    var fileId = ExtractFileId(blob.Name);

                    pdfs.Add(new PdfFileInfo
                    {
                        FileId = fileId,
                        FileName = props.TryGetValue("OriginalFileName", out var name) ? name : blob.Name,
                        UploadedAt = blob.Modified ?? DateTime.UtcNow,
                        FileSizeBytes = blob.Size ?? 0,
                        Description = props.TryGetValue("Description", out var desc) ? desc : null
                    });

                    logger.LogDebug("Processed blob: {BlobName}, FileId={FileId}, Size={Size}", blob.Name, fileId, blob.Size);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing blob: {BlobName}", blob.Name);
                }
            }

            var result = pdfs.OrderByDescending(p => p.UploadedAt).ToList();
            logger.LogInformation("PDF list completed successfully: TotalCount={TotalCount}", result.Count);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error listing PDFs");
            throw;
        }
    }

    private static string ExtractFileId(string blobName)
    {
        var parts = blobName.Split('_');
        return parts.Length >= 2 ? parts[^1].Replace(".pdf", "") : blobName;
    }
}
