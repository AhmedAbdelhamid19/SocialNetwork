import { Component, inject, OnInit, signal } from '@angular/core';
import { MemberService } from '../../../core/services/member-service';
import { Member, MemberParams } from '../../../types/member';
import { MemberCard } from '../member-card/member-card';
import { PaginatedResult } from '../../../types/pagination';
import { Paginator } from "../../../shared/paginator/paginator";

@Component({
  selector: 'app-memeber-list',
  imports: [MemberCard, Paginator],
  templateUrl: './memeber-list.html',
  styleUrl: './memeber-list.css'
})
export class MemeberList implements OnInit {
  private memberService = inject(MemberService);
  protected paginatedMembers = signal<PaginatedResult<Member> | null>(null);
  protected memberParams = new MemberParams();

  ngOnInit(): void {
    this.loadMembers();
  }
  
  loadMembers() {
    this.memberService.getMembers(this.memberParams).subscribe({
      next: (members) => {
        this.paginatedMembers.set(members);
      },
      error: (error) => {
        console.error('Error loading members:', error);
      }
    });
  }
  onPageChanged(event: { pageNumber: number, pageSize: number; }) {
    // event is output parameter from paginator component
    this.memberParams.pageNumber = event.pageNumber;
    this.memberParams.pageSize = event.pageSize;
    this.loadMembers();
  }
}