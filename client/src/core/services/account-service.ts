import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { LoginCreds, RegisterCreds, User } from '../../types/user';
import { tap } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private http = inject(HttpClient);
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
    this.currentUser.set(null);
  }
  setCurrentUser(user: User) {
    // Temporary debug log to verify image URL
    console.log('Setting current user with image:', user);
    console.log('Setting current user with image:', user?.imageUrl);
    console.log('Setting current user with image:', user?.displayName);
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUser.set(user);
  }
}