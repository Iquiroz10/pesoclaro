import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';

export interface LoginDTO {
  email: string;
  password: string;
}

export interface RegistroDTO {
  nombre: string;
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  nombre: string;
  email: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private apiUrl = `${environment.apiUrl}/auth`;
  private usuarioActual = new BehaviorSubject<AuthResponse | null>(null);

  constructor(private http: HttpClient, private router: Router) {
    const token = localStorage.getItem('token');
    const nombre = localStorage.getItem('nombre');
    const email = localStorage.getItem('email');

    if (token && nombre && email) {
      this.usuarioActual.next({ token, nombre, email });
    }
  }

  get usuario$(): Observable<AuthResponse | null> {
    return this.usuarioActual.asObservable();
  }

  get estaAutenticado(): boolean {
    return !!localStorage.getItem('token');
  }

  login(dto: LoginDTO): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, dto).pipe(
      tap(response => this.guardarSesion(response))
    );
  }

  registro(dto: RegistroDTO): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/registro`, dto).pipe(
      tap(response => this.guardarSesion(response))
    );
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('nombre');
    localStorage.removeItem('email');
    this.usuarioActual.next(null);
    this.router.navigate(['/login']);
  }

  private guardarSesion(response: AuthResponse): void {
    localStorage.setItem('token', response.token);
    localStorage.setItem('nombre', response.nombre);
    localStorage.setItem('email', response.email);
    this.usuarioActual.next(response);
  }
}
