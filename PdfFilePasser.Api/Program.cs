using Azure.Storage.Blobs;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using PdfFilePasser.Api.Features.PdfUpload;
using PdfFilePasser.Api.Features.DownloadPdf;
using PdfFilePasser.Api.Features.ListPdfs;
using PdfFilePasser.Api.Features.DeletePdf;
using PdfFilePasser.Api.Features.ViewPdf;
using PdfFilePasser.Api.Features.StreamPdf;
using PdfFilePasser.Api.Storage;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy
            .SetIsOriginAllowed(origin => true)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .WithExposedHeaders("Content-Type", "Content-Disposition");
    });
});

builder.Services.AddOpenApi();

// Configure Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Configure OpenTelemetry
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation(options =>
            {
                options.RecordException = true;
            })
            .AddHttpClientInstrumentation(options =>
            {
                options.RecordException = true;
            })
            .AddConsoleExporter();
    })
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddConsoleExporter();
    });

// Configure structured logging with OpenTelemetry
builder.Logging.AddOpenTelemetry(options =>
{
    options.AddConsoleExporter();
});

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
var storageConnectionString = builder.Configuration["BLOBS_CONNECTIONSTRING"]
    ?? "UseDevelopmentStorage=true";
 var containerName = builder.Configuration["AzureStorage:ContainerName"] ?? "pdffiles";

builder.Services.AddSingleton<IStorage>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<AzureBlobStorage>>();
    logger.LogInformation("Initializing Azure Blob Storage with connection string and container: {ContainerName}", containerName);

    var blobServiceClient = new BlobServiceClient(storageConnectionString);
    var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
    return new AzureBlobStorage(blobContainerClient);
});

// Register handlers as scoped
builder.Services.AddScoped<UploadPdfHandler>();
builder.Services.AddScoped<DownloadPdfHandler>();
builder.Services.AddScoped<ListPdfsHandler>();
builder.Services.AddScoped<DeletePdfHandler>();
builder.Services.AddScoped<ViewPdfHandler>();
builder.Services.AddScoped<StreamPdfHandler>();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

// Middleware - CORS must be before routing
app.UseCors("AllowAngular");

// Request logging middleware for CORS debugging
app.Use(async (context, next) =>
{
    var origin = context.Request.Headers.Origin.ToString();
    var method = context.Request.Method;
    var path = context.Request.Path;

    if (!string.IsNullOrEmpty(origin))
    {
        logger.LogInformation("Request: {Method} {Path} from Origin: {Origin}", method, path, origin);
    }

    await next();
});

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

// Map endpoints
app.MapUploadPdfEndpoint();
app.MapDownloadPdfEndpoint();
app.MapListPdfsEndpoint();
app.MapDeletePdfEndpoint();
app.MapViewPdfEndpoint();
app.MapStreamPdfEndpoint();
app.MapGet("/hello", () => "helloBack");
app.Run();
