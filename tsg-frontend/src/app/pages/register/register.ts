import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl, Validators } from '@angular/forms';
import { RegistrationService } from '../../core/register';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class RegisterComponent {
  private registrationService = inject(RegistrationService);

  registerForm = new FormGroup({
    firstName: new FormControl('', [Validators.required, Validators.minLength(2)]),
    lastName: new FormControl('', [Validators.required, Validators.minLength(2)]),
    email: new FormControl('', [Validators.required, Validators.email]),
    phoneNumber: new FormControl('', [Validators.required, Validators.pattern(/^[0-9+ ]{8,15}$/)]),
    cvUrl: new FormControl('', [Validators.required, Validators.pattern(/^(https?:\/\/)?([\da-z.-]+)\.([a-z.]{2,6})([\/\w .-]*)*\/?$/)])
  });

  loading = false;
  success = false;
  errorMsg = '';

  onSubmit() {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.errorMsg = '';

    this.registrationService.submitRegistration(this.registerForm.value).subscribe({
      next: (res) => {
        this.loading = false;
        this.success = true;
        this.registerForm.reset();
      },
      error: (err) => {
        this.loading = false;
        this.errorMsg = err.error?.message || 'A apărut o eroare la trimiterea înscrierii. Te rugăm să încerci din nou.';
      }
    });
  }
}
