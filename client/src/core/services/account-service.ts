import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { LoginCreds, RegisterCreds, User } from '../../types/user';
import { tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { FollowService } from './follow-service';
import { PresenceService } from './presence-service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private http = inject(HttpClient);
  private followService = inject(FollowService);
  currentUser = signal<User | null>(null);
  private presenceService = inject(PresenceService);
  baseUrl = environment.apiUrl;

  getRefreshToken() {
    return this.http.post<User>(this.baseUrl + 
      "account/refresh-token", {}, {withCredentials: true});
  }
  startRefreshTokenInterval() {
    // remember it from JS
    // it stop when you close the tap or the browser
    // so it keep runing when the user visit the app
    setInterval(() => { 
      return this.http.post<User>(this.baseUrl + 
        "account/refresh-token", {}, {withCredentials: true}).subscribe({
          next: user => {
            this.currentUser.set(user);
          },
          error: () => {
            this.logout();
          }
        })
    }, 5*60*1000)
  }
  register(creds: RegisterCreds) {
    return this.http.post<User>(this.baseUrl + 'account/register', creds, {withCredentials: true}).pipe(
      tap(user => {
        if(user) {
          this.setCurrentUser(user);
          this.startRefreshTokenInterval();
        }
      })
    );
  }
  login(creds: LoginCreds) {
    return this.http.post<User>(this.baseUrl + 'account/login', creds, {withCredentials: true}).pipe(
      tap(user => {
        if(user) {
          this.setCurrentUser(user);
          this.startRefreshTokenInterval();
        }
      })
    )
  }
  logout() {
    localStorage.removeItem('filters');
    this.currentUser.set(null);
    this.followService.clearFollows();
    this.presenceService.stopHubConnection();
  }
  setCurrentUser(user: User) {
    user.roles = this.getRolesFromToken(user.token);
    this.currentUser.set(user);
    // Load following IDs to maintain follow status
    this.followService.getFollowingIdsPaged({ 
      predicate: 'following', pageNumber: 1, pageSize: 5 
    }).subscribe();
    // Optionally pre-load a small page of members to warm cache
    this.followService.getFollowersPaged({ 
      predicate: 'followers', pageNumber: 1, pageSize: 5 
    }).subscribe();
    this.followService.getFollowingPaged({ 
      predicate: 'following', pageNumber: 1, pageSize: 5 
    }).subscribe();

    if(!this.presenceService.isConnected()) {
      console.log('Starting SignalR connection');
      this.presenceService.createHubConnection(user);
    }
  }
  private getRolesFromToken(token: string): string[] {
    const payload = JSON.parse(atob(token.split('.')[1]));
    // token encoded in base64, and Atop decode a base64 string
    return Array.isArray(payload.role) ? payload.role : [payload.role];
  }
}