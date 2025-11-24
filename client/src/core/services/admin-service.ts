import { ElementRef, inject, Injectable, ViewChild } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { User } from '../../types/user';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl + 'admin/';
  private http = inject(HttpClient);

  getUserWithRoles() {
    return this.http.get(this.baseUrl + 'users-with-roles');
  }
  updateUserRoles(id: number, roles: string[]) {
    return this.http.post(`${this.baseUrl}edit-roles/${id}?roles=${roles}`, {});
  }
}