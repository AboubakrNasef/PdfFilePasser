using PdfFilePasser.Api.Storage;

namespace PdfFilePasser.Api.Features.StreamPdf;

public static class StreamPdfEndpoint
{
	public static void MapStreamPdfEndpoint(this WebApplication app)
	{
		// Method 2: Memory Stream (downloads PDF to memory and returns as stream)
		app.MapGet("/api/pdf/{fileId}/memory-stream", HandleMemoryStream)
			.WithName("StreamPdfMemoryStream")
			.WithOpenApi()
			.WithSummary("Get PDF as memory stream")
			.WithDescription("Download PDF to memory stream and return it as bytes. Entire file is loaded into memory.")
			.RequireCors("AllowAngular")
			.Produces(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status404NotFound);

		// Method 3: Blob Storage Stream (streams the PDF file directly from blob storage)
		app.MapGet("/api/pdf/{fileId}/blob-stream", HandleBlobStorageStream)
			.WithName("StreamPdfDirect")
			.WithOpenApi()
			.WithSummary("Stream PDF file directly")
			.WithDescription("Stream PDF file directly as bytes. Use this URL directly in an iframe src.")
			.RequireCors("AllowAngular")
			.Produces(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status404NotFound);
	}

	private static async Task<IResult> HandleMemoryStream(
		string fileId,
		IStorage storage,
		CancellationToken cancellation)
	{
		try
		{
			var blobs = await storage.ListBlobsAsync(cancellation);
			var blob = blobs.FirstOrDefault(b => b.Name.EndsWith($"_{fileId}.pdf"));

			if (blob == null)
			{
				return Results.NotFound("PDF not found");
			}

			var properties = await blob.GetPropertiesAsync(cancellation);

			var fileName = properties.TryGetValue("OriginalFileName", out var name) ? name : blob.Name;
			var contentType = properties.TryGetValue("ContentType", out var ct) ? ct : "application/pdf";

			var bytes =await storage.GetBlobAsBytesAsync(blob.Name, cancellation);

			return Results.File(new MemoryStream(bytes), contentType, fileName, enableRangeProcessing: true);
		}
		catch (Exception ex)
		{

			return Results.StatusCode(StatusCodes.Status500InternalServerError);
		}
	}

	private static async Task<IResult> HandleBlobStorageStream(
		string fileId,
		IStorage storage,
		CancellationToken cancellation)
	{
		try
		{

			var blobs = await storage.ListBlobsAsync(cancellation);
			var blob = blobs.FirstOrDefault(b => b.Name.EndsWith($"_{fileId}.pdf"));

			if (blob == null)
			{
				return Results.NotFound("PDF not found");
			}

			var properties = await blob.GetPropertiesAsync(cancellation);

			var fileName = properties.TryGetValue("OriginalFileName", out var name) ? name : blob.Name;
			var contentType = properties.TryGetValue("ContentType", out var ct) ? ct : "application/pdf";


			var stream = await blob.OpenReadAsync(cancellation);

			return Results.File(stream, contentType, fileName, enableRangeProcessing: true);
		}
		catch (Exception ex)
		{
			return Results.StatusCode(StatusCodes.Status500InternalServerError);
		}
	}
}
