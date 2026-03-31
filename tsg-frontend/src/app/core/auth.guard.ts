// Paznic pt rute
// ruleaza inainte sa incarce o pag si decide dc utiliz o poate accesa

import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth';

export const authGuard: CanActivateFn = () => {
  const auth = inject(AuthService); // pt a verif dc utiliz e logat
  const router = inject(Router); // pt a redirectiona daca nu e logat

  if (!auth.isLoggedIn()) return true;
  return router.createUrlTree(['/login']); // redirectioneaza catre pagina de login daca nu e logat
};

export const adminGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (auth.isAdmin()) return true;
  return router.createUrlTree(['/']); // redirectioneaza catre home daca nu e admin
};
