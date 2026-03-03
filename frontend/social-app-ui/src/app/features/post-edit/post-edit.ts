import { Component, inject, signal, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { PostService } from '../../core/services/post-service';
import { ThemeService } from '../../core/services/theme-service';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-post-edit',
  imports: [
    ReactiveFormsModule, RouterLink,
    MatFormFieldModule, MatInputModule,
    MatButtonModule, MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './post-edit.html',
  styleUrl: './post-edit.scss',
})
export class PostEdit implements OnInit{
  constructor(
    private fb: FormBuilder,
    private postService: PostService,
    public themeService: ThemeService,
    private toastService: ToastService,
    private router: Router,
    private route: ActivatedRoute,
  ) {
    this.buildForm();
  }

  isLoading        = signal(false);
  isFetching       = signal(true);
  selectedImage    = signal<File | null>(null);
  imagePreview     = signal<string | null>(null);
  existingImageUrl = signal<string | null>(null);
  postId!: string;
  form!:FormGroup;

  buildForm() {
    this.form = this.fb.group({
      title:   ['', [Validators.required, Validators.minLength(3)]],
      content: ['', [Validators.required, Validators.minLength(10)]]
    });
  }

  get titleCtrl()   { return this.form.controls['title']; }
  get contentCtrl() { return this.form.controls['content']; }

  ngOnInit() {
    this.postId = this.route.snapshot.paramMap.get('id')!;

    this.postService.getPostById(this.postId).subscribe({
      next: (post) => {
        this.form.patchValue({ title: post.title, content: post.content });
        this.existingImageUrl.set(post.imageUrl ?? null);
        this.isFetching.set(false);
      },
      error: () => {
        this.toastService.error('Post not found.');
        this.router.navigate(['/posts']);
      }
    });
  }

  onImageSelected(event: Event) {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (!file) return;

    if (!file.type.startsWith('image/')) {
      this.toastService.error('Please select a valid image file.');
      return;
    }

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

    this.postService.updatePost(this.postId, {
      title:   this.form.value.title!,
      content: this.form.value.content!,
      image:   this.selectedImage() ?? undefined
    }).subscribe({
      next: () => {
        this.toastService.success('Post updated successfully!');
        this.router.navigate(['/posts']);
      },
      error: () => {
        this.toastService.error('Failed to update post.');
        this.isLoading.set(false);
      }
    });
  }
}
