import { Component, computed, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FollowService } from '../../core/services/follow-service';
import { MemberCard } from '../members/member-card/member-card';
import { Member } from '../../types/member';

@Component({
  selector: 'app-lists',
  imports: [MemberCard],
  standalone: true,
  templateUrl: './lists.html',
  styleUrl: './lists.css'
})
export class Lists {
  private followService = inject(FollowService);
  private followersSignal = signal<Member[]>([]);
  private followingSignal = signal<Member[]>([]);

  protected followers = computed(() => this.followersSignal());
  protected following = computed(() => this.followingSignal());
  protected mutualFollowers = computed(() => {
    return this.following().filter(member => 
      this.followers().some(follower => follower.id === member.id)
    );
  });

  constructor() {
    // Initialize the lists
    this.loadLists();
  }

  private loadLists() {
    this.followService.getFollowers().subscribe(followers => {
      this.followersSignal.set(followers);
    });
    
    this.followService.getFollowing().subscribe(following => {
      this.followingSignal.set(following);
    });
  }

  onFollowToggled(event: {memberId: number, isFollowing: boolean}) {
    if (event.isFollowing) {
      // Update following list
      this.followService.getFollowing().subscribe(following => {
        this.followingSignal.set(following);
      });
    } else {
      // Remove from following list
      this.followingSignal.update(list => 
        list.filter(member => member.id !== event.memberId)
      );
    }
  }
}
