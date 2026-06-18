using PdfFilePasser.Api.Features.Common;

namespace PdfFilePasser.Api.Features.ListPdfs;

public class ListPdfsHandler(IStorageFolder pdfFolder)
{
    public async Task<IEnumerable<PdfFileInfo>> Handle(CancellationToken cancellation)
    {
        var files = await pdfFolder.ListFilesAsync(null, null, cancellation);

        var pdfs = new List<PdfFileInfo>();
        foreach (var file in files)
        {
            var props = await file.GetPropertiesAsync(cancellation);
            var fileId = ExtractFileId(file.Name);

            pdfs.Add(new PdfFileInfo
            {
                FileId = fileId,
                FileName = props.TryGetValue("OriginalFileName", out var name) ? name : file.Name,
                UploadedAt = file.Modified ?? DateTime.UtcNow,
                FileSizeBytes = file.Size ?? 0,
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
