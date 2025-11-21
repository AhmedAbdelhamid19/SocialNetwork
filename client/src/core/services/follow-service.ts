import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { tap, map } from 'rxjs';
import { Member } from '../../types/member';
import { environment } from '../../environments/environment';
import { PaginatedResult, FollowParams } from '../../types/pagination';

@Injectable({
  providedIn: 'root'
})
export class FollowService {
  private baseUrl = environment.apiUrl;
  private http = inject (HttpClient);
  followersIds = signal<number[]>([]);
  followingIds = signal<number[]>([]);
  toggleFollow(targetMemberId: number) {
    return this.http.post(this.baseUrl + 
        `follow/toggle-follow/${targetMemberId}`, {});
  }

  getFollowersIdsPaged(params?: FollowParams) {
    let httpParams = new HttpParams();
    if (params?.predicate) httpParams = httpParams.append('predicate', params.predicate);
    if (params?.pageNumber) httpParams = httpParams.append('pageNumber', params.pageNumber.toString());
    if (params?.pageSize) httpParams = httpParams.append('pageSize', params.pageSize.toString());
    return this.http.get<PaginatedResult<number>>(this.baseUrl + 'follow/follows-ids', { params: httpParams }).pipe(
      tap(res => this.followersIds.set(res.items))
    );
  }
  getFollowingIdsPaged(params?: FollowParams) {
    let httpParams = new HttpParams();
    if (params?.predicate) httpParams = httpParams.append('predicate', params.predicate);
    if (params?.pageNumber) httpParams = httpParams.append('pageNumber', params.pageNumber.toString());
    if (params?.pageSize) httpParams = httpParams.append('pageSize', params.pageSize.toString());
    return this.http.get<PaginatedResult<number>>(this.baseUrl + 'follow/follows-ids', { params: httpParams }).pipe(
      tap(res => this.followingIds.set(res.items))
    );
  }
  getFollowersPaged(params?: FollowParams) {
    let httpParams = new HttpParams();
    if (params?.predicate) httpParams = httpParams.append('predicate', params.predicate);
    if (params?.pageNumber) httpParams = httpParams.append('pageNumber', params.pageNumber.toString());
    if (params?.pageSize) httpParams = httpParams.append('pageSize', params.pageSize.toString());
    return this.http.get<PaginatedResult<Member>>(this.baseUrl + 'follow/follows-members', { params: httpParams }).pipe(
      tap(res => this.followersIds.set(res.items.map(m => m.id)))
    );
  }
  getFollowingPaged(params?: FollowParams) {
    let httpParams = new HttpParams();
    if (params?.predicate) httpParams = httpParams.append('predicate', params.predicate);
    if (params?.pageNumber) httpParams = httpParams.append('pageNumber', params.pageNumber.toString());
    if (params?.pageSize) httpParams = httpParams.append('pageSize', params.pageSize.toString());
    return this.http.get<PaginatedResult<Member>>(this.baseUrl + 'follow/follows-members', { params: httpParams }).pipe(
      tap(res => this.followingIds.set(res.items.map(m => m.id)))
    );
  }
  clearFollows() {
    this.followersIds.set([]);
    this.followingIds.set([]);
  }
}