import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { FollowService } from '../../core/services/follow-service';
import { MemberCard } from '../members/member-card/member-card';
import { Member } from '../../types/member';
import { FollowParams, PaginatedResult } from '../../types/pagination';
import { Paginator } from '../../shared/paginator/paginator';

@Component({
  selector: 'app-lists',
  imports: [MemberCard, Paginator],
  standalone: true,
  templateUrl: './lists.html',
  styleUrl: './lists.css'
})
export class Lists implements OnInit {
  private followService = inject(FollowService);

  // member lists and pagination metadata
  protected followers = signal<Member[]>([]);
  protected following = signal<Member[]>([]);
  protected followersMeta = signal({ pageNumber: 1, pageSize: 5, totalCount: 0, totalPages: 0 });
  protected followingMeta = signal({ pageNumber: 1, pageSize: 5, totalCount: 0, totalPages: 0 });
  protected mutualFollowers = computed(() => {
    return this.following().filter((member: Member) =>
      this.followers().some((follower: Member) => follower.id === member.id)
    );
  });


  ngOnInit(): void {
    // initial load using default meta (page 1)
    this.loadFollowersPage(this.followersMeta().pageNumber, this.followersMeta().pageSize);
    this.loadFollowingPage(this.followingMeta().pageNumber, this.followingMeta().pageSize);
  }
  private loadFollowersPage(page: number, pageSize: number) {
    const params: FollowParams = { predicate: 'followers', pageNumber: page, pageSize };
    this.followService.getFollowersPaged(params).subscribe({
      next: (res: PaginatedResult<Member>) => {
        this.followers.set(res.items);
        this.followersMeta.set({
          pageNumber: params.pageNumber ?? 1,
          pageSize: params.pageSize ?? 10,
          totalCount: res.metadata?.totalCount ?? 0,
          totalPages: res.metadata?.totalPages ?? 0
        });
      },
      error: () => {
        this.followers.set([]);
        this.followersMeta.set({ pageNumber: 1, pageSize: pageSize, totalCount: 0, totalPages: 0 });
      }
    });
  }
  private loadFollowingPage(page: number, pageSize: number) {
    const params: FollowParams = { predicate: 'following', pageNumber: page, pageSize };
    this.followService.getFollowingPaged(params).subscribe({
      next: (res: PaginatedResult<Member>) => {
        this.following.set(res.items);
        this.followingMeta.set({
          pageNumber: params.pageNumber ?? 1,
          pageSize: params.pageSize ?? 10,
          totalCount: res.metadata?.totalCount ?? 0,
          totalPages: res.metadata?.totalPages ?? 0
        });
      },
      error: () => {
        this.following.set([]);
        this.followingMeta.set({ pageNumber: 1, pageSize: pageSize, totalCount: 0, totalPages: 0 });
      }
    });
  }
  // paginator handlers (wired from template)
  onFollowersPageChange(event: { pageNumber: number; pageSize: number }) {
    this.loadFollowersPage(event.pageNumber, event.pageSize);
  }
  onFollowingPageChange(event: { pageNumber: number; pageSize: number }) {
    this.loadFollowingPage(event.pageNumber, event.pageSize);
  }
}