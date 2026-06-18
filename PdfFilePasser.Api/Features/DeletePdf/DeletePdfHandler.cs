using PdfFilePasser.Api.Features.Common;

namespace PdfFilePasser.Api.Features.DeletePdf;

public class DeletePdfHandler(IStorageFolder pdfFolder)
{
    public async Task Handle(string fileId, CancellationToken cancellation)
    {
        var files = await pdfFolder.ListFilesAsync(null, null, cancellation);
        var file = files.FirstOrDefault(f => f.Name.Contains(fileId));

        if (file == null)
            throw new PdfNotFoundException($"PDF with ID {fileId} not found");

        await file.DeleteAsync(cancellation);
    }
}
