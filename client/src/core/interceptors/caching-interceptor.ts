import { HttpEvent, HttpInterceptorFn, HttpParams } from '@angular/common/http';
import { inject } from '@angular/core';
import { of, tap } from 'rxjs';

// Simple in-memory cache. For production prefer a more robust solution 
// key: URL + query parameters.
// value: cached HTTP response.
const cache = new Map<string, HttpEvent<unknown>>();

const generateCacheKey = (url: string, params: HttpParams): string => {
  // Build a deterministic key ('?page=1&sort=name' same as '?sort=name&page=1)
  const keys = params.keys().sort();
  const paramString = keys.map(key => `${key}=${params.getAll(key)?.sort().join(',')}`).join('&');
  return paramString ? `${url}?${paramString}` : url;
}
const invalidateCache = (urlPattern: string) => {
  // delete given url if exist in cache
  for (const key of cache.keys()) {
    if (key.includes(urlPattern)) {
      cache.delete(key);
    }
  }
}

export const cachingInterceptor: HttpInterceptorFn = (req, next) => {
  const cacheKey = generateCacheKey(req.url, req.params);
  if (req.method === 'POST' || req.method === 'PUT' || req.method === 'DELETE') {
    // remove the last segment of the url
    const urlPattern = req.url.split('/').slice(0, -1).join('/');
    invalidateCache(urlPattern);
  }
  if(req.method.includes('POST') && req.url.includes('/logout')) {
    cache.clear();
  }
  // Return cached response for GET requests when available
  if (req.method === 'GET') {
    const cached = cache.get(cacheKey);
    if (cached) {
      // Return cached result as observable and skip the request pipeline
      return of(cached);
    }
  }
  // For non-cached flows, let the request proceed and store the response when it arrives
  return next(req).pipe(
    tap(response => {
      if (req.method === 'GET') {
        cache.set(cacheKey, response);
      }
    })
  );
};