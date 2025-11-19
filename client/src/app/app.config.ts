import { ApplicationConfig, inject, provideAppInitializer, provideBrowserGlobalErrorListeners, provideZonelessChangeDetection } from '@angular/core';
import { provideRouter, withViewTransitions } from '@angular/router';
import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { InitService } from '../core/services/init-service';
import { lastValueFrom } from 'rxjs';
import { errorInterceptor } from '../core/interceptors/error-interceptor';
import { jwtInterceptor } from '../core/interceptors/jwt-interceptor';
import { cachingInterceptor } from '../core/interceptors/caching-interceptor';
import { loadingInterceptor } from '../core/interceptors/loading-interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZonelessChangeDetection(),
    provideRouter(routes, withViewTransitions()),
  // Order matters: errorInterceptor runs first to catch response errors, jwtInterceptor attaches token,
  // cachingInterceptor can short-circuit GETs and avoid loading state, loadingInterceptor manages busy state.
  provideHttpClient(withInterceptors([errorInterceptor, jwtInterceptor, cachingInterceptor, loadingInterceptor]))  ,
    provideAppInitializer(async () => {
      const initServcie = inject(InitService);
      return new Promise<void>((resolve) => {
        setTimeout(() => {
          try {
            return lastValueFrom(initServcie.init());
          } finally {
            const splash = document.getElementById('initial-splash');
            if (splash) {
              splash.remove();
            }
            resolve()
          }
        }, 200);
      })

    })
  ]
};