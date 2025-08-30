import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { ToastService } from '../services/toast-service';
import { Router } from '@angular/router';
import { catchError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const toast = inject(ToastService);
  const router = inject(Router);

  return next(req).pipe(
    catchError(error => {
      if(error) {
        switch (error.status) {
          case 400:
            if(error.error.errors) {
              const modelStateErrors = [];
              for(const key in error.error.errors) {
                if(error.error.errors[key]) {
                  modelStateErrors.push(error.error.errors[key]);
                }
              }
              throw modelStateErrors.flat();
            } else {
              toast.error(error.error);
            }
            break;
          case 401:
            toast.error("Unauthorized.");
            break;
          case 404:
            toast.error("Not Found.");
            break;
          case 500:
            toast.error("Server Error.");
            break;
          default:
            toast.error("Something went wrong, try later or contact the team.");
            break;
        }
      }
      throw error;
    })
  );
};
