import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterLink],
  template: `
    <div class="home-container">
      <div class="hero">
        <h2>Welcome to PDF File Passer</h2>
        <p>Easily upload, view, and manage your PDF files in the cloud.</p>
      </div>

      <div class="features">
        <div class="feature-card">
          <h3>📁 Upload PDFs</h3>
          <p>Upload your PDF files securely to the cloud</p>
          <a routerLink="/upload" class="btn">Upload Now</a>
        </div>

        <div class="feature-card">
          <h3>👁️ View PDFs</h3>
          <p>View and manage all your uploaded PDFs</p>
          <a routerLink="/list" class="btn">View PDFs</a>
        </div>

        <div class="feature-card">
          <h3>☁️ Cloud Storage</h3>
          <p>All your files are stored safely in Azure Blob Storage</p>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .home-container {
      padding: 2rem;
    }

    .hero {
      text-align: center;
      margin-bottom: 3rem;
    }

    .hero h2 {
      font-size: 2.5rem;
      margin-bottom: 1rem;
      color: #2c3e50;
    }

    .hero p {
      font-size: 1.2rem;
      color: #7f8c8d;
    }

    .features {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
      gap: 2rem;
    }

    .feature-card {
      background: white;
      padding: 2rem;
      border-radius: 8px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);
      text-align: center;
    }

    .feature-card h3 {
      font-size: 1.5rem;
      margin-bottom: 1rem;
      color: #2c3e50;
    }

    .feature-card p {
      color: #7f8c8d;
      margin-bottom: 1.5rem;
    }

    .btn {
      display: inline-block;
      padding: 0.75rem 1.5rem;
      background-color: #3498db;
      color: white;
      text-decoration: none;
      border-radius: 4px;
      transition: background-color 0.3s;
    }

    .btn:hover {
      background-color: #2980b9;
    }
  `]
})
export class HomeComponent {}
