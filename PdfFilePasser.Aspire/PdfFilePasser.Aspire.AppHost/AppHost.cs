using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage");
var blobs = storage.AddBlobs("blobs");

// Run as the Azurite emulator in local development
if (builder.Environment.IsDevelopment())
{
	storage.RunAsEmulator();
}

var api = builder.AddProject<Projects.PdfFilePasser_Api>("Api")
	.WithReference(blobs)
	.WaitFor(blobs);

var ui = builder.AddViteApp("ui", "../../pdf-file-passer-ui")
	.WithCommand("start")
	.WithHttpEndpoint(port: 4200, env: "PORT")
	.WithReference(api);

builder.Build().Run();
