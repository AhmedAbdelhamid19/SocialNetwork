import { HttpClient, HttpHandler, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member, Photo } from '../../types/member'; 

@Injectable({
  providedIn: 'root'
})
export class MemberService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;

  getMembers() {
    return this.http.get<Member []> (this.baseUrl + 'members/GetUsers');
  }
  getMember(id: string) {
    return this.http.get<Member>(this.baseUrl + 'members/GetUser/' + id);
  }
  getMemberPhotos(id: string) {
    return this.http.get<Photo[]>(this.baseUrl + `members/${id}/photos`);
  }
}
