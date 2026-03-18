import { Component, OnInit } from '@angular/core';
import { CommonModule, CurrencyPipe, DatePipe } from '@angular/common';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDividerModule } from '@angular/material/divider';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MovimientoService, ResumenMovimientos } from '../../services/movimiento.service';
import { CuentaService, Cuenta } from '../../services/cuenta.service';

@Component({
  selector: 'app-movimientos',
  standalone: true,
  imports: [
    CommonModule,
    CurrencyPipe,
    DatePipe,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDividerModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './movimientos.component.html',
  styleUrl: './movimientos.component.css'
})
export class MovimientosComponent implements OnInit {
  resumen: ResumenMovimientos | null = null;
  cuentas: Cuenta[] = [];
  cargando = true;
  mostrarFormulario = false;
  guardando = false;
  error = '';
  mesActual = new Date().getMonth() + 1;
  anioActual = new Date().getFullYear();
  formulario: FormGroup;

  categorias = [
    'Comida', 'Gasolina', 'Transporte', 'Renta',
    'Entretenimiento', 'Salud', 'Educación',
    'Ropa', 'Servicios', 'Ahorro', 'Inversión', 'Otro'
  ];

  constructor(
    private movimientoService: MovimientoService,
    private cuentaService: CuentaService,
    private fb: FormBuilder,
    private router: Router
  ) {
    this.formulario = this.fb.group({
      tipo:        ['Gasto', Validators.required],
      monto:       [null, [Validators.required, Validators.min(0.01)]],
      categoria:   ['', Validators.required],
      descripcion: [''],
      fecha:       [new Date().toISOString().split('T')[0], Validators.required],
      cuentaId:    [null, Validators.required]
    });
  }

  ngOnInit(): void {
    this.cargarCuentas();
    this.cargarResumen();
  }

  cargarCuentas(): void {
    this.cuentaService.obtenerTodas().subscribe({
      next: (data) => { this.cuentas = data; }
    });
  }

  cargarResumen(): void {
    this.cargando = true;
    this.movimientoService.obtenerResumen(this.mesActual, this.anioActual).subscribe({
      next: (data) => {
        this.resumen = data;
        this.cargando = false;
      },
      error: () => { this.cargando = false; }
    });
  }

  toggleFormulario(): void {
    this.mostrarFormulario = !this.mostrarFormulario;
    if (!this.mostrarFormulario) {
      this.formulario.reset({
        tipo: 'Gasto',
        fecha: new Date().toISOString().split('T')[0]
      });
      this.error = '';
    }
  }

  guardarMovimiento(): void {
    if (this.formulario.invalid) return;
    this.guardando = true;
    this.error = '';

    this.movimientoService.crear(this.formulario.value).subscribe({
      next: () => {
        this.guardando = false;
        this.mostrarFormulario = false;
        this.formulario.reset({
          tipo: 'Gasto',
          fecha: new Date().toISOString().split('T')[0]
        });
        this.cargarResumen();
      },
      error: () => {
        this.guardando = false;
        this.error = 'Error al guardar el movimiento.';
      }
    });
  }

  get categoriasTotales(): { nombre: string; monto: number }[] {
    if (!this.resumen) return [];
    return Object.entries(this.resumen.gastosPorCategoria)
      .map(([nombre, monto]) => ({ nombre, monto }))
      .sort((a, b) => b.monto - a.monto);
  }

  colorTipo(tipo: string): string {
    return tipo === 'Ingreso' ? 'ingreso' : 'gasto';
  }

  regresar(): void {
    this.router.navigate(['/dashboard']);
  }
}
