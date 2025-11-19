import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { finalize } from 'rxjs';
import { BusyService } from '../services/busy-service';

// This interceptor is responsible only for showing/hiding the global busy state
// Caching has been extracted to a separate interceptor (`caching-interceptor.ts`).
export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  // busyService is a singleton used to track active requests. busy() increments the
  // active count and idle() decrements it. We call busy() for every request and
  // ensure idle() in finalize so the counter is always consistent even on error.
  const busyService = inject(BusyService);

  busyService.busy();
  return next(req).pipe(
    finalize(() => busyService.idle())
  );
};
