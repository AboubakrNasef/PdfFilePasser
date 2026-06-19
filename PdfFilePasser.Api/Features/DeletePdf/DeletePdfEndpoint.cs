namespace PdfFilePasser.Api.Features.DeletePdf;

public static class DeletePdfEndpoint
{
    public static void MapDeletePdfEndpoint(this WebApplication app)
    {
        app.MapDelete("/api/pdf/{fileId}", Handle)
            .WithName("DeletePdf")
            .WithOpenApi()
            .WithSummary("Delete a PDF file")
            .WithDescription("Delete a PDF file by its file ID.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> Handle(
        string fileId,
        DeletePdfHandler handler,
        CancellationToken cancellation)
    {
        try
        {
            await handler.Handle(fileId, cancellation);
            return Results.NoContent();
        }
        catch (Exception ex) when (ex.Message.Contains("not found"))
        {
            return Results.NotFound("PDF not found");
        }
    }
}
