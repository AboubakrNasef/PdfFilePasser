using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage")
	.RunAsEmulator(azurite =>
	{
		azurite.WithBlobPort(27000)
			   .WithQueuePort(27001)
			   .WithTablePort(27002);
	}).AddBlobContainer("blobs","pdffiles");



var api = builder.AddProject<Projects.PdfFilePasser_Api>("Api")
	.WithReference(storage)
	.WaitFor(storage);

var ui = builder.AddJavaScriptApp("ui", "../../pdf-file-passer-ui","start")
	.WithReference(api).WaitFor(api);

builder.Build().Run();
