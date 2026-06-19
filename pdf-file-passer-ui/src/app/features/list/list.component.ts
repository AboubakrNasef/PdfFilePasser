import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { signal } from '@angular/core';
import { PdfService } from '../../shared/services/pdf.service';
import { PdfFileInfo } from '../../shared/models/pdf.model';

@Component({
  selector: 'app-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="list-container">
      <h2>My PDFs</h2>

      <div *ngIf="pdfService.loading()" class="loading">
        <p>Loading your PDFs...</p>
      </div>

      <div *ngIf="!pdfService.loading() && pdfService.pdfs().length === 0" class="empty-state">
        <p>No PDFs uploaded yet.</p>
        <a routerLink="/upload" class="btn">Upload your first PDF</a>
      </div>

      <div *ngIf="!pdfService.loading() && pdfService.pdfs().length > 0" class="pdf-grid">
        <div *ngFor="let pdf of pdfService.pdfs()" class="pdf-card">
          <div class="pdf-header">
            <h3>{{ pdf.fileName }}</h3>
            <span class="file-size">{{ formatFileSize(pdf.fileSizeBytes) }}</span>
          </div>

          <div *ngIf="pdf.description" class="pdf-description">
            {{ pdf.description }}
          </div>

          <div class="pdf-meta">
            <span>{{ formatDate(pdf.uploadedAt) }}</span>
          </div>

          <div class="pdf-actions">
            <a
              [routerLink]="['/viewer', pdf.fileId]"
              class="btn btn-view"
            >
              View
            </a>
            <button
              (click)="downloadPdf(pdf)"
              class="btn btn-download"
            >
              Download
            </button>
            <button
              (click)="deletePdf(pdf.fileId)"
              class="btn btn-delete"
            >
              Delete
            </button>
          </div>
        </div>
      </div>

      <div *ngIf="pdfService.error()" class="error-message">
        {{ pdfService.error() }}
      </div>
    </div>
  `,
  styles: [`
    .list-container {
      padding: 2rem;
    }

    h2 {
      color: #2c3e50;
      margin-bottom: 2rem;
    }

    .loading, .empty-state {
      text-align: center;
      padding: 2rem;
      color: #7f8c8d;
    }

    .empty-state .btn {
      display: inline-block;
      margin-top: 1rem;
      padding: 0.75rem 1.5rem;
      background-color: #3498db;
      color: white;
      text-decoration: none;
      border-radius: 4px;
    }

    .pdf-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
      gap: 2rem;
    }

    .pdf-card {
      background: white;
      border-radius: 8px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);
      padding: 1.5rem;
      transition: transform 0.3s, box-shadow 0.3s;
    }

    .pdf-card:hover {
      transform: translateY(-4px);
      box-shadow: 0 4px 12px rgba(0,0,0,0.15);
    }

    .pdf-header {
      display: flex;
      justify-content: space-between;
      align-items: start;
      margin-bottom: 1rem;
    }

    .pdf-header h3 {
      margin: 0;
      color: #2c3e50;
      word-break: break-word;
      flex: 1;
    }

    .file-size {
      background-color: #ecf0f1;
      padding: 0.25rem 0.75rem;
      border-radius: 4px;
      font-size: 0.85rem;
      color: #7f8c8d;
      white-space: nowrap;
      margin-left: 0.5rem;
    }

    .pdf-description {
      color: #7f8c8d;
      font-size: 0.95rem;
      margin-bottom: 1rem;
      line-height: 1.4;
    }

    .pdf-meta {
      color: #95a5a6;
      font-size: 0.85rem;
      margin-bottom: 1rem;
    }

    .pdf-actions {
      display: flex;
      gap: 0.5rem;
      flex-wrap: wrap;
    }

    .btn {
      flex: 1;
      min-width: 80px;
      padding: 0.5rem;
      border: none;
      border-radius: 4px;
      cursor: pointer;
      font-size: 0.9rem;
      text-decoration: none;
      text-align: center;
      transition: background-color 0.3s;
    }

    .btn-view {
      background-color: #3498db;
      color: white;
    }

    .btn-view:hover {
      background-color: #2980b9;
    }

    .btn-download {
      background-color: #27ae60;
      color: white;
    }

    .btn-download:hover {
      background-color: #229954;
    }

    .btn-delete {
      background-color: #e74c3c;
      color: white;
    }

    .btn-delete:hover {
      background-color: #c0392b;
    }

    .error-message {
      padding: 1rem;
      margin-top: 2rem;
      background-color: #fadbd8;
      border-left: 4px solid #e74c3c;
      color: #c0392b;
      border-radius: 4px;
    }
  `]
})
export class ListComponent implements OnInit {
  errorMessage = signal('');

  constructor(public pdfService: PdfService) {}

  ngOnInit(): void {
    this.pdfService.loadPdfs();
  }

  downloadPdf(pdf: PdfFileInfo): void {
    this.pdfService.downloadPdf(pdf.fileId).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = pdf.fileName + '.pdf';
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: () => {
        this.errorMessage.set('Failed to download PDF.');
      }
    });
  }

  deletePdf(fileId: string): void {
    if (confirm('Are you sure you want to delete this PDF?')) {
      this.pdfService.deletePdf(fileId).subscribe({
        next: () => {
          this.pdfService.loadPdfs();
        },
        error: () => {
          this.errorMessage.set('Failed to delete PDF.');
        }
      });
    }
  }

  formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 B';
    const k = 1024;
    const sizes = ['B', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i];
  }

  formatDate(date: string | Date): string {
    const d = new Date(date);
    return d.toLocaleDateString() + ' ' + d.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
  }
}
