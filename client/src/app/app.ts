import { Component, inject, OnInit } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { Nav } from '../layout/nav/nav';
import { NgClass } from '@angular/common';
import { AccountService } from '../core/services/account-service';
import { User } from '../types/user';

@Component({
  selector: 'app-root',
  imports: [Nav, RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'  
})
export class App implements OnInit {
  protected router = inject(Router);
  private accountService = inject(AccountService);

  ngOnInit() {
    // Restore user from localStorage on app start
    const userString = localStorage.getItem('user');
    if (userString) {
      const user: User = JSON.parse(userString);
      this.accountService.setCurrentUser(user);
    }
  }


}
