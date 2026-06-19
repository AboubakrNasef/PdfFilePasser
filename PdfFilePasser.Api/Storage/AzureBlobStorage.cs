using Azure.Storage.Blobs;

namespace PdfFilePasser.Api.Storage;

public class AzureBlobStorage : IStorage
{
    private readonly BlobContainerClient _containerClient;

    public AzureBlobStorage(BlobContainerClient containerClient)
    {
        _containerClient = containerClient;
    }

    public async Task<IBlobObject> WriteAsync(string blobName, Stream content, bool overwrite = true, CancellationToken cancellation = default)
    {
        var blobClient = _containerClient.GetBlockBlobClient(blobName);
        await blobClient.UploadAsync(content, overwrite: overwrite, cancellationToken: cancellation);
        return new AzureBlobObject(blobClient, _containerClient.Name);
    }

    public async Task<List<IBlobObject>> ListBlobsAsync(CancellationToken cancellation = default)
    {
        var blobs = new List<IBlobObject>();
        await foreach (var blobItem in _containerClient.GetBlobsAsync(cancellationToken: cancellation))
        {
            var blobClient = _containerClient.GetBlockBlobClient(blobItem.Name);
            blobs.Add(new AzureBlobObject(blobClient, _containerClient.Name));
        }
        return blobs;
    }

    public async Task<List<IBlobObject>> ListFilesAsync(object? options = null, object? prefix = null, CancellationToken cancellation = default)
    {
        return await ListBlobsAsync(cancellation);
    }

    public async Task DeleteAsync(string blobPath, CancellationToken cancellation = default)
    {
        var blobName = ExtractBlobName(blobPath);
        var blobClient = _containerClient.GetBlockBlobClient(blobName);
        await blobClient.DeleteAsync(cancellation: cancellation);
    }

    private string ExtractBlobName(string blobPath)
    {
        if (blobPath.Contains('/'))
        {
            return blobPath.Substring(blobPath.LastIndexOf('/') + 1);
        }
        return blobPath;
    }
}
