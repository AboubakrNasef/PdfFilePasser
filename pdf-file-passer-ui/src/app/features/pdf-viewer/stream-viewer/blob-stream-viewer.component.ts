import { Component, signal, effect } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';

import { PdfService } from '../../../shared/services/pdf.service';
import { NgxExtendedPdfViewerModule } from 'ngx-extended-pdf-viewer';

@Component({
  selector: 'app-blob-stream-viewer',
  standalone: true,
  imports: [CommonModule, NgxExtendedPdfViewerModule],
  templateUrl: './blob-stream-viewer.component.html',
  styleUrl: './blob-stream-viewer.component.css'
})
export class BlobStreamViewerComponent {

  fileId = signal<string | null>(null);
  fileName = signal<string | null>(null);
  streamUrl = signal<string | null>(null);
  loading = signal(false);
  error = signal<string | null>(null);

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private pdfService: PdfService
  ) {
    effect(() => {
      this.route.paramMap.subscribe(params => {
        const id = params.get('id');
        this.fileId.set(id);
        if (id) {
          this.loadPdf();
        }
      });
    });
  }

  loadPdf() {
    const id = this.fileId();
    if (!id) return;

    this.loading.set(true);
    this.error.set(null);
    this.streamUrl.set(null);

    console.log('Starting to load blob storage stream PDF for fileId:', id);

    this.streamUrl.set(this.pdfService.getBlobStreamUrl(id));
    this.fileName.set(id + '.pdf');
    this.loading.set(false);
  }

  downloadPdf() {
    const id = this.fileId();
    if (!id) return;

    console.log('Downloading PDF for fileId:', id);

    this.pdfService.downloadPdf(id).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = this.fileName() || `document-${id}.pdf`;
        link.click();
        window.URL.revokeObjectURL(url);
      },
      error: (error) => {
        console.error('Error downloading PDF:', error);
        this.error.set('Failed to download PDF');
      }
    });
  }

  goBack() {
    this.router.navigate(['/list']);
  }
}
