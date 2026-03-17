import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Cuenta {
  id: number;
  nombre: string;
  tipo: string;
  saldo: number;
  tasaAnual: number;
  tipoTasa: string;
  diaDeCorteMensual: number | null;
  notas: string;
  rendimientoAnual: number;
  rendimientoMensual: number;
  rendimientoDiario: number;
  fechaCreacion: string;
}

export interface CrearCuenta {
  nombre: string;
  tipo: string;
  saldo: number;
  tasaAnual: number;
  tipoTasa: string;
  diaDeCorteMensual: number | null;
  notas: string;
}

@Injectable({ providedIn: 'root' })
export class CuentaService {
  private apiUrl = `${environment.apiUrl}/cuentas`;

  constructor(private http: HttpClient) {}

  obtenerTodas(): Observable<Cuenta[]> {
    return this.http.get<Cuenta[]>(this.apiUrl);
  }

  crear(dto: CrearCuenta): Observable<Cuenta> {
    return this.http.post<Cuenta>(this.apiUrl, dto);
  }

  actualizarSaldo(id: number, nuevoSaldo: number): Observable<Cuenta> {
    return this.http.put<Cuenta>(`${this.apiUrl}/${id}/saldo`, { nuevoSaldo });
  }

  eliminar(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
