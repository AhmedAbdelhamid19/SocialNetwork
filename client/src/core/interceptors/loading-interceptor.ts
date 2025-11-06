import { HttpEvent, HttpInterceptorFn, HttpParams } from '@angular/common/http';
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
  // busyService is singleton, same instance used across the app, used to track number of active requests
  // busy() increments the count, idle() decrements the count
  const busyService = inject(BusyService);
  const generateCacheKey = (url: string, params: HttpParams): string => {
    // for example: members/GetUsers?pageNumber=1&pageSize=5&minAge=18&maxAge=100&orderBy=lastActive
    const paramString = params.keys() 
      .map(key => `${key}=${params.getAll(key)?.join(',')}`) // , here is used to join multiple values for the same key like ?key=value1,value2
      .join('&');
    return paramString ? `${url}?${paramString}` : url;
  }

  const cacheKey = generateCacheKey(req.url, req.params);
  if (req.method === 'GET') {
    const cached = cache.get(cacheKey);
    if (cached) {
        return of(cached); // return cached result instantly
    }
  }

  busyService.busy();
  return next(req).pipe(
    delay(100),
    tap(response => {
       cache.set(cacheKey, response)
    }),
    finalize(() => busyService.idle())
  );
};
