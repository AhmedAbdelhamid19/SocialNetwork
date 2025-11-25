import { inject, Injectable } from '@angular/core';
import { AccountService } from './account-service';
import { Observable, of, tap } from 'rxjs';
import { FollowService } from './follow-service';

@Injectable({
  providedIn: 'root'
})
// this service runs during app initialization to set the current user from cookie
// before any components load that might depend on the user being set
// it run in app.config.ts via APP_INITIALIZER provider
export class InitService {
  private accountService = inject(AccountService);
  private followService = inject(FollowService);

  init(){
    return this.accountService.getRefreshToken().pipe(
      tap(user => {
        if(user) {
          this.accountService.setCurrentUser(user);
          this.accountService.startRefreshTokenInterval();
        }
      })
    )
  }
}