using PdfFilePasser.Api.Storage;
using PdfFilePasser.Api.Features.Common;

namespace PdfFilePasser.Api.Features.ListPdfs;

public class ListPdfsHandler(IStorage storage)
{
    public async Task<IEnumerable<PdfFileInfo>> Handle(CancellationToken cancellation)
    {
        var blobs = await storage.ListFilesAsync(null, null, cancellation);

        var pdfs = new List<PdfFileInfo>();
        foreach (var blob in blobs)
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
        }

        return pdfs.OrderByDescending(p => p.UploadedAt);
    }

    private static string ExtractFileId(string blobName)
    {
        var parts = blobName.Split('_');
        return parts.Length >= 2 ? parts[^1].Replace(".pdf", "") : blobName;
    }
}
