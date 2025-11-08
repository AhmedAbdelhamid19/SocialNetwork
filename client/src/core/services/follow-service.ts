import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { tap } from 'rxjs';
import { Member } from '../../types/member';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class FollowService {
  private baseUrl = environment.apiUrl;
  private http = inject (HttpClient);
  followersIds = signal<number[]>([]);
  followingIds = signal<number[]>([]);

  toggleFollow(targetMemberId: number) {
    return this.http.post(this.baseUrl + `follow/toggle-follow/${targetMemberId}`, {});
  }
  
  getFollowersIds() {
    return this.http.get<number[]>(this.baseUrl + `follow/follows-ids?predicate=followers`).pipe(
      tap(ids => {
        this.followersIds.set(ids);
      })
    );
  }
  getFollowingIds() {
    return this.http.get<number[]>(this.baseUrl + `follow/follows-ids?predicate=following`).pipe(
      tap(ids => {
        this.followingIds.set(ids);
      })
    );
  }
  getFollowers() {
    return this.http.get<Member[]>(this.baseUrl + `follow/follows-members?predicate=followers`);
  }
  getFollowing() {
    return this.http.get<Member[]>(this.baseUrl + `follow/follows-members?predicate=following`);
  }
  clearFollows() {
    this.followersIds.set([]);
    this.followingIds.set([]);
  }
}
