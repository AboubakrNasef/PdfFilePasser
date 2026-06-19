import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BlobViewerComponent } from '../pdf-viewer/blob-viewer/blob-viewer.component';
import { DirectStreamViewerComponent } from '../pdf-viewer/direct-stream-viewer/direct-stream-viewer.component';
import { StreamViewerComponent } from '../pdf-viewer/stream-viewer/stream-viewer.component';

interface Method {
  id: 'blob' | 'stream' | 'direct-stream';
  label: string;
  icon: string;
}

@Component({
  selector: 'app-viewer',
  standalone: true,
  imports: [CommonModule, BlobViewerComponent, StreamViewerComponent, DirectStreamViewerComponent],
  templateUrl: './viewer.component.html',
  styleUrl: './viewer.component.css'
})
export class ViewerComponent {
  public selectedMethod = signal<'blob' | 'stream' | 'direct-stream'>('blob');

  public allowedMethods = signal<Method[]>([
    {
      id: 'blob',
      label: 'Blob Viewer',
      icon: '📥'
    },
    {
      id: 'stream',
      label: 'Stream Viewer',
      icon: '🔗'
    },
    {
      id: 'direct-stream',
      label: 'Direct Stream',
      icon: '▶️'
    }
  ])

  selectMethod(method: 'blob' | 'stream' | 'direct-stream') {
    this.selectedMethod.set(method);
  }
}
