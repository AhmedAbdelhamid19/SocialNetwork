import { Component, inject, OnDestroy, OnInit, signal, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { EditMember, Member } from '../../../types/member';
import { DatePipe } from '@angular/common';
import { MemberService } from '../../../core/services/member-service';
import { FormsModule, NgForm } from '@angular/forms';
import { ToastService } from '../../../core/services/toast-service';

@Component({
  selector: 'app-member-profile',
  imports: [DatePipe, FormsModule],
  templateUrl: './member-profile.html',
  styleUrl: './member-profile.css'
})
export class MemberProfile implements OnInit, OnDestroy {
  // view child component named editForm and name it here editForm and make it fo type NgForm here
  // inside () must be same as the name of the template reference variable in the template file (stars with #).
  @ViewChild('editForm') editForm?: NgForm;
  protected memberService = inject(MemberService);
  private toast = inject(ToastService);
  private route = inject(ActivatedRoute);
  protected member = signal<Member | undefined>(undefined);
  protected editMember: EditMember = {
    displayName: '',
    description: '',
    city: '',
    country: ''
  };

  ngOnInit(): void {
    this.route.parent?.data.subscribe({
      next: (data) => {
        this.member.set(data['member']);
      }
    });
        // you set the edit member to the member properties to display the member properties in the edit form in User interface.
    this.editMember = {
      displayName: this.member()?.displayName || '',
      city: this.member()?.city || '',
      country: this.member()?.country || '',
      description: this.member()?.description || ''
    }
  }
  updateProfile() {
     if(!this.member() || !this.editForm) return;
     // you make ... to the member then editMember to make editMember properties override the member properties (clever way).
     const updatedMember = {...this.member(), ...this.editMember};
     console.log(updatedMember);
     this.toast.success('Profile updated successfully');
     // you set the edit mode to false to hide the edit form and show the profile again.
     this.memberService.editMode.set(false);
  }

  ngOnDestroy(): void {
    if(this.memberService.editMode()) {
       this.memberService.editMode.set(false);
    }
  }
}
