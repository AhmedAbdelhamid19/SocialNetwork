import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { LoginCreds, RegisterCreds, User } from '../../types/user';
import { tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { FollowService } from './follow-service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private http = inject(HttpClient);
  private followService = inject(FollowService);
  currentUser = signal<User | null>(null);
  baseUrl = environment.apiUrl;

  register(creds: RegisterCreds) {
    return this.http.post<User>(this.baseUrl + 'account/register', creds).pipe(
      tap(user => {
        if(user) {
          this.setCurrentUser(user);
        }
      })
    );
  }
  login(creds: LoginCreds) {
    return this.http.post<User>(this.baseUrl + 'account/login', creds).pipe(
      tap(user => {
        if(user) {
          this.setCurrentUser(user);
        }
      })
    )
  }
  logout() {
    localStorage.removeItem('user');
    localStorage.removeItem('filters');
    this.currentUser.set(null);
    this.followService.clearFollows();
  }
  setCurrentUser(user: User) {
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUser.set(user);
    // Load following IDs to maintain follow status
    this.followService.getFollowingIdsPaged({ predicate: 'following', pageNumber: 1, pageSize: 1000 }).subscribe();
    // Optionally pre-load a small page of members to warm cache
    this.followService.getFollowersPaged({ predicate: 'followers', pageNumber: 1, pageSize: 5 }).subscribe();
    this.followService.getFollowingPaged({ predicate: 'following', pageNumber: 1, pageSize: 5 }).subscribe();
  }
}