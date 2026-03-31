import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { jwtInterceptor } from './core/jwt-interceptor';
import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes), // sistem de rutare

    // Activează HttpClient (pentru fetch/post către backend)
    // withInterceptors — aplică interceptorul JWT la TOATE requesturile HTTP
    provideHttpClient(withInterceptors([jwtInterceptor])),
  ]
};
