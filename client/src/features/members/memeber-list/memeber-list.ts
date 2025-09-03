import { Component, inject } from '@angular/core';
import { MemberService } from '../../../core/services/member-service';
import { Observable } from 'rxjs';
import { Member } from '../../../types/member';
import { AsyncPipe } from '@angular/common';
import { MemberCard } from '../member-card/member-card';

@Component({
  selector: 'app-memeber-list',
  imports: [AsyncPipe, MemberCard],
  templateUrl: './memeber-list.html',
  styleUrl: './memeber-list.css'
})
export class MemeberList {
  private memberService = inject(MemberService);
  protected members$: Observable<Member[]>;

  constructor() {
    this.members$ = this.memberService.getMembers();
  }
}
