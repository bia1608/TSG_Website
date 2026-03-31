// Interceptor JWT = functie care ruleaza automatic la fiecare request HTTP,
// pentru a adauga tokenul JWT in headerul requestului,

// Rol: daca utilizatorul e logat, adauga token-ul JWT
// in headerul "Authorization" al fiecarui request

//Fara -> endpointurile [Authorize] returneaza 401 Unauthorized

import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from './auth';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const token = inject(AuthService).getToken(); // preia tokenul JWT din AuthService

  if (token) {
    req = req.clone({ // clonam requestul și adaugam headerul
      setHeaders: {
        Authorization: `Bearer ${token}` // adauga tokenul JWT in header
      }
    });
  }
  return next(req); // trimit requestul mai departe
};
