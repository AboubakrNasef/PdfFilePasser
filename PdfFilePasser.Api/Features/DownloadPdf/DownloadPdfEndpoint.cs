namespace PdfFilePasser.Api.Features.DownloadPdf;

public static class DownloadPdfEndpoint
{
    public static void MapDownloadPdfEndpoint(this WebApplication app)
    {
        app.MapGet("/api/pdf/{fileId}", Handle)
            .WithName("DownloadPdf")
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> Handle(
        string fileId,
        DownloadPdfHandler handler,
        CancellationToken cancellation)
    {
        try
        {
            var response = await handler.Handle(fileId, cancellation);
            return Results.File(response.Stream, response.ContentType, response.FileName);
        }
        catch (Exception ex) when (ex.Message.Contains("not found"))
        {
            return Results.NotFound("PDF not found");
        }
    }
}
