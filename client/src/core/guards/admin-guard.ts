import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AccountService } from '../services/account-service';
import { ToastService } from '../services/toast-service';

export const adminGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const toastService = inject(ToastService);
  const user = accountService.currentUser();
  if(user && user.roles.includes('Admin')) {
    return true;
  }

  toastService.error('You do not have permission to access');
  return false;
};
