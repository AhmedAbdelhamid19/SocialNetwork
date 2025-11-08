import { Component, computed, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FollowService } from '../../core/services/follow-service';
import { MemberCard } from '../members/member-card/member-card';

@Component({
  selector: 'app-lists',
  imports: [MemberCard],
  standalone: true,
  templateUrl: './lists.html',
  styleUrl: './lists.css'
})
export class Lists {
  private followService = inject(FollowService);
  protected followers = toSignal(this.followService.getFollowers(), { initialValue: [] });
  protected following = toSignal(this.followService.getFollowing(), { initialValue: [] });
  protected mutualFollowers = computed(() => {
    return this.following().filter(member => 
      this.followers().some(follower => follower.id === member.id)
    );
  });
}
