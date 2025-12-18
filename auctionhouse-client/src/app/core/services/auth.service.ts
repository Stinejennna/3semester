import { Injectable, signal } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { tap } from 'rxjs/operators';
import { Observable } from 'rxjs';
import * as jwt_decode from 'jwt-decode';


export interface AuthResponse {
  accessToken: string;
  refreshToken?: string;
}

interface JwtPayload {
  role: string;
  username: string;
  sub: string;
  id: string;
}

@Injectable({
  providedIn: 'root'
})

export class AuthService {
  private apiUrl = 'http://localhost:5098/api/Auth';

  // Signal til at holde JWT token
  token = signal<string | null>(localStorage.getItem('token'));
  role = signal<string | null>(null);

  constructor(private http: HttpClient) {
    this.updateRoleFromToken();
  }

  private saveToken(token: string) {
    console.log('Saving token:', token);
    localStorage.setItem('token', token);
    this.token.set(token);
    this.updateRoleFromToken();
  }


  private updateRoleFromToken() {
    const token = this.token();
    if (!token) {
      this.role.set(null);
      return;
    }

    try {
      const decoded = (jwt_decode as any)(token);
      const role = decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
      this.role.set(role ?? null);
    } catch {
      this.role.set(null);
    }
  }

  getUserRole(): string | null {
    return this.role();
  }

  // Login med email/username (identifier)
  login(identifier: string, password: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, { identifier, password })
      .pipe(tap(res => this.saveToken(res.accessToken)));
  }

  // Registrering
  register(username: string, fullname: string, email: string, password: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/register`, { username, fullname, email, password })
      .pipe(tap(res => this.saveToken(res.accessToken)));
  }

  // Refresh token
  refresh(): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/refresh`, {})
      .pipe(tap(res => this.saveToken(res.accessToken)));
  }

  // Logout
  logout() {
    localStorage.removeItem('token');
    this.token.set(null);
    this.role.set(null); // <-- ryd role
  }


  // Check login-status
  isLoggedIn() {
    return !!this.token();
  }

  getUserInfo() {
    return this.http.get<any>('http://localhost:5098/api/users/me', { headers: this.getAuthHeaders() });
  }

  getUserStats() {
    return this.http.get<any>('http://localhost:5098/api/users/me/stats', { headers: this.getAuthHeaders() });
  }

  getUserInfoActiveAuctions() {
    return this.http.get<any[]>('http://localhost:5098/api/users/me/active-auctions', { headers: this.getAuthHeaders() });
  }

  getUserInfoWonAuctions() {
    return this.http.get<any[]>('http://localhost:5098/api/users/me/won-auctions', { headers: this.getAuthHeaders() });
  }

  // Auth headers til API-kald
  getAuthHeaders(): HttpHeaders {
    return new HttpHeaders({ Authorization: `Bearer ${this.token()}` });
  }
}
