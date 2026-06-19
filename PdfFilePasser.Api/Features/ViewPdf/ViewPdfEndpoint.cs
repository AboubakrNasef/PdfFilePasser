namespace PdfFilePasser.Api.Features.ViewPdf;

public static class ViewPdfEndpoint
{
    public static void MapViewPdfEndpoint(this WebApplication app)
    {
        app.MapGet("/api/pdf/{fileId}/view", Handle)
            .WithName("ViewPdf")
            .WithOpenApi()
            .WithSummary("View a PDF file (get bytes)")
            .WithDescription("Download PDF file as bytes for in-browser viewing. Returns PDF data that can be converted to blob URL.")
            .RequireCors("AllowAngular")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> Handle(
        string fileId,
        ViewPdfHandler handler,
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
}
