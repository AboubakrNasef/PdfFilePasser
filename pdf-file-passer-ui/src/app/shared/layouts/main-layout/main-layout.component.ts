import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterOutlet],
  template: `
    <div class="app-container">
      <nav class="navbar">
        <div class="nav-brand">
          <h1>PDF Filer</h1>
        </div>
        <ul class="nav-links">
          <li><a routerLink="/" class="nav-link">Home</a></li>
          <li><a routerLink="/upload" class="nav-link">Upload PDF</a></li>
          <li><a routerLink="/list" class="nav-link">My PDFs</a></li>
        </ul>
      </nav>

      <main class="content">
        <router-outlet></router-outlet>
      </main>
    </div>
  `,
  styles: [`
    .app-container {
      display: flex;
      flex-direction: column;
      min-height: 100vh;
    }

    .navbar {
      background-color: #2c3e50;
      color: white;
      padding: 1rem 2rem;
      display: flex;
      justify-content: space-between;
      align-items: center;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }

    .nav-brand h1 {
      margin: 0;
      font-size: 1.5rem;
    }

    .nav-links {
      list-style: none;
      margin: 0;
      padding: 0;
      display: flex;
      gap: 2rem;
    }

    .nav-link {
      color: white;
      text-decoration: none;
      transition: color 0.3s;
    }

    .nav-link:hover {
      color: #3498db;
    }

    .content {
      flex: 1;
      padding: 2rem;
      max-width: 1200px;
      width: 100%;
      margin: 0 auto;
    }
  `]
})
export class MainLayoutComponent {}
