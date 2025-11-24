import { Component, ElementRef, inject, OnInit, signal, ViewChild } from '@angular/core';
import { AdminService } from '../../../core/services/admin-service';
import { User } from '../../../types/user';
import { ToastService } from '../../../core/services/toast-service';

@Component({
  selector: 'app-user-management',
  imports: [],
  templateUrl: './user-management.html',
  styleUrl: './user-management.css'
})
export class UserManagement implements OnInit {
  @ViewChild('rolesModal') rolesModal!: ElementRef<HTMLDialogElement>;
  private  adminService = inject(AdminService);
  private toastService = inject(ToastService);
  protected users = signal<User[]>([]);
  protected availableRoles = ['Admin', 'Moderator', 'Member'];
  protected selectedUser: User | null = null;

  ngOnInit(): void {
    this.loadUsersWithRoles();
  }

  loadUsersWithRoles() {
    this.adminService.getUserWithRoles().subscribe({
      next: users => {
        this.users.set(users as User[]);
      }
    });
  } 
  openRolesModal(user: User) {
    this.selectedUser = user;
    this.rolesModal.nativeElement.showModal();
  }
  closeRolesModal() {
    this.rolesModal.nativeElement.close();
    this.selectedUser = null;
  }
  toggleRoleSelection(role: string, event: Event) {
    if(!this.selectedUser) return;

    const checkbox = event.target as HTMLInputElement;
    if (checkbox.checked) {
      this.selectedUser?.roles?.push(role);
    } else {
      this.selectedUser!.roles = this.selectedUser!.roles!.filter(r => r !== role);
    }
  }
  saveRoles() {
    if (!this.selectedUser) return;
    
    this.adminService.updateUserRoles(this.selectedUser.id, this.selectedUser.roles).subscribe({
      next: updatedRoles => { 
        this.users.update(users => users.map(user => {
          if (user.id === this.selectedUser!.id) {
            user.roles = updatedRoles as string[];
            this.rolesModal.nativeElement.close();
            this.toastService.info('Roles updated successfully for ' + user.displayName);
          }
          return user;
        }))
      },
      error: err => {
        this.toastService.error('Failed to update roles: ' + err);
      }
    })
  }
}