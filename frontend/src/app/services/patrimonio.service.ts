import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Consejo {
  tipo: string;
  mensaje: string;
  impactoMensual: number;
}

export interface Patrimonio {
  patrimonioNeto: number;
  totalActivos: number;
  totalDeudas: number;
  totalLiquidez: number;
  totalInversiones: number;
  totalAhorros: number;
  rendimientoMensualTotal: number;
  costoMensualDeudas: number;
  rendimientoNeto: number;
  consejos: Consejo[];
}

@Injectable({ providedIn: 'root' })
export class PatrimonioService {
  private apiUrl = `${environment.apiUrl}/patrimonio`;

  constructor(private http: HttpClient) {}

  obtener(): Observable<Patrimonio> {
    return this.http.get<Patrimonio>(this.apiUrl);
  }
}
