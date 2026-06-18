export interface PdfFileInfo {
  fileId: string;
  fileName: string;
  uploadedAt: Date;
  fileSizeBytes: number;
  description?: string;
}

export interface UploadResponse {
  fileId: string;
  fileName: string;
  uploadedAt: Date;
  fileSizeBytes: number;
}
