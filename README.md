# PDF File Passer

A modern, cloud-native application for uploading, storing, and retrieving PDF files using .NET, Angular, and Azure Blob Storage with local Aspire orchestration.

## Architecture Overview

### Complete System Architecture
```
┌──────────────────────────────────────────────────────────────────┐
│                      Client Browser                              │
└───────────────────────────┬──────────────────────────────────────┘
                            │ HTTP/REST
                            │
      ┌─────────────────────▼──────────────────────┐
      │    Angular Frontend (Standalone Routes)    │
      │  ┌──────────────────────────────────────┐ │
      │  │  Route: /                            │ │
      │  │  Home Component (Dashboard)          │ │
      │  └──────────────────────────────────────┘ │
      │  ┌──────────────────────────────────────┐ │
      │  │  Route: /upload                      │ │
      │  │  Upload Component                    │ │
      │  │  - File input + Progress             │ │
      │  └──────────────────────────────────────┘ │
      │  ┌──────────────────────────────────────┐ │
      │  │  Route: /list                        │ │
      │  │  List Component                      │ │
      │  │  - PDF Gallery with Delete buttons   │ │
      │  └──────────────────────────────────────┘ │
      │  ┌──────────────────────────────────────┐ │
      │  │  Route: /viewer/:id                  │ │
      │  │  Viewer Component                    │ │
      │  │  - ngx-extended-pdf-viewer           │ │
      │  │  - PDF controls (zoom, nav, etc)     │ │
      │  └──────────────────────────────────────┘ │
      │                                            │
      │  ┌──────────────────────────────────────┐ │
      │  │  Services                            │ │
      │  │  - PdfService (HTTP calls)           │ │
      │  │  - Api Interceptor                   │ │
      │  └──────────────────────────────────────┘ │
      └─────────────────────┬──────────────────────┘
                            │ API Calls (REST/JSON)
                            │
      ┌─────────────────────▼──────────────────────┐
      │   .NET 9 API Server (SVA Pattern)          │
      │                                            │
      │  ╔════════════════════════════════════╗   │
      │  ║   VERTICAL SLICE FEATURES          ║   │
      │  ╠════════════════════════════════════╣   │
      │  ║                                    ║   │
      │  ║  PdfUpload/                        ║   │
      │  ║  ├─ UploadPdfRequest               ║   │
      │  ║  ├─ UploadPdfHandler               ║   │
      │  ║  └─ UploadPdfEndpoint              ║   │
      │  ║       POST /api/pdf/upload         ║   │
      │  ║                                    ║   │
      │  ║  DownloadPdf/                      ║   │
      │  ║  ├─ DownloadPdfHandler             ║   │
      │  ║  └─ DownloadPdfEndpoint            ║   │
      │  ║       GET /api/pdf/{id}            ║   │
      │  ║                                    ║   │
      │  ║  ListPdfs/                         ║   │
      │  ║  ├─ ListPdfsHandler                ║   │
      │  ║  └─ ListPdfsEndpoint               ║   │
      │  ║       GET /api/pdf/list            ║   │
      │  ║                                    ║   │
      │  ║  DeletePdf/                        ║   │
      │  ║  ├─ DeletePdfHandler               ║   │
      │  ║  └─ DeletePdfEndpoint              ║   │
      │  ║       DELETE /api/pdf/{id}         ║   │
      │  ║                                    ║   │
      │  ╚════════════════════════════════════╝   │
      │                                            │
      │  ┌──────────────────────────────────────┐ │
      │  │  Infrastructure Layer                │ │
      │  │  - FluentStorage (IStorageFolder)    │ │
      │  │  - Azure Blob provider configured   │ │
      │  └──────────────────────────────────────┘ │
      │                                            │
      │  ┌──────────────────────────────────────┐ │
      │  │  Shared                              │ │
      │  │  - PdfErrors, PdfConstants           │ │
      │  │  - CORS Middleware                   │ │
      │  └──────────────────────────────────────┘ │
      └─────────────────────┬──────────────────────┘
                            │ FluentStorage (Azure Provider)
                            │
      ┌─────────────────────▼──────────────────────┐
      │      Azurite Emulator                      │
      │   (Local Azure Blob Storage)               │
      │                                            │
      │  ┌──────────────────────────────────────┐ │
      │  │  Blob Container: pdffiles            │ │
      │  │  ├─ pdfs/{fileName}_{timestamp}.pdf  │ │
      │  │  ├─ Metadata & Properties            │ │
      │  │  └─ Soft-delete support              │ │
      │  └──────────────────────────────────────┘ │
      └──────────────────────────────────────────┘
```

## Technology Stack

### Backend
- **Framework**: ASP.NET Core 9
- **Language**: C#
- **API Pattern**: REST with Controllers
- **Storage**: Azure Blob Storage (emulated with Azurite)
- **Orchestration**: .NET Aspire

### Frontend
- **Framework**: Angular (latest)
- **Language**: TypeScript
- **PDF Viewer**: ngx-extended-pdf-viewer
- **HTTP Client**: Angular HttpClient
- **UI Framework**: Optional (Bootstrap/Material)

### Infrastructure
- **Local Storage Emulator**: Azurite
- **Orchestration**: .NET Aspire (AppHost)
- **Port Configuration**: 
  - Angular Frontend: `http://localhost:4200`
  - .NET API: `http://localhost:5000`
  - Azurite: `http://localhost:10000` (Blob Service)

## Project Structure

### Backend - Vertical Slice Architecture (SVA)

```
PdfFilePasser/
├── README.md                           # This file
├── PdfFilePasser.AppHost/              # Aspire AppHost project
│   ├── Program.cs                      # Aspire orchestration setup
│   └── appsettings.json                # Configuration
├── PdfFilePasser.Api/                  # .NET API Server (SVA Pattern)
│   ├── Features/
│   │   ├── PdfUpload/                  # Upload Feature Slice
│   │   │   ├── UploadPdfRequest.cs     # Request DTO
│   │   │   ├── UploadPdfResponse.cs    # Response DTO
│   │   │   ├── UploadPdfHandler.cs     # Business Logic/Handler
│   │   │   └── UploadPdfEndpoint.cs    # Minimal API Endpoint
│   │   │
│   │   ├── DownloadPdf/                # Download Feature Slice
│   │   │   ├── DownloadPdfRequest.cs
│   │   │   ├── DownloadPdfResponse.cs
│   │   │   ├── DownloadPdfHandler.cs
│   │   │   └── DownloadPdfEndpoint.cs
│   │   │
│   │   ├── ListPdfs/                   # List Feature Slice
│   │   │   ├── ListPdfsRequest.cs
│   │   │   ├── PdfFileInfo.cs          # Domain Model
│   │   │   ├── ListPdfsHandler.cs
│   │   │   └── ListPdfsEndpoint.cs
│   │   │
│   │   ├── DeletePdf/                  # Delete Feature Slice
│   │   │   ├── DeletePdfRequest.cs
│   │   │   ├── DeletePdfHandler.cs
│   │   │   └── DeletePdfEndpoint.cs
│   │   │
│   │   └── Common/                     # Shared across slices
│   │       ├── PdfErrors.cs            # Domain exceptions
│   │       └── PdfConstants.cs
│   │
│   ├── Infrastructure/
│   │   └── Configuration/
│   │       └── FluentStorageConfiguration.cs  # FluentStorage setup
│   │
│   │
│   ├── Program.cs                      # API configuration & DI setup
│   └── appsettings.json
```

### Frontend - Angular with Routing

```
└── pdf-file-passer-ui/                 # Angular Frontend
    ├── src/
    │   ├── app/
    │   │   ├── app.routes.ts           # Route definitions
    │   │   ├── app.component.ts        # Root component
    │   │   ├── app.component.html      # Root template with router-outlet
    │   │   │
    │   │   ├── shared/                 # Shared across routes
    │   │   │   ├── services/
    │   │   │   │   └── pdf.service.ts  # HTTP service for API
    │   │   │   ├── models/
    │   │   │   │   └── pdf.model.ts
    │   │   │   └── layouts/
    │   │   │       └── main-layout/
    │   │   │
    │   │   ├── features/
    │   │   │   ├── upload/             # Upload Feature Module
    │   │   │   │   ├── upload.routes.ts
    │   │   │   │   ├── upload.component.ts
    │   │   │   │   └── upload.component.html
    │   │   │   │
    │   │   │   ├── viewer/             # Viewer Feature Module
    │   │   │   │   ├── viewer.routes.ts
    │   │   │   │   ├── viewer.component.ts
    │   │   │   │   └── viewer.component.html
    │   │   │   │
    │   │   │   ├── list/               # List Feature Module
    │   │   │   │   ├── list.routes.ts
    │   │   │   │   ├── list.component.ts
    │   │   │   │   └── list.component.html
    │   │   │   │
    │   │   │   └── home/               # Home Feature Module
    │   │   │       ├── home.routes.ts
    │   │   │       ├── home.component.ts
    │   │   │       └── home.component.html
    │   │   │
    │   │   └── core/                   # Core module (singletons)
    │   │       └── interceptors/
    │   │           └── api.interceptor.ts
    │   │
    │   ├── environments/
    │   │   ├── environment.ts
    │   │   └── environment.prod.ts
    │   │
    │   ├── main.ts
    │   ├── index.html
    │   └── styles.scss
    │
    ├── angular.json
    ├── tsconfig.json
    └── package.json
```


## Backend Architecture - Vertical Slice Architecture (SVA)

The backend uses **Vertical Slice Architecture** which organizes code by **feature** rather than by technical layer.

### Key Principles
- **One Feature = One Slice** - Each feature (Upload, Download, List, Delete) is self-contained
- **Minimal API Endpoints** - Lightweight endpoint definitions in each slice
- **Handlers** - Business logic encapsulated in handler classes
- **Domain Models** - Domain-specific types in shared Common folder
- **Infrastructure Isolation** - Storage/external concerns isolated in Infrastructure layer

### Feature Slice Example (Upload)
```
PdfUpload/
├── UploadPdfRequest.cs      // Input model
├── UploadPdfResponse.cs     // Output model
├── UploadPdfHandler.cs      // Contains business logic
└── UploadPdfEndpoint.cs     // Maps HTTP route → Handler
```

Each slice is **vertically organized** - containing everything needed to implement that feature from endpoint to business logic.

---

## Frontend Routing Architecture

### Route Structure
```
/                           → Home (Dashboard)
├── /upload                 → PDF Upload Page
├── /list                   → PDF List/Gallery
└── /viewer/:id             → PDF Viewer Page
```

### Routing Configuration (app.routes.ts)
```typescript
export const routes: Routes = [
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      { path: '', component: HomeComponent },
      { path: 'upload', component: UploadComponent },
      { path: 'list', component: ListComponent },
      { path: 'viewer/:id', component: ViewerComponent },
    ]
  }
];
```

### Navigation Flow
```
Home → [Upload Button] → Upload Page
     → [View PDFs] → List Page → [Click PDF] → Viewer
```

---

## Key Features

### PDF Management
- **Upload** - Multi-file upload support with progress tracking (Route: `/upload`)
- **View** - Embedded PDF viewer with navigation and zoom controls (Route: `/viewer/:id`)
- **List** - Display all uploaded PDFs with metadata (Route: `/list`)
- **Delete** - Remove PDFs from storage (from List page)

### API Endpoints

| Method | Endpoint | Description | Feature Slice |
|--------|----------|-------------|---|
| POST | `/api/pdf/upload` | Upload a PDF file | PdfUpload |
| GET | `/api/pdf/{id}` | Download a specific PDF | DownloadPdf |
| GET | `/api/pdf/list` | List all uploaded PDFs | ListPdfs |
| DELETE | `/api/pdf/{id}` | Delete a PDF file | DeletePdf |

### Storage
- Blob naming convention: `pdfs/{fileName}_{timestamp}.pdf`
- Metadata storage in blob properties
- Container: `pdffiles`

## Development Workflow

### Prerequisites
- .NET 9 SDK
- Node.js 18+
- Docker (for Azurite) or Azurite installed locally

### Setup Steps
1. Clone the repository
2. Restore .NET dependencies
3. Install Angular dependencies
4. Configure Azurite connection string
5. Run Aspire orchestration

### Running the Application
```bash
# Using Aspire (recommended)
cd PdfFilePasser.AppHost
dotnet run

# Or individually:
# Terminal 1 - Start Azurite
azurite

# Terminal 2 - Start .NET API
cd PdfFilePasser.Api
dotnet run

# Terminal 3 - Start Angular
cd pdf-file-passer-ui
ng serve
```

## Implementation Details

### Backend - Vertical Slice Architecture

#### Feature Slice Structure (Upload Example)

**UploadPdfRequest.cs** - Input contract
```csharp
namespace PdfFilePasser.Api.Features.PdfUpload;

public class UploadPdfRequest
{
    public required IFormFile File { get; set; }
    public string? Description { get; set; }
}
```

**UploadPdfResponse.cs** - Output contract
```csharp
namespace PdfFilePasser.Api.Features.PdfUpload;

public class UploadPdfResponse
{
    public string FileId { get; set; }
    public string FileName { get; set; }
    public DateTime UploadedAt { get; set; }
    public long FileSizeBytes { get; set; }
}
```

**UploadPdfHandler.cs** - Business logic
```csharp
namespace PdfFilePasser.Api.Features.PdfUpload;

public class UploadPdfHandler(IStorageFolder pdfFolder)
{
    public async Task<UploadPdfResponse> Handle(UploadPdfRequest request, CancellationToken cancellation)
    {
        var fileId = Guid.NewGuid().ToString();
        var fileName = Path.GetFileNameWithoutExtension(request.File.FileName);
        var blobName = $"{fileName}_{fileId}.pdf";
        
        using var stream = request.File.OpenReadStream();
        
        // Upload using FluentStorage's simple fluent API
        var file = await pdfFolder.WriteAsync(blobName, stream, cancellation);
        
        // Set metadata properties
        var properties = new Dictionary<string, string>
        {
            { "OriginalFileName", request.File.FileName },
            { "ContentType", request.File.ContentType },
            { "Description", request.Description ?? string.Empty }
        };
        await file.SetPropertiesAsync(properties, cancellation);
        
        return new UploadPdfResponse
        {
            FileId = fileId,
            FileName = fileName,
            UploadedAt = DateTime.UtcNow,
            FileSizeBytes = request.File.Length
        };
    }
}
```

**UploadPdfEndpoint.cs** - HTTP mapping (Minimal API)
```csharp
namespace PdfFilePasser.Api.Features.PdfUpload;

public static class UploadPdfEndpoint
{
    public static void MapUploadPdfEndpoint(this WebApplication app)
    {
        app.MapPost("/api/pdf/upload", Handle)
            .WithName("UploadPdf")
            .WithOpenApi()
            .Produces<UploadPdfResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Accepts<UploadPdfRequest>("multipart/form-data");
    }

    private static async Task<IResult> Handle(
        HttpRequest request,
        IStorageFolder pdfFolder,
        UploadPdfHandler handler,
        CancellationToken cancellation)
    {
        var form = await request.ReadFormAsync(cancellation);
        var file = form.Files["file"];
        
        if (file == null || file.Length == 0)
            return Results.BadRequest("No file uploaded");

        if (!file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            return Results.BadRequest("Only PDF files are allowed");

        var uploadRequest = new UploadPdfRequest 
        { 
            File = file,
            Description = form["description"]
        };
        
        var response = await handler.Handle(uploadRequest, cancellation);
        return Results.Ok(response);
    }
}
```

### Program.cs - Service Registration with FluentStorage
```csharp
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
// Using development storage (Azurite) for local development
var storageConnectionString = builder.Configuration["AzureStorage:ConnectionString"] 
    ?? "UseDevelopmentStorage=true";

builder.Services.AddSingleton(sp =>
{
    var blobStorage = StorageFactory.Blobs.AzureBlobStorage(storageConnectionString);
    var containerName = builder.Configuration["AzureStorage:ContainerName"] ?? "pdffiles";
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
```

**Other Feature Handlers** - Download, List, Delete

```csharp
// DownloadPdfHandler.cs
namespace PdfFilePasser.Api.Features.DownloadPdf;

public class DownloadPdfHandler(IStorageFolder pdfFolder)
{
    public async Task<DownloadPdfResponse> Handle(string fileId, CancellationToken cancellation)
    {
        var files = await pdfFolder.ListFilesAsync(null, null, cancellation);
        var file = files.FirstOrDefault(f => f.Name.Contains(fileId));
        
        if (file == null)
            throw new PdfNotFoundException($"PDF with ID {fileId} not found");

        var stream = await file.OpenReadAsync(cancellation);
        var properties = await file.GetPropertiesAsync(cancellation);
        
        return new DownloadPdfResponse
        {
            Stream = stream,
            FileName = properties["OriginalFileName"] ?? file.Name,
            ContentType = properties.TryGetValue("ContentType", out var ct) ? ct : "application/pdf"
        };
    }
}

// ListPdfsHandler.cs
namespace PdfFilePasser.Api.Features.ListPdfs;

public class ListPdfsHandler(IStorageFolder pdfFolder)
{
    public async Task<IEnumerable<PdfFileInfo>> Handle(CancellationToken cancellation)
    {
        var files = await pdfFolder.ListFilesAsync(null, null, cancellation);
        
        var pdfs = new List<PdfFileInfo>();
        foreach (var file in files)
        {
            var props = await file.GetPropertiesAsync(cancellation);
            var fileId = ExtractFileId(file.Name);
            
            pdfs.Add(new PdfFileInfo
            {
                FileId = fileId,
                FileName = props.TryGetValue("OriginalFileName", out var name) ? name : file.Name,
                UploadedAt = file.Modified ?? DateTime.UtcNow,
                FileSizeBytes = file.Size ?? 0,
                Description = props.TryGetValue("Description", out var desc) ? desc : null
            });
        }
        
        return pdfs.OrderByDescending(p => p.UploadedAt);
    }

    private static string ExtractFileId(string blobName)
    {
        var parts = blobName.Split('_');
        return parts.Length >= 2 ? parts[^1].Replace(".pdf", "") : blobName;
    }
}

// DeletePdfHandler.cs
namespace PdfFilePasser.Api.Features.DeletePdf;

public class DeletePdfHandler(IStorageFolder pdfFolder)
{
    public async Task Handle(string fileId, CancellationToken cancellation)
    {
        var files = await pdfFolder.ListFilesAsync(null, null, cancellation);
        var file = files.FirstOrDefault(f => f.Name.Contains(fileId));
        
        if (file == null)
            throw new PdfNotFoundException($"PDF with ID {fileId} not found");

        await file.DeleteAsync(cancellation);
    }
}

// Common/PdfErrors.cs
namespace PdfFilePasser.Api.Features.Common;

public class PdfNotFoundException : Exception
{
    public PdfNotFoundException(string message) : base(message) { }
}

public class PdfValidationException : Exception
{
    public PdfValidationException(string message) : base(message) { }
}
```

---

### Frontend - Angular Routing

**app.routes.ts** - Standalone routing configuration
```typescript
import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home.component';
import { UploadComponent } from './features/upload/upload.component';
import { ListComponent } from './features/list/list.component';
import { ViewerComponent } from './features/viewer/viewer.component';
import { MainLayoutComponent } from './shared/layouts/main-layout/main-layout.component';

export const routes: Routes = [
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      { path: '', component: HomeComponent },
      { path: 'upload', component: UploadComponent },
      { path: 'list', component: ListComponent },
      { path: 'viewer/:id', component: ViewerComponent },
      { path: '**', redirectTo: '' }
    ]
  }
];
```

**main.ts** - Application bootstrap with routing
```typescript
import { bootstrapApplication } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { AppComponent } from './app/app.component';
import { routes } from './app/app.routes';

bootstrapApplication(AppComponent, {
  providers: [
    provideRouter(routes),
    provideHttpClient()
  ]
}).catch(err => console.error(err));
```

**app.component.html** - Root component with router outlet
```html
<div class="app-container">
  <nav class="navbar">
    <a routerLink="/">Home</a>
    <a routerLink="/upload">Upload PDF</a>
    <a routerLink="/list">My PDFs</a>
  </nav>
  
  <main class="content">
    <router-outlet></router-outlet>
  </main>
</div>
```

**pdf.service.ts** - HTTP client service
```typescript
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class PdfService {
  private apiUrl = `${environment.apiUrl}/pdf`;

  constructor(private http: HttpClient) {}

  uploadPdf(file: File, description?: string) {
    const formData = new FormData();
    formData.append('file', file);
    if (description) formData.append('description', description);
    
    return this.http.post<any>(`${this.apiUrl}/upload`, formData);
  }

  listPdfs() {
    return this.http.get<any[]>(`${this.apiUrl}/list`);
  }

  downloadPdf(fileId: string) {
    return this.http.get(`${this.apiUrl}/${fileId}`, { 
      responseType: 'blob' 
    });
  }

  deletePdf(fileId: string) {
    return this.http.delete(`${this.apiUrl}/${fileId}`);
  }
}
```

---

## Configuration

### .NET API (appsettings.json)
```json
{
  "AzureStorage": {
    "ConnectionString": "UseDevelopmentStorage=true",
    "ContainerName": "pdffiles"
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:4200"]
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Angular (environment.ts)
```typescript
export const environment = {
  apiUrl: 'http://localhost:5000/api',
  production: false
};
```

### Angular (environment.prod.ts)
```typescript
export const environment = {
  apiUrl: 'https://api.pdffiler.com/api',
  production: true
};
```

---

## FluentStorage Integration

### Why FluentStorage?
- **Fluent API** - Simpler, more intuitive than direct Azure SDK calls
- **Provider-Agnostic** - Easily switch storage providers if needed
- **Abstraction** - IStorageFolder/IStorageFile interfaces for better testability
- **Less Boilerplate** - No need for custom wrapper services

### Connection Strings

**Development (Azurite)**
```
UseDevelopmentStorage=true
```

**Production (Azure)**
```
DefaultEndpointsProtocol=https;AccountName=youraccountname;AccountKey=youraccountkey;EndpointSuffix=core.windows.net
```

### FluentStorage with Aspire

When using Aspire, you can configure FluentStorage in the AppHost:

```csharp
// PdfFilePasser.AppHost/Program.cs
var builder = DistributedApplication.CreateBuilder(args);

var storage = builder
    .AddAzureStorage("storage")
    .RunAsEmulator()  // Uses Azurite
    .AddBlobs("blobs");

builder
    .AddProject<Projects.PdfFilePasser_Api>("api")
    .WithReference(storage)
    .WithEnvironment("AzureStorage__ContainerName", "pdffiles");

builder.Build().Run();
```

This automatically:
- Starts Azurite emulator
- Injects connection strings
- Handles container creation
- Provides service discovery

### Key FluentStorage Methods
```csharp
// Reading files from a folder
var files = await folder.ListFilesAsync();

// Writing/uploading
var file = await folder.WriteAsync(name, stream);

// Deleting
await file.DeleteAsync();

// Getting file metadata
var properties = await file.GetPropertiesAsync();

// Reading file content
var stream = await file.OpenReadAsync();

// Setting custom properties/metadata
await file.SetPropertiesAsync(new Dictionary<string, string> { ... });
```

## Next Steps

- [ ] Initialize Aspire AppHost project
- [ ] Create .NET API project structure
- [ ] Implement BlobStorageService
- [ ] Create API Controllers and endpoints
- [ ] Initialize Angular frontend
- [ ] Implement PDF upload component
- [ ] Implement PDF viewer component
- [ ] Implement PDF list component
- [ ] Configure CORS in .NET
- [ ] Set up Azurite configuration
- [ ] Test end-to-end workflow
- [ ] Add error handling and validation
- [ ] Add authentication (optional)
- [ ] Add logging and monitoring

## Dependencies Overview

### Backend NuGet Packages
- `FluentStorage.Azure` - FluentStorage Azure Blob Storage provider (fluent API wrapper)
- `Aspire.*` - .NET Aspire packages for orchestration
- `Microsoft.AspNetCore.OpenApi` - OpenAPI/Swagger support

### Frontend NPM Packages
- `@angular/core` - Angular framework
- `ngx-extended-pdf-viewer` - PDF viewer component
- `@angular/common/http` - HTTP client

## Notes

- Azurite emulates Azure Blob Storage locally for development
- Aspire provides unified orchestration and service discovery
- CORS is configured to allow Angular frontend to communicate with .NET API
- All PDFs stored with unique timestamps to prevent conflicts
