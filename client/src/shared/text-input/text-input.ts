import { Component, input, Self } from '@angular/core';
import { ControlValueAccessor, Form, FormControl, NgControl, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-text-input',
  imports: [ReactiveFormsModule],
  templateUrl: './text-input.html',
  styleUrl: './text-input.css'
})

// ControlValueAccessor allows this custom component to work with Angular forms 
// It defines how the component should interact with Angular's form APIs 
// like (writeValue, registerOnChange, registerOnTouched ...)
export class TextInput implements ControlValueAccessor{
  // label and type (angular input prop) will be passed from 
  // the parent component where this component is used
  label = input<string>('');
  type = input<string>('text');
  constructor(@Self() public ngControl: NgControl) {
    // @Self() ensures Angular looks for NgControl only on this component not parent
    this.ngControl.valueAccessor = this;
  }


  // When the form itself changes, call this function
  writeValue(obj: any): void {

  }
  // When user types something, call this function
  registerOnChange(fn: any): void {
    
  }
  // When user leaves the field, call this function
  registerOnTouched(fn: any): void {
    
  }

  // used to attach it in the form in html to spcify which form control this input is bound to,
  // and tell the html that angular form is controlling this input
  get control() {
    return this.ngControl.control as FormControl;
  }
}
