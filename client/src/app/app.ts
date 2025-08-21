import { HttpClient } from '@angular/common/http';
import { Component, inject, signal, OnInit  } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { lastValueFrom } from 'rxjs';
import { Nav } from '../layout/nav/nav';
import { AccountService } from '../core/services/account-service';
import { Home } from "../features/home/home";
import { User } from '../types/user';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-root',
  imports: [Nav, RouterOutlet, NgClass],
  templateUrl: './app.html',
  styleUrl: './app.css'  
})
export class App implements OnInit{
  private accountService = inject(AccountService);
  protected router = inject(Router);
  private http = inject(HttpClient);
  protected readonly title  = signal('Social Network');
  protected members = signal<User[]>([]);

  async ngOnInit() {
    this.members.set(await this.getMembers());
    this.setCurrentUser();
  }
  setCurrentUser() {
    // if user logged in then the information stored in local storage
    // if user not logged in then remain in as you are
    const userString = localStorage.getItem('user');
    if(!userString) return
    const user = JSON.parse(userString);
    this.accountService.currentUser.set(user);
  }

  async getMembers() {
    try {
      return lastValueFrom(this.http.get<User[]>('https://localhost:5001/api/members/getusers'));
    } catch (error) {
      console.log(error);
      throw error;
    }
  }
}
