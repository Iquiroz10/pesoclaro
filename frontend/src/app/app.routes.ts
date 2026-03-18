import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  {
    path: 'login',
    loadComponent: () =>
      import('./components/login/login.component')
        .then(m => m.LoginComponent)
  },
  {
    path: 'registro',
    loadComponent: () =>
      import('./components/registro/registro.component')
        .then(m => m.RegistroComponent)
  },
  {
    path: 'dashboard',
    loadComponent: () =>
      import('./components/dashboard/dashboard.component')
        .then(m => m.DashboardComponent),
    canActivate: [authGuard]
  },
  {
  path: 'cuentas',
  loadComponent: () =>
    import('./components/cuentas/cuentas.component')
      .then(m => m.CuentasComponent),
  canActivate: [authGuard]
  },
  { path: '**', redirectTo: 'login' },

];
