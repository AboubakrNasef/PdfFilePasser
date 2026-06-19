import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NgxExtendedPdfViewerModule } from 'ngx-extended-pdf-viewer';
import { PdfService } from '../../../shared/services/pdf.service';

@Component({
  selector: 'app-blob-viewer',
  standalone: true,
  imports: [CommonModule, NgxExtendedPdfViewerModule],
  templateUrl: './blob-viewer.component.html',
  styleUrl: './blob-viewer.component.css'
})
export class BlobViewerComponent implements OnInit {

  fileId: string | null = null;
  fileName: string | null = null;
  pdfUrl: string | null = null;
  loading = false;
  error: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private pdfService: PdfService
  ) {}

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      this.fileId = params.get('fileId');
      if (this.fileId) {
        this.loadPdf();
      }
    });
  }

  loadPdf() {
    if (!this.fileId) return;

    this.loading = true;
    this.error = null;
    this.pdfUrl = null;

    console.log('Starting to load PDF for fileId:', this.fileId);

    this.pdfService.viewPdf(this.fileId).subscribe({
      next: (response) => {
        console.log('PDF data received, size:', response.pdfBytes?.length || 0);
        this.fileName = response.fileName;

        const pdfData = typeof response.pdfBytes === 'string'
          ? this.base64ToBytes(response.pdfBytes)
          : response.pdfBytes;

        const blob = new Blob([pdfData], { type: 'application/pdf' });
        this.pdfUrl = URL.createObjectURL(blob);

        console.log('PDF blob URL created:', this.pdfUrl);
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading PDF:', error);
        this.error = `Failed to load PDF: ${error.status} ${error.statusText}`;
        this.loading = false;
      }
    });
  }

  downloadPdf() {
    if (!this.fileId) return;

    console.log('Downloading PDF for fileId:', this.fileId);

    this.pdfService.downloadPdf(this.fileId).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = this.fileName || `document-${this.fileId}.pdf`;
        link.click();
        window.URL.revokeObjectURL(url);
      },
      error: (error) => {
        console.error('Error downloading PDF:', error);
        this.error = 'Failed to download PDF';
      }
    });
  }

  goBack() {
    this.router.navigate(['/list']);
  }

  private base64ToBytes(base64: string): Uint8Array {
    const binaryString = atob(base64);
    const bytes = new Uint8Array(binaryString.length);
    for (let i = 0; i < binaryString.length; i++) {
      bytes[i] = binaryString.charCodeAt(i);
    }
    return bytes;
  }
}
