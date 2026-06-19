using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;

namespace PdfFilePasser.Api.Storage;

public class AzureBlobObject : IBlobObject
{
    private readonly BlobClient _blobClient;
    private BlobProperties? _properties;

    public string Name { get; }
    public string FullPath { get; }
    public long? Size { get; private set; }
    public DateTimeOffset? Modified { get; private set; }

    public AzureBlobObject(BlobClient blobClient, string containerName)
    {
        _blobClient = blobClient;
        Name = blobClient.Name;
        FullPath = blobClient.Uri.ToString();
    }

    public async Task<Stream> OpenReadAsync(CancellationToken cancellation = default)
    {
        var download = await _blobClient.DownloadAsync(cancellation);
        return download.Value.Content;
    }

    public async Task<Stream> ReadAsStreamAsync(CancellationToken cancellation = default)
    {
        var download = await _blobClient.DownloadAsync(cancellation);
        return download.Value.Content;
    }

    public async Task<byte[]> ReadAsBytesAsync(CancellationToken cancellation = default)
    {
        var download = await _blobClient.DownloadAsync(cancellation);
        using var memoryStream = new MemoryStream();
        await download.Value.Content.CopyToAsync(memoryStream, cancellation);
        return memoryStream.ToArray();
    }

    public async Task DeleteAsync(CancellationToken cancellation = default)
    {
        await _blobClient.DeleteAsync(cancellationToken: cancellation);
    }

    public async Task<Dictionary<string, string>> GetPropertiesAsync(CancellationToken cancellation = default)
    {
        var properties = await _blobClient.GetPropertiesAsync(cancellationToken: cancellation);
        _properties = properties.Value;
        Size = properties.Value.ContentLength;
        Modified = properties.Value.LastModified;

        var metadata = new Dictionary<string, string>();
        foreach (var (key, value) in properties.Value.Metadata)
        {
            metadata[key] = value;
        }
        return metadata;
    }

    public async Task SetPropertiesAsync(Dictionary<string, string> properties, CancellationToken cancellation = default)
    {
        await _blobClient.SetMetadataAsync(properties, cancellationToken: cancellation);
    }
}
