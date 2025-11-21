import { Component, input, Self } from '@angular/core';
import { ControlValueAccessor, Form, FormControl, NgControl, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-text-input',
  imports: [ReactiveFormsModule],
  templateUrl: './text-input.html',
  styleUrl: './text-input.css'
})

export class TextInput implements ControlValueAccessor{
  label = input<string>('');
  type = input<string>('text');
  constructor(@Self() public ngControl: NgControl) {
    this.ngControl.valueAccessor = this;
  }
  writeValue(obj: any): void {
    // When the form itself changes, call this function
  }
  registerOnChange(fn: any): void {
    // When user types something, call this function
  }
  registerOnTouched(fn: any): void {
    // When user leaves the field, call this function
  }
  get control() {
    return this.ngControl.control as FormControl;
  }
}