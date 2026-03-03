import { Component, inject, OnInit, signal } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators,
  AbstractControl,
  ValidationErrors,
  FormGroup,
  FormControl,
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../core/services/auth-service';
import { ToastService } from '../../core/services/toast.service';
import { ThemeService } from '../../core/services/theme-service';

function passwordsMatch(ctrl: AbstractControl): ValidationErrors | null {
  const pw = ctrl.get('password')?.value;
  const cpw = ctrl.get('confirmPassword')?.value;
  return pw === cpw ? null : { passwordMismatch: true };
}

@Component({
  selector: 'app-register',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './register.html',
  styleUrl: './register.scss',
})
export class Register implements OnInit {
  form!: FormGroup;
  isLoading = signal(false);
  showPassword = signal(false);
  errorMessage = signal('');

  constructor(
    private auth: AuthService,
    private toast: ToastService,
    private router: Router,
    private fb: FormBuilder,
    public themeService : ThemeService, 
  ) {}

  ngOnInit(): void {
    this.buildForm();
  }

  buildForm() {
    this.form = this.fb.group(
      {
        username: ['', [Validators.required, Validators.minLength(3)]],
        email: ['', [Validators.required, Validators.email]],
        password: ['', [Validators.required, Validators.minLength(6)]],
        confirmPassword: ['', Validators.required],
      },
      { validators: passwordsMatch },
    );
  }

  get usernameCtrl() {
    return this.form.controls['username'];
  }
  get emailCtrl() {
    return this.form.controls['email'];
  }
  get passwordCtrl() {
    return this.form.controls['password'];
  }
  get confirmPasswordCtrl() {
    return this.form.controls['confirmPassword'];
  }

  onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.isLoading.set(true);
    this.errorMessage.set('');

    const { username, email, password } = this.form.value;

    this.auth.register({ username: username!, email: email!, password: password! }).subscribe({
      next: (user) => {
        this.auth.persistUser(user);
        this.toast.success('Account created! Welcome 🎉');
        this.router.navigate(['/posts']);
      },
      error: (err) => {
        this.errorMessage.set(err.error?.message || 'Registration failed. Try again.');
        this.isLoading.set(false);
      },
    });
  }
}
