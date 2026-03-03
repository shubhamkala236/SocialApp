import { Component, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { PostService } from '../../core/services/post-service';
import { AuthService } from '../../core/services/auth-service';
import { ThemeService } from '../../core/services/theme-service';
import { ToastService } from '../../core/services/toast.service';
import { Post } from '../../core/models/post.model';
import { PostCard } from '../post-card/post-card';

@Component({
  selector: 'app-post-list',
  imports: [
    CommonModule, RouterLink,
    MatButtonModule, MatIconModule,
    MatProgressSpinnerModule, MatDialogModule,
    PostCard
  ],
  templateUrl: './post-list.html',
  styleUrl: './post-list.scss',
})
export class PostList implements OnInit{
  constructor(
    private postService: PostService,
    public authService: AuthService,
    public themeService: ThemeService,
    public toastService: ToastService
  ) {}

  posts     = signal<Post[]>([]);
  isLoading = signal(true);
  error     = signal('');

  ngOnInit() {
    this.loadPosts();
  }

  loadPosts() {
    this.isLoading.set(true);
    this.postService.getAllPosts().subscribe({
      next: (posts) => {
        this.posts.set(posts);
        this.isLoading.set(false);
      },
      error: () => {
        this.error.set('Failed to load posts.');
        this.isLoading.set(false);
      }
    });
  }

  onDeletePost(id: string) {
    if (!confirm('Are you sure you want to delete this post?')) return;

    this.postService.deletePost(id).subscribe({
      next: () => {
        this.posts.update(posts => posts.filter(p => p.id !== id));
        this.toastService.success('Post deleted successfully.');
      },
      error: () => this.toastService.error('Failed to delete post.')
    });
  }
}
