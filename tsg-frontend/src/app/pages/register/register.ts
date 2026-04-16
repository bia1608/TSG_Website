import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule], // IMPORTANTE!
  templateUrl: './register.html',
  template: '<h1>Register</h1>',
  styleUrl: './register.css'
})
export class RegisterComponent {
  registerForm = new FormGroup({
    firstName: new FormControl('', [Validators.required]),
    lastName: new FormControl('', [Validators.required]),
    email: new FormControl('', [Validators.required, Validators.email]),
    phoneNumber: new FormControl('', [Validators.required]),
    cvUrl: new FormControl('')
  });

  onSubmit() {
    if (this.registerForm.valid) {
      console.log('Date trimise:', this.registerForm.value);
      alert('Înregistrare reușită (vizual)!');
    }
  }
}
