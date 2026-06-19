import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { signal } from '@angular/core';
import { PdfService } from '../../shared/services/pdf.service';
import { NgxExtendedPdfViewerModule } from 'ngx-extended-pdf-viewer';

@Component({
  selector: 'app-viewer',
  standalone: true,
  imports: [CommonModule, RouterLink, NgxExtendedPdfViewerModule],
  template: `
    <div class="viewer-container">
      <div class="viewer-header">
        <a routerLink="/list" class="back-link">← Back to PDFs</a>
        <h2>{{ fileName() }}</h2>
      </div>

      <div *ngIf="loading()" class="loading">
        <p>Loading PDF...</p>
      </div>

      <div *ngIf="!loading() && pdfUrl()" class="viewer-content">
        <ngx-extended-pdf-viewer
          [src]="pdfUrl()"
          [showBorders]="true"
        ></ngx-extended-pdf-viewer>
      </div>

      <div *ngIf="errorMessage()" class="error-message">
        {{ errorMessage() }}
        <a routerLink="/list" class="btn">Back to PDFs</a>
      </div>
    </div>
  `,
  styles: [`
    .viewer-container {
      padding: 2rem;
      max-width: 100%;
    }

    .viewer-header {
      margin-bottom: 2rem;
    }

    .back-link {
      display: inline-block;
      margin-bottom: 1rem;
      color: #3498db;
      text-decoration: none;
      transition: color 0.3s;
    }

    .back-link:hover {
      color: #2980b9;
    }

    .viewer-header h2 {
      color: #2c3e50;
      margin: 0;
      word-break: break-word;
    }

    .loading {
      text-align: center;
      padding: 2rem;
      color: #7f8c8d;
    }

    .viewer-content {
      background: white;
      border-radius: 8px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);
      padding: 1rem;
      height: 80vh;
      overflow: auto;
    }

    .error-message {
      padding: 2rem;
      background-color: #fadbd8;
      border-left: 4px solid #e74c3c;
      color: #c0392b;
      border-radius: 4px;
      text-align: center;
    }

    .btn {
      display: inline-block;
      margin-top: 1rem;
      padding: 0.75rem 1.5rem;
      background-color: #3498db;
      color: white;
      text-decoration: none;
      border-radius: 4px;
    }

    .btn:hover {
      background-color: #2980b9;
    }
  `]
})
export class ViewerComponent implements OnInit {
  fileId = signal<string | null>(null);
  fileName = signal('PDF Viewer');
  pdfUrl = signal<string >('');
  loading = signal(true);
  errorMessage = signal('');

  constructor(private route: ActivatedRoute, private pdfService: PdfService) {}

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.fileId.set(params['id']);
      if (this.fileId()) {
        this.loadPdf();
      }
    });
  }

  private loadPdf(): void {
    if (!this.fileId()) return;

    this.loading.set(true);
    this.pdfUrl.set(this.pdfService.getPdfUrl(this.fileId()!));
    this.loading.set(false);
  }
}
