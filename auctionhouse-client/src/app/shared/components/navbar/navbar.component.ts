import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <nav class="navbar">
      <div class="logo">
        <a routerLink="/">Smith & Co.</a>
      </div>
      <ul class="nav-links">
        <li><a routerLink="/">Forside</a></li>
        <li><a routerLink="/auctions">Auktioner</a></li>

        <!-- Ikke-logget -->
        <li *ngIf="!auth.isLoggedIn()"><a routerLink="/register" class="btn">Opret Konto</a></li>
        <li *ngIf="!auth.isLoggedIn()"><a routerLink="/login" class="btn btn-login">Log ind</a></li>

        <!-- Logget ind -->
        <ng-container *ngIf="auth.isLoggedIn()">
          <li><a routerLink="/profile">Min Profil</a></li>
          <li><a routerLink="/my-bids">Mine Bud</a></li>

          <!-- Kun admin -->
          <li *ngIf="auth.isLoggedIn()">
            <a routerLink="/admin">Admin Dashboard</a>
          </li>
        </ng-container>
      </ul>
    </nav>
  `,
  styles: [`
    /* Navbar container */
    .navbar {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 15px 40px;
      background-color: #bfa15e;
      color: #fff;
      position: sticky;
      top: 0;
      z-index: 1000;
      box-shadow: 0 2px 5px rgba(0,0,0,0.15);
      font-family: 'Segoe UI', sans-serif;
    }

    /* Logo */
    .navbar .logo a {
      font-size: 1.6rem;
      font-weight: bold;
      color: #fff;
      text-decoration: none;
    }

    /* Nav links container */
    .nav-links {
      display: flex;
      gap: 25px;
      list-style: none;
      align-items: center;
      margin: 0;
      padding: 0;
    }

    /* Regular links */
    .nav-links li a {
      color: #fff;
      text-decoration: none;
      font-weight: 500;
      padding: 6px 12px;
      border-radius: 5px;
      transition: background 0.3s, color 0.3s;
    }

    .nav-links li a:hover {
      background-color: rgba(255, 255, 255, 0.2);
    }

    /* Buttons */
    .btn {
      background-color: transparent;
      border: 1px solid #fff;
      color: #fff;
      padding: 6px 14px;
      border-radius: 5px;
      font-weight: 500;
      transition: all 0.3s ease;
    }

    .btn:hover {
      background-color: #fff;
      color: #bfa15e;
    }

    .btn-login {
      border: 1px solid #fff;
    }

    .btn-login:hover {
      background-color: #fff;
      color: #bfa15e;
    }

    /* Admin button */
    .btn-admin {
      background-color: #fff;
      color: #bfa15e;
      font-weight: 600;
      padding: 6px 16px;
      border-radius: 8px;
      transition: background 0.3s, color 0.3s;
    }

    .btn-admin:hover {
      background-color: #e6d6a2;
      color: #333;
    }

    /* Responsive spacing */
    @media (max-width: 768px) {
      .nav-links {
        gap: 15px;
      }
      .navbar {
        padding: 10px 20px;
      }
    }
  `]
})
export class NavbarComponent {
  constructor(public auth: AuthService, private router: Router) {
    console.log('Navbar role:', this.auth.getUserRole());
  }

  logout() {
    this.auth.logout();
    this.router.navigate(['/']);
  }
}
