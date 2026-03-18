import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface CrearMovimiento {
  tipo: string;
  monto: number;
  categoria: string;
  descripcion: string;
  fecha: string;
  cuentaId: number;
}

export interface Movimiento {
  id: number;
  tipo: string;
  monto: number;
  categoria: string;
  descripcion: string;
  fecha: string;
  nombreCuenta: string;
}

export interface ResumenMovimientos {
  totalIngresos: number;
  totalGastos: number;
  diferencia: number;
  movimientos: Movimiento[];
  gastosPorCategoria: Record<string, number>;
}

@Injectable({ providedIn: 'root' })
export class MovimientoService {
  private apiUrl = `${environment.apiUrl}/movimientos`;

  constructor(private http: HttpClient) {}

  crear(dto: CrearMovimiento): Observable<Movimiento> {
    return this.http.post<Movimiento>(this.apiUrl, dto);
  }

  obtenerResumen(mes: number, anio: number): Observable<ResumenMovimientos> {
    return this.http.get<ResumenMovimientos>(
      `${this.apiUrl}/resumen?mes=${mes}&anio=${anio}`
    );
  }
}
