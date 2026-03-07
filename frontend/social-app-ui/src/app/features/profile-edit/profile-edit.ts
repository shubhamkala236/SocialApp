import { Component, inject, signal, OnInit } from '@angular/core';
import { FormBuilder, FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTabsModule } from '@angular/material/tabs';
import { UserService } from '../../core/services/user-service';
import { AuthService } from '../../core/services/auth-service';
import { ThemeService } from '../../core/services/theme-service';
import { ToastService } from '../../core/services/toast.service';
import { UserProfile } from '../../core/models/user.model';

@Component({
  selector: 'app-profile-edit',
  imports: [
    CommonModule,
    RouterLink,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatTabsModule,
  ],
  templateUrl: './profile-edit.html',
  styleUrl: './profile-edit.scss',
})
export class ProfileEdit {
  private userService = inject(UserService);
  private router = inject(Router);
  private toast = inject(ToastService);
  authService = inject(AuthService);
  themeService = inject(ThemeService);

  profile = signal<UserProfile | null>(null);
  isLoading = signal(true);
  isSaving = signal(false);
  selectedAvatar = signal<File | null>(null);
  avatarPreview = signal<string | null>(null);

  bioCtrl = new FormControl('', [Validators.maxLength(160)]);

  ngOnInit() {
    const userId = this.authService.userId();
    if (!userId) {
      this.router.navigate(['/login']);
      return;
    }

    this.userService.getProfile(userId).subscribe({
      next: (profile) => {
        this.profile.set(profile);
        this.bioCtrl.setValue(profile.bio ?? '');
        this.isLoading.set(false);
      },
      error: () => {
        this.toast.error('Failed to load profile.');
        this.isLoading.set(false);
      },
    });
  }

  onAvatarSelected(event: Event) {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (!file) return;

    if (!file.type.startsWith('image/')) {
      this.toast.error('Please select a valid image.');
      return;
    }

    if (file.size > 5 * 1024 * 1024) {
      this.toast.error('Image must be under 5MB.');
      return;
    }

    this.selectedAvatar.set(file);
    const reader = new FileReader();
    reader.onload = () => this.avatarPreview.set(reader.result as string);
    reader.readAsDataURL(file);
  }

  removeAvatar() {
    this.selectedAvatar.set(null);
    this.avatarPreview.set(null);
  }

  onSave() {
    if (this.bioCtrl.invalid) return;
    this.isSaving.set(true);

    const formData = new FormData();
    formData.append('bio', this.bioCtrl.value ?? '');
    if (this.selectedAvatar()) {
      formData.append('avatar', this.selectedAvatar()!);
    }

    this.userService.updateProfile(formData).subscribe({
      next: (updated) => {
        this.profile.set(updated);
        this.selectedAvatar.set(null);
        this.avatarPreview.set(null);
        this.isSaving.set(false);
        this.toast.success('Profile updated! ✅');
      },
      error: () => {
        this.toast.error('Failed to update profile.');
        this.isSaving.set(false);
      },
    });
  }
}
