import { Component, inject, OnInit, signal, ViewChild } from '@angular/core';
import { MemberService } from '../../../core/services/member-service';
import { Member, MemberParams } from '../../../types/member';
import { MemberCard } from '../member-card/member-card';
import { PaginatedResult } from '../../../types/pagination';
import { Paginator } from "../../../shared/paginator/paginator";
import { FilterModal } from '../filter-modal/filter-modal';

@Component({
  selector: 'app-memeber-list',
  imports: [MemberCard, Paginator, FilterModal],
  templateUrl: './memeber-list.html',
  styleUrl: './memeber-list.css'
})
export class MemeberList implements OnInit {
  // We use ViewChild to get a reference to the FilterModal component instance
  // so you can control it (open/close functions inside the child component) from this parent component.
  @ViewChild('filterModal') modal!: FilterModal;
  private memberService = inject(MemberService);
  protected paginatedMembers = signal<PaginatedResult<Member> | null>(null);
  protected memberParams = new MemberParams();


  ngOnInit(): void {
    const filters = localStorage.getItem('filters');
    if (filters) {
      this.memberParams = JSON.parse(filters);
    }
    this.loadMembers();
  }
  
  loadMembers() {
    console.log('Loading members with params:', this.memberParams);
    this.memberService.getMembers(this.memberParams).subscribe({
      next: (members) => {
        console.log('Received members:', members);
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

  openModal() {
    this.modal.openModal(); // call open() in FilterModalComponent
  }
  closeModal () {
    console.log('Filter modal closed');
    this.modal.closeModal();
  }
  onFilterChanged(newParams: MemberParams) {
    // when filter modal submit new params with submit button, we update memberParams and reload members
    this.memberParams = newParams;
    this.loadMembers();
  }
  resetFilters() {
    this.memberParams = new MemberParams();
    this.loadMembers();
  }
}