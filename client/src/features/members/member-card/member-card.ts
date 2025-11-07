import { Component, computed, inject, input } from '@angular/core';
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
  isFollowee = computed(() => {
    return this.followService.followeesIds().includes(this.member().id);
  });

  toggleFollow() {
    this.followService.toggleFollow(this.member().id).subscribe({
      next: () => {
        if (this.isFollowee()) {
          this.followService.followeesIds.set(
            this.followService.followeesIds().filter(id => id !== this.member().id)
          );
        } else {
          this.followService.followeesIds.set([
            ...this.followService.followeesIds(),
            this.member().id
          ]);
        }
      },
      error: (error) => {
        console.error('Error toggling follow status:', error);
      }
    });
  }
}
