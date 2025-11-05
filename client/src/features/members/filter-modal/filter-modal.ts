import { Component, ElementRef, output, ViewChild} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MemberParams } from '../../../types/member';

@Component({
  selector: 'app-filter-modal',
  imports: [FormsModule],
  templateUrl: './filter-modal.html',
  styleUrl: './filter-modal.css'
})

/*
  modal (not model) is just a small popup window that appears on top of the page.
*/
export class FilterModal {
  /*
    In the HTML, we give the dialog element a template reference variable like: <dialog #filterModal> ... </dialog>
    This allows us to access the dialog element in our TypeScript code using @ViewChild, this dialog used by DaisyUI for modals (not models).
    ! means that remove warning and i promise that this won't be null when accessed.
  */
  @ViewChild('filterModal') modelRef!: ElementRef<HTMLDialogElement>;
  closeModel = output();
  submitData = output<MemberParams>();
  filterParams = new MemberParams();
  ageError = '';

  /*
    These are native methods (showModal() and close()),
    not Angular-specific — they belong to the HTML <dialog> element.
  */
  openModal() {
    this.modelRef.nativeElement.showModal(); // built-in HTML dialog method
  }
  
  closeModal() {
    this.modelRef.nativeElement.close(); // built-in HTML dialog method
    this.closeModel.emit(); // send signal to parent that modal is closed
  }

  submitModal() {
    this.submitData.emit(this.filterParams);
    this.closeModal(); // close the modal after submitting
  }
  onMinAgeChange() {
    if (this.filterParams.minAge > this.filterParams.maxAge) {
      this.filterParams.minAge = this.filterParams.maxAge;
    }
    if(this.filterParams.minAge<18) {
      this.filterParams.minAge=18;
    }
  }
  onMaxAgeChange() {
    if (this.filterParams.maxAge < this.filterParams.minAge) {
      this.filterParams.maxAge = this.filterParams.minAge;
    }
    if(this.filterParams.maxAge>100) {
      this.filterParams.maxAge=100;
    }
  }
}
