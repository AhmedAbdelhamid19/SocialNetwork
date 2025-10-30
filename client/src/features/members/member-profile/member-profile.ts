import { Component, HostListener, inject, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { EditMember, Member } from '../../../types/member';
import { DatePipe } from '@angular/common';
import { MemberService } from '../../../core/services/member-service';
import { FormsModule, NgForm } from '@angular/forms';
import { ToastService } from '../../../core/services/toast-service';
import { AccountService } from '../../../core/services/account-service';

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
  @HostListener('window:beforeunload', ['$event']) notify($event: BeforeUnloadEvent) {
    if (this.editForm?.dirty) {
      $event.preventDefault();              // optional in modern browsers
    }
  }
  protected memberService = inject(MemberService);
  private accountService = inject(AccountService);
  private toast = inject(ToastService);
  protected editMember: EditMember = {
    displayName: '',
    description: '',
    city: '',
    country: ''
  };

  ngOnInit(): void {
    // you set the edit member from the member properties to display the member properties in the edit form in User interface.
    this.editMember = {
      displayName: this.memberService.member()?.displayName || '',
      city: this.memberService.member()?.city || '',
      country: this.memberService.member()?.country || '',
      description: this.memberService.member()?.description || ''
    }
  }
  updateProfile() {
    if(!this.memberService.member()) return;

    // you make ... to the member then editMember to make editMember properties override the member properties (clever way).
    const updatedMember = {...this.memberService.member(), ...this.editMember};
    this.memberService.updateMember(updatedMember).subscribe({
      next: () => {
        const currentUser = this.accountService.currentUser();
        if(currentUser && currentUser?.displayName !== updatedMember?.displayName) {
          currentUser.displayName = updatedMember.displayName;
          this.accountService.setCurrentUser(currentUser);
        }
        this.toast.success('Profile updated successfully');
        // you set the edit mode to false to hide the edit form and show the profile again.
        this.memberService.editMode.set(false);
        this.memberService.member.set(updatedMember as Member);
        // we reset it to remove dirty flag in form becasue without these the deactivate gateway won't make us to continue to another page.
        this.editForm?.reset(updatedMember); 
      }
    });
  }

  ngOnDestroy(): void {
    if(this.memberService.editMode()) {
       this.memberService.editMode.set(false);
    }
  }
}
