import { HttpEvent, HttpInterceptorFn, HttpParams } from '@angular/common/http';
import { inject } from '@angular/core';
import { of, tap } from 'rxjs';

// Simple in-memory cache. For production prefer a more robust solution 
// (LRU, TTL, size limits, persistent storage, or server-side caching) The Map must live outside the function so it persists between requests.
const cache = new Map<string, HttpEvent<unknown>>();

const generateCacheKey = (url: string, params: HttpParams): string => {
  // Build a deterministic key including query params in stable order
  const keys = params.keys().sort();
  const paramString = keys.map(key => `${key}=${params.getAll(key)?.join(',')}`).join('&');
  return paramString ? `${url}?${paramString}` : url;
}
const invalidateCache = (urlPattern: string) => {
  for (const key of Array.from(cache.keys())) {
    if (key.includes(urlPattern)) {
      cache.delete(key);
    }
  }
}
// Special-purpose invalidation rules can be added when the backend has specific naming conventions (example: deleteMessage -> invalidate getmessages)
const applySpecialInvalidation = (url: string) => {
  if (url.includes('deleteMessage')) {
    invalidateCache('getmessages');
  }
}
export const cachingInterceptor: HttpInterceptorFn = (req, next) => {
  const cacheKey = generateCacheKey(req.url, req.params);

  // Invalidate cache on mutating requests so stale GET results aren't returned
  if (req.method === 'POST' || req.method === 'PUT' || req.method === 'DELETE') {
    const urlPattern = req.url.split('/').slice(0, -1).join('/');
    invalidateCache(urlPattern);
    applySpecialInvalidation(req.url);
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
/*
Notes / possible improvements:
- Add TTL (time-to-live) per cache entry and periodically purge expired entries.
- Limit cache size (LRU) to avoid memory growth.
- Make cache key include headers if responses vary by header (e.g. Accept-Language).
- Consider using request.clone() and storing response body rather than full HttpEvent for memory.
*/