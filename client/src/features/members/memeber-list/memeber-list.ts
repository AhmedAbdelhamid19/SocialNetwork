import { Component, inject, OnInit, signal } from '@angular/core';
import { MemberService } from '../../../core/services/member-service';
import { Observable } from 'rxjs';
import { Member } from '../../../types/member';
import { AsyncPipe } from '@angular/common';
import { MemberCard } from '../member-card/member-card';
import { PaginatedResult } from '../../../types/pagination';
import { Paginator } from "../../../shared/paginator/paginator";

@Component({
  selector: 'app-memeber-list',
  imports: [AsyncPipe, MemberCard, Paginator],
  templateUrl: './memeber-list.html',
  styleUrl: './memeber-list.css'
})
export class MemeberList implements OnInit {
onPageChange(arg0: any) {
throw new Error('Method not implemented.');
}
  private memberService = inject(MemberService);
  protected paginatedMembers = signal<PaginatedResult<Member> | null>(null);
  pageNumber = 1;
  pageSize = 5;

  ngOnInit(): void {
    this.loadMembers();
  }
  
  loadMembers() {
    this.memberService.getMembers(this.pageNumber, this.pageSize).subscribe({
      next: (members) => {
        this.paginatedMembers.set(members);
      },
      error: (error) => {
        console.error('Error loading members:', error);
      }
    });
  }
  onPageChanged(event: { pageNumber: number; pageSize: number; }) {
    this.pageNumber = event.pageNumber;
    this.pageSize = event.pageSize;
    this.loadMembers();
  }
}