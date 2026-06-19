using PdfFilePasser.Api.Storage;

namespace PdfFilePasser.Api.Features.StreamPdf;

public static class StreamPdfEndpoint
{
    public static void MapStreamPdfEndpoint(this WebApplication app)
    {
        // Method 2: Stream URL (returns metadata with direct blob URL)
        app.MapGet("/api/pdf/{fileId}/stream-url", HandleStreamUrl)
            .WithName("StreamPdfUrl")
            .WithOpenApi()
            .WithSummary("Get PDF stream URL")
            .WithDescription("Get the stream URL for a PDF file. Use this URL directly in an iframe.")
            .RequireCors("AllowAngular")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        // Method 3: Direct PDF Stream (streams the PDF file directly)
        app.MapGet("/api/pdf/{fileId}/stream", HandleDirectStream)
            .WithName("StreamPdfDirect")
            .WithOpenApi()
            .WithSummary("Stream PDF file directly")
            .WithDescription("Stream PDF file directly as bytes. Use this URL directly in an iframe src.")
            .RequireCors("AllowAngular")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleStreamUrl(
        string fileId,
        StreamPdfHandler handler,
        CancellationToken cancellation)
    {
        try
        {
            var response = await handler.Handle(fileId, cancellation);
            return Results.Ok(response);
        }
        catch (Exception ex) when (ex.Message.Contains("not found"))
        {
            return Results.NotFound("PDF not found");
        }
    }

    private static async Task<IResult> HandleDirectStream(
        string fileId,
        IStorage storage,
        ILogger<StreamPdfHandler> logger,
        CancellationToken cancellation)
    {
        try
        {
            logger.LogInformation("Starting direct PDF stream: FileId={FileId}", fileId);

            var blobs = await storage.ListBlobsAsync(cancellation);
            var blob = blobs.FirstOrDefault(b => b.Name.EndsWith($"_{fileId}.pdf"));

            if (blob == null)
            {
                logger.LogWarning("PDF not found for direct streaming: FileId={FileId}", fileId);
                return Results.NotFound("PDF not found");
            }

            logger.LogDebug("Found blob: {BlobName}, Size={Size}", blob.Name, blob.Size);
            var properties = await blob.GetPropertiesAsync(cancellation);

            var fileName = properties.TryGetValue("OriginalFileName", out var name) ? name : blob.Name;
            var contentType = properties.TryGetValue("ContentType", out var ct) ? ct : "application/pdf";

            logger.LogInformation("Streaming PDF directly: FileId={FileId}, BlobName={BlobName}, Size={Size}",
                fileId, blob.Name, blob.Size);

            var stream = await blob.OpenReadAsync(cancellation);

            return Results.File(stream, contentType, fileName, enableRangeProcessing: true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in direct PDF stream: FileId={FileId}", fileId);
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
