var builder = DistributedApplication.CreateBuilder(args);

var storage = builder
    .AddAzureStorage("storage")
    .RunAsEmulator()
    .AddBlobs("blobs");

var api = builder
    .AddProject<Projects.PdfFilePasser_Api>("api")
    .WithReference(storage)
    .WithEnvironment("AzureStorage__ConnectionString", storage)
    .WithEnvironment("AzureStorage__ContainerName", "pdffiles");

builder.Build().Run();
