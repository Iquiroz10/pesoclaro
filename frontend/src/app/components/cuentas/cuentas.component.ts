import { Component, OnInit } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDividerModule } from '@angular/material/divider';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { CuentaService, Cuenta } from '../../services/cuenta.service';

@Component({
  selector: 'app-cuentas',
  standalone: true,
  imports: [
    CommonModule,
    CurrencyPipe,
    ReactiveFormsModule,
    FormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDividerModule,
    MatProgressSpinnerModule,
    MatChipsModule
  ],
  templateUrl: './cuentas.component.html',
  styleUrl: './cuentas.component.css'
})
export class CuentasComponent implements OnInit {
  cuentas: Cuenta[] = [];
  cargando = true;
  mostrarFormulario = false;
  cuentaEditandoId: number | null = null;
  nuevoSaldo = 0;
  formulario: FormGroup;
  guardando = false;
  error = '';

  constructor(
    private cuentaService: CuentaService,
    private fb: FormBuilder,
    private router: Router
  ) {
    this.formulario = this.fb.group({
      nombre:            ['', Validators.required],
      tipo:              ['', Validators.required],
      saldo:             [0, [Validators.required, Validators.min(0)]],
      tasaAnual:         [0, [Validators.required, Validators.min(0)]],
      tipoTasa:          ['', Validators.required],
      diaDeCorteMensual: [null],
      notas:             ['']
    });
  }

  ngOnInit(): void {
    this.cargarCuentas();
  }

  cargarCuentas(): void {
    this.cargando = true;
    this.cuentaService.obtenerTodas().subscribe({
      next: (data) => {
        this.cuentas = data;
        this.cargando = false;
      },
      error: () => { this.cargando = false; }
    });
  }

  toggleFormulario(): void {
    this.mostrarFormulario = !this.mostrarFormulario;
    if (!this.mostrarFormulario) {
      this.formulario.reset();
      this.error = '';
    }
  }

  guardarCuenta(): void {
    if (this.formulario.invalid) return;
    this.guardando = true;
    this.error = '';

    this.cuentaService.crear(this.formulario.value).subscribe({
      next: () => {
        this.guardando = false;
        this.mostrarFormulario = false;
        this.formulario.reset();
        this.cargarCuentas();
      },
      error: () => {
        this.guardando = false;
        this.error = 'Error al guardar la cuenta.';
      }
    });
  }

  editarSaldo(cuenta: Cuenta): void {
    this.cuentaEditandoId = cuenta.id;
    this.nuevoSaldo = cuenta.saldo;
  }

  guardarSaldo(id: number): void {
    this.cuentaService.actualizarSaldo(id, this.nuevoSaldo).subscribe({
      next: () => {
        this.cuentaEditandoId = null;
        this.cargarCuentas();
      }
    });
  }

  eliminarCuenta(id: number): void {
    if (!confirm('¿Deseas eliminar esta cuenta?')) return;
    this.cuentaService.eliminar(id).subscribe({
      next: () => this.cargarCuentas()
    });
  }

  colorTipo(tipo: string): string {
    const colores: Record<string, string> = {
      'Liquidez':  'chip-liquidez',
      'Inversion': 'chip-inversion',
      'Deuda':     'chip-deuda',
      'Ahorro':    'chip-ahorro'
    };
    return colores[tipo] || '';
  }

  regresar(): void {
    this.router.navigate(['/dashboard']);
  }
}
