import { Component, inject } from '@angular/core';
import { AccountService } from '../../core/services/account-service';
import { PhotoManagement } from "./photo-management/photo-management";
import { UserManagement } from './user-management/user-management';
import { HasRole } from '../../shared/directives/has-role';

@Component({
  selector: 'app-admin',
  imports: [PhotoManagement, UserManagement, HasRole],
  templateUrl: './admin.html',
  styleUrl: './admin.css'
})
export class Admin {
  protected accountService = inject(AccountService);
  currentTap = 'photos';
  tabs = [
    { label: 'Photo Management', value: 'photos' },
    { label: 'User Management', value: 'roles' }
  ]

  setCurrentTap(tap: string) {
    this.currentTap = tap;
  }
}