import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { PdfFileInfo, UploadResponse } from '../models/pdf.model';

@Injectable({ providedIn: 'root' })
export class PdfService {
  private apiUrl = `${environment.apiUrl}/pdf`;
  pdfs = signal<PdfFileInfo[]>([]);
  loading = signal(false);
  error = signal<string>('');

  constructor(private http: HttpClient) {
    console.log('PDF Service initialized with API URL:', this.apiUrl);
  }

  uploadPdf(file: File, description?: string) {
    const formData = new FormData();
    formData.append('file', file);
    if (description) formData.append('description', description);

    return this.http.post<UploadResponse>(`${this.apiUrl}/upload`, formData);
  }

  loadPdfs() {
    this.loading.set(true);
    this.error.set('');

    const apiUrl = `${this.apiUrl}/list`;
    console.log('Fetching PDFs from:', apiUrl);

    this.http.get<PdfFileInfo[]>(apiUrl).subscribe({
      next: (data) => {
        console.log('PDFs loaded successfully:', data.length, 'files');
        this.pdfs.set(data);
        this.loading.set(false);
      },
      error: (error) => {
        console.error('Error loading PDFs:', error);
        console.error('Status:', error.status);
        console.error('Message:', error.message);
        this.error.set(`Failed to load PDFs: ${error.status} ${error.statusText}`);
        this.loading.set(false);
      }
    });
  }

  downloadPdf(fileId: string) {
    return this.http.get(`${this.apiUrl}/${fileId}`, { responseType: 'blob' });
  }

  deletePdf(fileId: string) {
    return this.http.delete(`${this.apiUrl}/${fileId}`);
  }

  getPdfUrl(fileId: string) {
    return `${this.apiUrl}/${fileId}`;
  }
}
