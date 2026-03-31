import { Injectable, signal } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { Router } from '@angular/router';
import { tap } from 'rxjs/operators'; // pt side effects (salvare token in localStorage)

export interface AuthUser {
  token: string; // jwt
  email: string;
  role: string; // admin/user
  firstName: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly API = 'http://localhost:5119/api/auth';

  private readonly TOKEN_KEY = 'tsg_token';
  private readonly USER_KEY = 'tsg_user';

  currentUser = signal<AuthUser | null>(this.loadUser());

  constructor(
    private http: HttpClient,
    private router: Router // pt navigare dupa login/logout
  ) { }

  login(email: string, password: string) {
    return this.http
      .post<AuthUser>(`${this.API}/login`, { email, password })
      .pipe(tap(res => {
        localStorage.setItem(this.TOKEN_KEY, res.token); // salveaza tokenul JWT
        localStorage.setItem(this.USER_KEY, JSON.stringify(res));
        this.currentUser.set(res); // actualizeaza currentUser
      }));
  }

  logout() {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    this.currentUser.set(null);
    this.router.navigate(['/']);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  isAdmin(): boolean {
    return this.currentUser()?.role === 'Admin';
  }

  private loadUser(): AuthUser | null {
    try {
      const userJson = localStorage.getItem(this.USER_KEY);
      return userJson ? JSON.parse(userJson) : null;
    } catch {
      return null;
    }
  }
}
