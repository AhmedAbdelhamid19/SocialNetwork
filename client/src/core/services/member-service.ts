import { HttpClient, HttpHandler, HttpHeaders, HttpParams } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { EditMember, Member, Photo } from '../../types/member'; 
import { tap } from 'rxjs';
import { PaginatedResult } from '../../types/pagination';

@Injectable({
  providedIn: 'root'
})
export class MemberService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;
  editMode = signal(false);
  member = signal<Member | null>(null);

  getMembers(pageNumber = 1, pageSize = 5) {
    let params = new HttpParams();
    params = params.append('pageNumber', pageNumber.toString());
    params = params.append('pageSize', pageSize.toString());

    return this.http.get<PaginatedResult<Member>> (this.baseUrl + 'members/GetUsers', {params});
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
  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + `members/set-main-photo/${photoId}`, {});
  }
  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + `members/delete-photo/${photoId}`);
  }
}