import { HttpClient, HttpHandler, HttpHeaders } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { EditMember, Member, Photo } from '../../types/member'; 
import { tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MemberService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;
  editMode = signal(false);
  member = signal<Member | null>(null);

  getMembers() {
    return this.http.get<Member []> (this.baseUrl + 'members/GetUsers');
  }
  getMember(id: string) {
    return this.http.get<Member>(this.baseUrl + 'members/GetUser/' + id).pipe(
      tap(member => {
        this.member.set(member);
      })
    );
  }
  getMemberPhotos(id: string) {
    return this.http.get<Photo[]>(this.baseUrl + `members/${id}/photos`);
  }
  updateMember(member: EditMember) {
     return this.http.put(this.baseUrl + `members/update`, member);
  }
  uploadPhoto(file: File) {
    // must be FormData to send file to backend because it expects multipart/form-data 
    const formData = new FormData(); 
    // must be 'file' because the backend expects it
    formData.append('file', file);
    return this.http.post<Photo>(this.baseUrl + 'members/add-photo', formData);
  }
}
