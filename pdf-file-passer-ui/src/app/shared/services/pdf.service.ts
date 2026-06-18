import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { PdfFileInfo, UploadResponse } from '../models/pdf.model';

@Injectable({ providedIn: 'root' })
export class PdfService {
  private apiUrl = `${environment.apiUrl}/pdf`;

  constructor(private http: HttpClient) {}

  uploadPdf(file: File, description?: string) {
    const formData = new FormData();
    formData.append('file', file);
    if (description) formData.append('description', description);

    return this.http.post<UploadResponse>(`${this.apiUrl}/upload`, formData);
  }

  listPdfs() {
    return this.http.get<PdfFileInfo[]>(`${this.apiUrl}/list`);
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
