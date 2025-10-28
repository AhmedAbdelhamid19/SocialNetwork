import { Component, inject, Signal, signal } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { filter, Observable, Subject, takeUntil } from 'rxjs';
import { Member } from '../../../types/member';


@Component({
  selector: 'app-member-detailed',
  imports: [RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './member-detailed.html',
  styleUrl: './member-detailed.css'
})

/*
  when you click on a member from the member list, you are navigated to this component
  which shows the details of the member.
  you can get the member id from the route params,
  and use the member service to get the member details by api call.
*/

export class MemberDetailed {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  protected member = signal<Member | undefined>(undefined);
  protected title = signal<string | undefined>('Profile');
 
  ngOnInit() {
    this.route.data.subscribe({
      next: (data) => {
        this.member.set(data['member']);
      }
    });
    this.title.set(this.route.firstChild?.snapshot?.title);

    // you subscribe to router events to update the title when the child route changes
    // which is in route outlet, because each child (messages, photos, profile) has title in app.routes.ts
    // and when you press on those links, the route changes and the child only replaced in the outlet
    // and you should update the title accordingly
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(() => {
      this.title.set(this.route.firstChild?.snapshot?.title);
    });
  }
}