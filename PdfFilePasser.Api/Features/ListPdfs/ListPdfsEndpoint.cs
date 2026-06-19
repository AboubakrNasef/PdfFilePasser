namespace PdfFilePasser.Api.Features.ListPdfs;

public static class ListPdfsEndpoint
{
    public static void MapListPdfsEndpoint(this WebApplication app)
    {
        app.MapGet("/api/pdf/list", Handle)
            .WithName("ListPdfs")
            .WithOpenApi()
            .WithSummary("List all PDF files")
            .WithDescription("Retrieve a list of all uploaded PDF files with their metadata.")
            .Produces<IEnumerable<object>>(StatusCodes.Status200OK);
    }

    private static async Task<IResult> Handle(
        ListPdfsHandler handler,
        CancellationToken cancellation)
    {
        var pdfs = await handler.Handle(cancellation);
        return Results.Ok(pdfs);
    }
}
