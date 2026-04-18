import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class LoginComponent {
  form: FormGroup;

  loading = false;
  error = '';

  constructor(private fb: FormBuilder, private auth: AuthService, private router: Router) {
    this.form = this.fb.group({
      // Validators.required = câmp obligatoriu
      // Validators.email = format valid de email
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  get email() { return this.form.get('email')!; } //getter
  get password() { return this.form.get('password')!; } //getter

  onSubmit() {
    if (this.form.invalid) {
      return;
    }
    this.loading = true;
    this.error = '';


    this.auth.login(this.email!.value, this.password!.value).subscribe({
      next: (res) => {
        if (res.role === 'Admin') {
          this.router.navigate(['/admin']);
        } else {
          this.router.navigate(['/dashboard']);
        }
      },
      error: (err) => {
        this.error = err.error.message || 'Login failed';
        this.loading = false;
      }
    });
  }
}
