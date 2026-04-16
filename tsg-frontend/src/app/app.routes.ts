// componenta se afișează la fiecare URL
//
// loadComponent = lazy loading — componenta se încarcă
// DOAR când utilizatorul navighează la acea rută

import { Routes } from '@angular/router';
import { authGuard, adminGuard } from './core/auth.guard';

export const routes: Routes = [
  {
    path: '',  // http://localhost:4200/
    loadComponent: () =>
      import('./pages/home/home').then(m => m.HomeComponent)
  },
  {
    path: 'login',
    loadComponent: () =>
      import('./pages/login/login').then(m => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./pages/register/register').then(m => m.RegisterComponent)
  },
  {
    path: 'blog',
    loadComponent: () =>
      import('./pages/blog/blog').then(m => m.BlogComponent)
  },
  {
    path: 'about',
    loadComponent: () =>
      import('./pages/about/about').then(m => m.AboutComponent)
  },
  {
    path: 'contact',
    loadComponent: () =>
      import('./pages/contact/contact').then(m => m.ContactComponent)
  },
  {
    path: 'dashboard',
    canActivate: [authGuard],  // ← trebuie să fii logat
    loadComponent: () =>
      import('./dashboard/dashboard').then(m => m.DashboardComponent)
  },
  {
    path: 'admin',
    canActivate: [adminGuard], // ← trebuie să fii admin
    loadComponent: () =>
      import('./admin/admin').then(m => m.AdminComponent)
  },
  {
    path: '**',
    redirectTo: ''
  }
];
