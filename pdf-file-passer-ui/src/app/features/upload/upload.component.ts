import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PdfService } from '../../shared/services/pdf.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-upload',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="upload-container">
      <h2>Upload PDF</h2>

      <div class="upload-box">
        <input
          #fileInput
          type="file"
          accept=".pdf"
          (change)="onFileSelected($event)"
          class="file-input"
        />
        <label class="file-label" (click)="fileInput.click()">
          <span class="upload-icon">📄</span>
          <span *ngIf="!selectedFile" class="upload-text">Click to select a PDF file</span>
          <span *ngIf="selectedFile" class="upload-text">{{ selectedFile.name }}</span>
        </label>
      </div>

      <div class="form-group">
        <label for="description">Description (optional):</label>
        <textarea
          id="description"
          [(ngModel)]="description"
          class="textarea"
          placeholder="Add a description for this PDF..."
        ></textarea>
      </div>

      <div *ngIf="errorMessage" class="error-message">
        {{ errorMessage }}
      </div>

      <div *ngIf="successMessage" class="success-message">
        {{ successMessage }}
      </div>

      <div *ngIf="uploading" class="progress">
        <div class="progress-bar">
          <div class="progress-fill"></div>
        </div>
        <p>Uploading...</p>
      </div>

      <button
        (click)="uploadFile()"
        [disabled]="!selectedFile || uploading"
        class="btn btn-primary"
      >
        {{ uploading ? 'Uploading...' : 'Upload PDF' }}
      </button>
    </div>
  `,
  styles: [`
    .upload-container {
      max-width: 600px;
      margin: 0 auto;
    }

    h2 {
      color: #2c3e50;
      margin-bottom: 2rem;
    }

    .upload-box {
      margin-bottom: 2rem;
      padding: 2rem;
      border: 2px dashed #3498db;
      border-radius: 8px;
      background-color: #ecf0f1;
      cursor: pointer;
      transition: all 0.3s;
    }

    .upload-box:hover {
      background-color: #d5dbdb;
    }

    .file-input {
      display: none;
    }

    .file-label {
      display: flex;
      flex-direction: column;
      align-items: center;
      cursor: pointer;
    }

    .upload-icon {
      font-size: 3rem;
      margin-bottom: 1rem;
    }

    .upload-text {
      font-size: 1rem;
      color: #2c3e50;
      text-align: center;
    }

    .form-group {
      margin-bottom: 2rem;
    }

    .form-group label {
      display: block;
      margin-bottom: 0.5rem;
      color: #2c3e50;
      font-weight: 500;
    }

    .textarea {
      width: 100%;
      padding: 0.75rem;
      border: 1px solid #bdc3c7;
      border-radius: 4px;
      font-family: inherit;
      resize: vertical;
      min-height: 100px;
    }

    .error-message {
      padding: 1rem;
      margin-bottom: 1rem;
      background-color: #fadbd8;
      border-left: 4px solid #e74c3c;
      color: #c0392b;
      border-radius: 4px;
    }

    .success-message {
      padding: 1rem;
      margin-bottom: 1rem;
      background-color: #d5f4e6;
      border-left: 4px solid #27ae60;
      color: #1e8449;
      border-radius: 4px;
    }

    .progress {
      margin-bottom: 2rem;
    }

    .progress-bar {
      height: 8px;
      background-color: #ecf0f1;
      border-radius: 4px;
      overflow: hidden;
      margin-bottom: 1rem;
    }

    .progress-fill {
      height: 100%;
      background-color: #3498db;
      animation: pulse 1.5s ease-in-out infinite;
    }

    @keyframes pulse {
      0%, 100% { width: 30%; }
      50% { width: 70%; }
    }

    .btn {
      width: 100%;
      padding: 0.75rem;
      font-size: 1rem;
      border: none;
      border-radius: 4px;
      cursor: pointer;
      transition: background-color 0.3s;
    }

    .btn-primary {
      background-color: #3498db;
      color: white;
    }

    .btn-primary:hover:not(:disabled) {
      background-color: #2980b9;
    }

    .btn:disabled {
      background-color: #bdc3c7;
      cursor: not-allowed;
    }
  `]
})
export class UploadComponent {
  selectedFile: File | null = null;
  description = '';
  uploading = false;
  errorMessage = '';
  successMessage = '';

  constructor(private pdfService: PdfService, private router: Router) {}

  onFileSelected(event: Event): void {
    const target = event.target as HTMLInputElement;
    const files = target.files;

    if (files && files.length > 0) {
      this.selectedFile = files[0];
      this.errorMessage = '';
      this.successMessage = '';
    }
  }

  uploadFile(): void {
    if (!this.selectedFile) {
      this.errorMessage = 'Please select a file first';
      return;
    }

    this.uploading = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.pdfService.uploadPdf(this.selectedFile, this.description).subscribe({
      next: (response) => {
        this.uploading = false;
        this.successMessage = `PDF "${response.fileName}" uploaded successfully!`;
        this.selectedFile = null;
        this.description = '';

        setTimeout(() => {
          this.router.navigate(['/list']);
        }, 1500);
      },
      error: (error) => {
        this.uploading = false;
        this.errorMessage = error.error?.message || 'Error uploading file. Please try again.';
      }
    });
  }
}
