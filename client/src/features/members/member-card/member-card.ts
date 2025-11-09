import { Component, computed, inject, input, output } from '@angular/core';
import { Member } from '../../../types/member';
import { RouterLink } from '@angular/router';
import { AgePipe } from '../../../core/pipes/age-pipe';
import { FollowService } from '../../../core/services/follow-service';

@Component({
  selector: 'app-member-card',
  imports: [RouterLink, AgePipe],
  templateUrl: './member-card.html',
  styleUrl: './member-card.css'
})
export class MemberCard {
  private followService = inject(FollowService);
  member = input.required<Member>();
  followToggled = output<{memberId: number, isFollowing: boolean}>();
  
  isFollowing = computed(() => {
    return this.followService.followingIds().includes(this.member().id);
  });

  toggleFollow() {
    this.followService.toggleFollow(this.member().id).subscribe({
      next: () => {
        const wasFollowing = this.isFollowing();
        if (wasFollowing) {
          this.followService.followingIds.set(
            this.followService.followingIds().filter(id => id !== this.member().id)
          );
        } else {
          this.followService.followingIds.set([
            ...this.followService.followingIds(),
            this.member().id
          ]);
        }
        // Emit the change event
        this.followToggled.emit({
          memberId: this.member().id,
          isFollowing: !wasFollowing
        });
      },
      error: (error) => {
        console.error('Error toggling follow:', error);
      }
    });
  }
}
