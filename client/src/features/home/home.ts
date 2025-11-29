import { Component, inject, Input, input, signal } from '@angular/core';
import { Register } from "../account/register/register";
import { User } from '../../types/user';
import { AccountService } from '../../core/services/account-service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  imports: [],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home {
  protected accountService = inject(AccountService);
  protected router = inject(Router);
}
