using FluentStorage;
using PdfFilePasser.Api.Features.PdfUpload;
using PdfFilePasser.Api.Features.DownloadPdf;
using PdfFilePasser.Api.Features.ListPdfs;
using PdfFilePasser.Api.Features.DeletePdf;

var builder = WebApplicationBuilder.CreateBuilder(args);

// Add services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddOpenApi();

// Register FluentStorage with Azure Blob Storage
var storageConnectionString = builder.Configuration["AzureStorage:ConnectionString"]
    ?? "UseDevelopmentStorage=true";

builder.Services.AddSingleton(sp =>
{
    var blobStorage = StorageFactory.Blobs.AzureBlobStorage(storageConnectionString);
    var containerName = builder.Configuration["AzureStorage:ContainerName"] ?? "pdffiles";

    // Ensure container exists
    try
    {
        blobStorage.CreateContainerAsync(containerName).Wait();
    }
    catch
    {
        // Container might already exist
    }

    return blobStorage[containerName];
});

// Register handlers as scoped
builder.Services.AddScoped<UploadPdfHandler>();
builder.Services.AddScoped<DownloadPdfHandler>();
builder.Services.AddScoped<ListPdfsHandler>();
builder.Services.AddScoped<DeletePdfHandler>();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseCors("AllowAngular");

// Map endpoints
app.MapUploadPdfEndpoint();
app.MapDownloadPdfEndpoint();
app.MapListPdfsEndpoint();
app.MapDeletePdfEndpoint();

app.Run();
