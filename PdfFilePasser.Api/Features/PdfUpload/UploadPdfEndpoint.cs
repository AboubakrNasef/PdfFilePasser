namespace PdfFilePasser.Api.Features.PdfUpload;

public static class UploadPdfEndpoint
{
    public static void MapUploadPdfEndpoint(this WebApplication app)
    {
        app.MapPost("/api/pdf/upload", Handle)
            .WithName("UploadPdf")
            .WithOpenApi()
            .WithSummary("Upload a PDF file")
            .WithDescription("Upload a new PDF file to the storage. Returns file ID, name, upload timestamp, and file size.")
            .Produces<UploadPdfResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Accepts<UploadPdfRequest>("multipart/form-data");
    }

    private static async Task<IResult> Handle(
        HttpRequest request,
        UploadPdfHandler handler,
        CancellationToken cancellation)
    {
        var form = await request.ReadFormAsync(cancellation);
        var file = form.Files["file"];

        if (file == null || file.Length == 0)
            return Results.BadRequest("No file uploaded");

        if (!file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            return Results.BadRequest("Only PDF files are allowed");

        var uploadRequest = new UploadPdfRequest
        {
            File = file,
            Description = form["description"]
        };

        var response = await handler.Handle(uploadRequest, cancellation);
        return Results.Ok(response);
    }
}
