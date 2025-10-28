import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { single } from 'rxjs';
import { Member } from '../../../types/member';

@Component({
  selector: 'app-member-profile',
  imports: [],
  templateUrl: './member-profile.html',
  styleUrl: './member-profile.css'
})
export class MemberProfile implements OnInit {
  private route = inject(ActivatedRoute);
  protected member = signal<Member | undefined>(undefined);

  ngOnInit(): void {
    this.route.parent?.data.subscribe({
      next: (data) => {
        this.member.set(data['member']);
      }
    });
  }
}
