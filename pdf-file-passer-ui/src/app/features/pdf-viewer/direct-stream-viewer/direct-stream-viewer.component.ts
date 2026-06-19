import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { PdfService } from '../../../shared/services/pdf.service';
import { NgxExtendedPdfViewerModule } from 'ngx-extended-pdf-viewer';

@Component({
  selector: 'app-direct-stream-viewer',
  standalone: true,
  imports: [CommonModule, NgxExtendedPdfViewerModule],
  templateUrl: './direct-stream-viewer.component.html',
  styleUrl: './direct-stream-viewer.component.css'
})
export class DirectStreamViewerComponent implements OnInit {

  fileId: string | null = null;
  fileName: string | null = null;
  directStreamUrl: string | null = null;
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

    console.log('Starting to load direct stream PDF for fileId:', this.fileId);

    this.directStreamUrl = this.pdfService.getDirectStreamUrl(this.fileId);
    this.loading = false;
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
}
