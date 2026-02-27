import { Injectable, signal, computed } from '@angular/core';
import { environment } from '../../../environments/environment';
import { LoginRequest, RegisterRequest, User } from '../models/user.model';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly baseURL = environment.apiURL + '/auth';
  private readonly storageKey = environment.storageKey;

  // Signal-based state
  currentUser = signal<User | null>(this.loadUserFromStorage());

  // Computed helpers
  isLoggedIn = computed(() => !!this.currentUser());
  username = computed(() => this.currentUser()?.username ?? '');

  constructor(private http: HttpClient, private router: Router) {}

  register(request: RegisterRequest) {
    return this.http.post<User>(`${this.baseURL}/register`, request).pipe(
      tap(user => this.persistUser(user))
    );
  }

  login(request: LoginRequest) {
    return this.http.post<User>(`${this.baseURL}/login`, request).pipe(
      tap(user => this.persistUser(user))
    );
  }

  logout() {
    localStorage.removeItem(this.storageKey);
    this.currentUser.set(null);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return this.currentUser()?.token ?? null;
  }

  persistUser(user: User) {
    localStorage.setItem(this.storageKey, JSON.stringify(user));
    this.currentUser.set(user);
  }

  private loadUserFromStorage(): User | null {
    const raw = localStorage.getItem(this.storageKey);
    return raw ? JSON.parse(raw) : null;
  }
  
}
