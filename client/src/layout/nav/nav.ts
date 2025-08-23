import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../../core/services/account-service';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { ToastService } from '../../core/services/toast-service';

@Component({
  selector: 'app-nav',
  imports: [FormsModule, RouterLink, RouterLinkActive],
  templateUrl: './nav.html',
  styleUrl: './nav.css'
})
export class Nav {
  protected accountService = inject(AccountService);
  protected toastService = inject(ToastService);
  private router = inject(Router);
  protected creds: any = {}

  login() {
    this.accountService.login(this.creds).subscribe({
      next: () => {
        this.router.navigateByUrl('/members');
        this.toastService.success("login successfully");
        this.creds = {};
      },
      error: error => {
        console.log(error);
        this.toastService.error("please type email and password correct!");
      }
    });
  }
  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }
}
