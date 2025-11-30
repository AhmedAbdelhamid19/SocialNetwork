import { Component, computed, inject, input, OnInit, output, Signal, signal } from '@angular/core';
import { Member } from '../../../types/member';
import { RouterLink } from '@angular/router';
import { FollowService } from '../../../core/services/follow-service';
import { ToastService } from '../../../core/services/toast-service';
import { TimeAgoPipe } from '../../../core/pipes/time-ago-pipe';
import { PresenceService } from '../../../core/services/presence-service';

@Component({
  selector: 'app-member-card',
  imports: [RouterLink, TimeAgoPipe],
  templateUrl: './member-card.html',
  styleUrl: './member-card.css'
})
export class MemberCard implements OnInit {
  private followService = inject(FollowService);
  private toastService = inject(ToastService);
  private presenceService = inject(PresenceService);
  member = input.required<Member>();
  isFollowing = signal<boolean>(false);
  protected isOnline = computed(() => {
    return this.presenceService.onlineUsers().map(id => Number(id)).includes(this.member().id);
  });
  ngOnInit(): void {
    this.isFollowing.set(this.followService.followingIds().includes(this.member().id))
  }
  toggleFollow() {
    this.followService.toggleFollow(this.member().id).subscribe({
      next: () => {
        if (this.isFollowing()) {
          this.followService.followingIds.set(
            this.followService.followingIds().filter(id => id !== this.member().id)
          );
        } else {
          this.followService.followingIds.set([
            ...this.followService.followingIds(), this.member().id
          ]);
        }
        this.isFollowing.set(!this.isFollowing());
      },
      error: (error) => {
        this.toastService.error('Failed to toggle follow status. Please try again.');
      }
    });
  }
}