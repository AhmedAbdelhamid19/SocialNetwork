import { Component, inject, input, OnInit, output, signal } from '@angular/core';
import { AbstractControl, Form, FormBuilder, FormControl, FormGroup, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { RegisterCreds, User } from '../../../types/user';
import { AccountService } from '../../../core/services/account-service';
import { JsonPipe } from '@angular/common';
import { TextInput } from "../../../shared/text-input/text-input";

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, TextInput],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register {
  private accountService = inject(AccountService);
  cancelRegister = output<boolean>();
  protected creds = {} as RegisterCreds;

  private formBuilder = inject(FormBuilder);
  protected credentialForm: FormGroup;
  protected profileForm: FormGroup;
  protected currentStep = signal(1);
  
  constructor() {
    this.credentialForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      displayName: ['', [Validators.required]],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', [Validators.required, this.matching('password')]]
    });
    this.credentialForm.controls['password'].valueChanges.subscribe(() => {
      // this mean when the password field changes, we need to run validators in the confirm password field (custom validator ...).
      this.credentialForm.controls['confirmPassword'].updateValueAndValidity();
    });

    this.profileForm = this.formBuilder.group({
      gender: ['', [Validators.required]],
      dateOfBirth: ['', [Validators.required]],
      city: ['', [Validators.required]],
      country: ['', [Validators.required]]
    });
  }
  matching(otherField: string) {
    return (control: AbstractControl): ValidationErrors | null => {
      // here control refer to the field where this validator is applied (the input field that you will put this validator on)
      const parent = control.parent;
      if(!parent) return null;
      const otherFieldValue = parent.get(otherField)?.value;
      return control.value === otherFieldValue ? null : { notMatching: true };
    }
  }
  nextStep() {
    if(this.credentialForm.valid) {
      this.currentStep.update(n => n + 1);
    }
  }
  previousStep() {

    this.currentStep.update(n => n + 1);
  }
  register() {
    if(this.credentialForm.valid && this.profileForm.valid) {
      console.log('form', {...this.credentialForm.value, ...this.profileForm.value});
    }
    // this.accountService.register(this.creds).subscribe({
    //   next: response => {
    //     console.log(response);
    //     this.cancel();
    //   },
    //   error: error => {
    //     console.log(error);
    //   }
    // })
  }
  cancel() {
    this.cancelRegister.emit(false);
  }
}
