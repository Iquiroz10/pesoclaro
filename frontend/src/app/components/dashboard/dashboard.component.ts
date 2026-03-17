import { Component, OnInit } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { PatrimonioService, Patrimonio } from '../../services/patrimonio.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    CurrencyPipe,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatDividerModule,
    MatProgressSpinnerModule,
    MatChipsModule
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  patrimonio: Patrimonio | null = null;
  cargando = true;
  nombreUsuario = '';

  constructor(
    private patrimonioService: PatrimonioService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.nombreUsuario = localStorage.getItem('nombre') || '';
    this.cargarPatrimonio();
  }

  cargarPatrimonio(): void {
    this.cargando = true;
    this.patrimonioService.obtener().subscribe({
      next: (data) => {
        this.patrimonio = data;
        this.cargando = false;
      },
      error: () => {
        this.cargando = false;
      }
    });
  }

  get colorPatrimonio(): string {
    if (!this.patrimonio) return 'neutral';
    return this.patrimonio.patrimonioNeto >= 0 ? 'positivo' : 'negativo';
  }

  get colorRendimiento(): string {
    if (!this.patrimonio) return '';
    return this.patrimonio.rendimientoNeto >= 0 ? 'positivo' : 'negativo';
  }

  irACuentas(): void {
    this.router.navigate(['/cuentas']);
  }

  logout(): void {
    this.authService.logout();
  }
}
