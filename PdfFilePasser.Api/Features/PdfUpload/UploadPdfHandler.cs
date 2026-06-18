namespace PdfFilePasser.Api.Features.PdfUpload;

public class UploadPdfHandler(IStorageFolder pdfFolder)
{
    public async Task<UploadPdfResponse> Handle(UploadPdfRequest request, CancellationToken cancellation)
    {
        var fileId = Guid.NewGuid().ToString();
        var fileName = Path.GetFileNameWithoutExtension(request.File.FileName);
        var blobName = $"{fileName}_{fileId}.pdf";

        using var stream = request.File.OpenReadStream();

        // Upload using FluentStorage's simple fluent API
        var file = await pdfFolder.WriteAsync(blobName, stream, cancellation);

        // Set metadata properties
        var properties = new Dictionary<string, string>
        {
            { "OriginalFileName", request.File.FileName },
            { "ContentType", request.File.ContentType ?? "application/pdf" },
            { "Description", request.Description ?? string.Empty }
        };
        await file.SetPropertiesAsync(properties, cancellation);

        return new UploadPdfResponse
        {
            FileId = fileId,
            FileName = fileName,
            UploadedAt = DateTime.UtcNow,
            FileSizeBytes = request.File.Length
        };
    }
}
