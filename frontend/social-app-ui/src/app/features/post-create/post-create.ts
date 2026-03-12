import { Component, inject, signal } from '@angular/core';
import { Form, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { PostService } from '../../core/services/post-service';
import { ThemeService } from '../../core/services/theme-service';
import { ToastService } from '../../core/services/toast.service';
import { AiAssistant, AIResult } from '../../shared/components/ai-assistant/ai-assistant';

@Component({
  selector: 'app-post-create',
  imports: [
    ReactiveFormsModule, RouterLink,
    MatFormFieldModule, MatInputModule,
    MatButtonModule, MatIconModule,
    MatProgressSpinnerModule,AiAssistant
  ],
  templateUrl: './post-create.html',
  styleUrl: './post-create.scss',
})
export class PostCreate {
  constructor(
    private fb: FormBuilder,
    private postService: PostService,
    public themeService: ThemeService,
    public toastService: ToastService,
    private router: Router
  ) {
    this.buildForm();
  }

  isLoading     = signal(false);
  selectedImage = signal<File | null>(null);
  imagePreview  = signal<string | null>(null);
  form!:FormGroup;
  showAiPanel = signal(false);

  buildForm(){
    this.form = this.fb.group({
      title:   ['', [Validators.required, Validators.minLength(3)]],
      content: ['', [Validators.required, Validators.minLength(10)]]
    });
  }

  onAiResult(result: AIResult) {
    console.log('📝 onAiResult called with:', result);  // ✅ add this
    
    this.form.patchValue({
      title:   result.title,
      content: result.content
    });

    console.log('📝 Form value after patch:', this.form.value); // ✅ add this
  }


  get titleCtrl()   { return this.form.controls['title']; }
  get contentCtrl() { return this.form.controls['content']; }

  onImageSelected(event: Event) {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (!file) return;

    // Validate file type
    if (!file.type.startsWith('image/')) {
      this.toastService.error('Please select a valid image file.');
      return;
    }

    // Validate file size (max 5MB)
    if (file.size > 5 * 1024 * 1024) {
      this.toastService.error('Image must be less than 5MB.');
      return;
    }

    this.selectedImage.set(file);
    const reader = new FileReader();
    reader.onload = () => this.imagePreview.set(reader.result as string);
    reader.readAsDataURL(file);
  }

  removeImage() {
    this.selectedImage.set(null);
    this.imagePreview.set(null);
  }

  onSubmit() {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.isLoading.set(true);

    this.postService.createPost({
      title:   this.form.value.title!,
      content: this.form.value.content!,
      image:   this.selectedImage() ?? undefined
    }).subscribe({
      next: () => {
        this.toastService.success('Post created successfully! 🎉');
        this.router.navigate(['/posts']);
      },
      error: () => {
        this.toastService.error('Failed to create post.');
        this.isLoading.set(false);
      }
    });
  }
}
