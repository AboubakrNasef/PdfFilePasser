namespace PdfFilePasser.Api.Storage;

public interface IBlobObject
{
    string Name { get; }
    string FullPath { get; }
    long? Size { get; }
    DateTimeOffset? Modified { get; }

    Task<Stream> OpenReadAsync(CancellationToken cancellation = default);
    Task DeleteAsync(CancellationToken cancellation = default);
    Task<Dictionary<string, string>> GetPropertiesAsync(CancellationToken cancellation = default);
    Task SetPropertiesAsync(Dictionary<string, string> properties, CancellationToken cancellation = default);
}
