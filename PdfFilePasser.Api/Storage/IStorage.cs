namespace PdfFilePasser.Api.Storage;

public interface IStorage
{
    Task<IBlobObject> WriteAsync(string blobName, Stream content, bool overwrite = true, CancellationToken cancellation = default);
    Task<List<IBlobObject>> ListBlobsAsync(CancellationToken cancellation = default);
    Task<List<IBlobObject>> ListFilesAsync(object? options = null, object? prefix = null, CancellationToken cancellation = default);
    Task DeleteAsync(string blobPath, CancellationToken cancellation = default);
}
