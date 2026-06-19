using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage");
var blobs = storage.AddBlobs("blobs");
storage.AddBlobContainer("pdffiles");
// Run as the Azurite emulator in local development
if (builder.Environment.IsDevelopment())
{
	storage.RunAsEmulator();
}

var api = builder.AddProject<Projects.PdfFilePasser_Api>("Api")
	.WithReference(blobs)
	.WaitFor(blobs);

var ui = builder.AddJavaScriptApp("ui", "../../pdf-file-passer-ui","start")
	.WithReference(api);

builder.Build().Run();
