import { inject, Injectable } from '@angular/core';
import { AccountService } from './account-service';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
// this service runs during app initialization to set the current user from local storage
// before any components load that might depend on the user being set
// it run in app.module.ts via APP_INITIALIZER provider
export class InitService {
  private accountService = inject(AccountService);
  
  init(): Observable<null> {
    const userString = localStorage.getItem('user');
    if(!userString) return of(null);
    const user = JSON.parse(userString);
    this.accountService.setCurrentUser(user);
    return of(null)
  }
}
