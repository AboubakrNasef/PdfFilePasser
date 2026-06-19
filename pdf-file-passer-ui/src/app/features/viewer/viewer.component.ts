import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BlobViewerComponent } from '../pdf-viewer/blob-viewer/blob-viewer.component';
import { BlobStreamViewerComponent } from '../pdf-viewer/stream-viewer/blob-stream-viewer.component';
import { MemoryStreamViewerComponent } from '../pdf-viewer/direct-stream-viewer/memory-stream-viewer.component';

interface Method {
  id: allowedViewMethod;
  label: string;
  icon: string;
}
type  allowedViewMethod='bytes' | 'memory-stream' | 'blobstorage-stream';
@Component({
  selector: 'app-viewer',
  standalone: true,
  imports: [CommonModule, BlobViewerComponent, BlobStreamViewerComponent, MemoryStreamViewerComponent],
  templateUrl: './viewer.component.html',
  styleUrl: './viewer.component.css'
})
export class ViewerComponent {
  public selectedMethod = signal<allowedViewMethod>('bytes');

  public allowedMethods = signal<Method[]>([
    {
      id: 'bytes',
      label: 'bytes viewer',
      icon: '📥'
    },
    {
      id: 'memory-stream',
      label: 'memory stream Viewer',
      icon: '🔗'
    },
    {
      id: 'blobstorage-stream',
      label: 'BlobStorage Stream Viewer',
      icon: '▶️'
    }
  ])

  selectMethod(method:allowedViewMethod) {
    this.selectedMethod.set(method);
  }
}
