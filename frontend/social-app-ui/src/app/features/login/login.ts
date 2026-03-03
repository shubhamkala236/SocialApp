import { Component, OnInit, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { AuthService } from '../../core/services/auth-service';
import { ToastService } from '../../core/services/toast.service';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ThemeService } from '../../core/services/theme-service';

@Component({
  selector: 'app-login',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatCardModule,
    MatDividerModule,
  ],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login implements OnInit {
  isLoading = signal(false);
  showPassword = signal(false);
  errorMessage = signal('');
  form!: FormGroup;

  constructor(
    private auth: AuthService,
    private toast: ToastService,
    private router: Router,
    public themeService : ThemeService
  ) {}

  ngOnInit(): void {
    this.buildForm();
  }

  buildForm() {
    this.form = new FormGroup({
      email: new FormControl('', [Validators.required, Validators.email]),
      password: new FormControl('', [Validators.required, Validators.minLength(6)]),
    });
  }

  get emailCtrl() {
    return this.form.controls['email'];
  }

  get passwordCtrl() {
    return this.form.controls['password'];
  }

  onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.isLoading.set(true);
    this.errorMessage.set('');
    
    this.auth.login(this.form.value as any).subscribe({
      next: (user) => {
        this.auth.persistUser(user);
        this.toast.success('Welcome back! 👋');
        this.router.navigate(['/posts']);
      },
      error: (err) => {
        this.errorMessage.set(err.error?.message || 'Invalid email or password.');
        this.isLoading.set(false);
      },
    });
  }
}
