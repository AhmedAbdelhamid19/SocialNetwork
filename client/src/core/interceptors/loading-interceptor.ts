import { HttpEvent, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { delay, finalize, of, tap } from 'rxjs';
import { BusyService } from '../services/busy-service';

// simple in-memory cache, you should use a more robust solution for production apps
// it must initialize outside of the interceptor function to persist between requests, 
// if it initilized inside the function, it will be recreated on each request
const cache = new Map<string, HttpEvent<unknown>>();
// interceptors are created once when the application starts and stays alive as long the app is running
// it intercepts all http requests and responses
// it load again if the when you refresh the page, close the browser and open it again, close the tab, etc.
export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const busyService = inject(BusyService);

  if (req.method === 'GET') {
    const cached = cache.get(req.url);
    if (cached) {
        return of(cached); // return cached result instantly
    }
  }

  busyService.busy();
  return next(req).pipe(
    delay(500),
    tap(response => {
       cache.set(req.url, response)
    }),
    finalize(() => busyService.idle())
  );
};
