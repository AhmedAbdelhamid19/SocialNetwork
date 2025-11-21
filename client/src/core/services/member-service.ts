import { HttpClient, HttpHandler, HttpHeaders, HttpParams } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { EditMember, Member, MemberParams } from '../../types/member'; 
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

  getMembers(memberParams: MemberParams) {
    // We use HttpParams to add query parameters to the request
    let params = new HttpParams();
    params = params.append('pageNumber', memberParams.pageNumber);
    params = params.append('pageSize', memberParams.pageSize);
    params = params.append('minAge', memberParams.minAge);
    params = params.append('maxAge', memberParams.maxAge);
    params = params.append('orderBy', memberParams.orderBy);

    if(memberParams.gender) {
      params = params.append('gender', memberParams.gender);
    }
    return this.http.get<PaginatedResult<Member>> (this.baseUrl + 'members/GetUsers', {params}).pipe(
      tap(() => {
        localStorage.setItem('filters', JSON.stringify(memberParams));
      })
    );
  }
  getMember(id: string) {
    return this.http.get<Member>(this.baseUrl + 'members/GetUser/' + id).pipe(
      tap(member => {
        this.member.set(member);
      })
    );
  }
  updateMember(member: EditMember) {
     return this.http.put(this.baseUrl + `members/update`, member);
  }
}