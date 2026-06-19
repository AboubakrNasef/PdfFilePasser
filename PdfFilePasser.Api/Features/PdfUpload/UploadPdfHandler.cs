using PdfFilePasser.Api.Storage;

namespace PdfFilePasser.Api.Features.PdfUpload;

public class UploadPdfHandler(IStorage storage)
{
    public async Task<UploadPdfResponse> Handle(UploadPdfRequest request, CancellationToken cancellation)
    {
        var fileId = Guid.NewGuid().ToString();
        var fileName = Path.GetFileNameWithoutExtension(request.File.FileName);
        var blobName = $"{fileName}_{fileId}.pdf";

        var stream = request.File.OpenReadStream();
        var blob = await storage.WriteAsync(blobName, stream, true, cancellation);

        var properties = new Dictionary<string, string>
        {
            { "OriginalFileName", request.File.FileName },
            { "ContentType", request.File.ContentType ?? "application/pdf" },
            { "Description", request.Description ?? string.Empty }
        };
        await blob.SetPropertiesAsync(properties, cancellation);

        return new UploadPdfResponse
        {
            FileId = fileId,
            FileName = fileName,
            UploadedAt = DateTime.UtcNow,
            FileSizeBytes = request.File.Length
        };
    }
}
