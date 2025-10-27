import { CanActivateFn } from '@angular/router';
import { AccountService } from '../services/account-service';
import { inject } from '@angular/core';
import { ToastService } from '../services/toast-service';

export const authGuard: CanActivateFn = () => {
  const accountService = inject(AccountService);
  const toast = inject(ToastService);
  
  if(accountService.currentUser()) {
    return true;
  }
  else {
    toast.error('You must be logged in to access this page');
    return false;
  }
};
