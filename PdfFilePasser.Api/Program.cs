using Azure.Storage.Blobs;
using PdfFilePasser.Api.Features.PdfUpload;
using PdfFilePasser.Api.Features.DownloadPdf;
using PdfFilePasser.Api.Features.ListPdfs;
using PdfFilePasser.Api.Features.DeletePdf;
using PdfFilePasser.Api.Storage;

var builder = WebApplication.CreateBuilder(args);

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

// Add Swagger/Swashbuckle
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "PDF File Passer API",
        Version = "v1",
        Description = "API for uploading, downloading, listing, and deleting PDF files"
    });
});

// Register Azure Blob Storage
var storageConnectionString = builder.Configuration["AzureStorage:ConnectionString"]
    ?? "UseDevelopmentStorage=true";
var containerName = builder.Configuration["AzureStorage:ContainerName"] ?? "pdffiles";

builder.Services.AddSingleton<IStorage>(sp =>
{
    var blobServiceClient = new BlobServiceClient(storageConnectionString);
    var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
    return new AzureBlobStorage(blobContainerClient);
});

// Register handlers as scoped
builder.Services.AddScoped<UploadPdfHandler>();
builder.Services.AddScoped<DownloadPdfHandler>();
builder.Services.AddScoped<ListPdfsHandler>();
builder.Services.AddScoped<DeletePdfHandler>();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "PDF File Passer API v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseCors("AllowAngular");

// Map endpoints
app.MapUploadPdfEndpoint();
app.MapDownloadPdfEndpoint();
app.MapListPdfsEndpoint();
app.MapDeletePdfEndpoint();

app.Run();
