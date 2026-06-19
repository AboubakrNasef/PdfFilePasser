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

  constructor(private http: HttpClient) {}

  uploadPdf(file: File, description?: string) {
    const formData = new FormData();
    formData.append('file', file);
    if (description) formData.append('description', description);

    return this.http.post<UploadResponse>(`${this.apiUrl}/upload`, formData);
  }

  loadPdfs() {
    this.loading.set(true);
    this.error.set('');

    this.http.get<PdfFileInfo[]>(`${this.apiUrl}/list`).subscribe({
      next: (data) => {
        this.pdfs.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load PDFs. Please try again.');
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
