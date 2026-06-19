using PdfFilePasser.Api.Storage;
using PdfFilePasser.Api.Features.Common;

namespace PdfFilePasser.Api.Features.DeletePdf;

public class DeletePdfHandler(IStorage storage)
{
    public async Task Handle(string fileId, CancellationToken cancellation)
    {
        var blobs = await storage.ListFilesAsync(null, null, cancellation);
        var blob = blobs.FirstOrDefault(b => b.Name.EndsWith($"_{fileId}.pdf"));

        if (blob == null)
            throw new PdfNotFoundException($"PDF with ID {fileId} not found");

        await blob.DeleteAsync(cancellation);
    }
}
