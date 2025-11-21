import { Component, inject, Input, input, signal } from '@angular/core';
import { Register } from "../account/register/register";
import { User } from '../../types/user';
import { AccountService } from '../../core/services/account-service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  imports: [Register],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home {
  protected registerMode = signal(true);
  protected accountService = inject(AccountService);
  protected router = inject(Router);
  // Don't auto-redirect from the Home component; App initialization handles initial redirect.
  // Keeping this component idempotent lets the logo/routerLink('/') show the landing page even
  // when a user is logged in (clicking the logo won't immediately redirect to /members).
  showRegister(value : boolean) {
    this.registerMode.set(value);
  }
}
