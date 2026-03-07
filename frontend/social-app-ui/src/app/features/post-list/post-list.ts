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
import { InteractionService } from '../../core/services/interaction-service';
import { PostInteraction } from '../../core/models/interaction.model';

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
    public toast: ToastService,
    private interactionService : InteractionService
  ) {}

  posts        = signal<Post[]>([]);
  interactions = signal<Map<string, PostInteraction>>(new Map());
  isLoading    = signal(true);
  error        = signal('');

  ngOnInit() { this.loadPosts(); }

  loadPosts() {
    this.isLoading.set(true);
    this.postService.getAllPosts().subscribe({
      next: (posts) => {
        this.posts.set(posts);
        console.log(this.posts());
        
        this.isLoading.set(false);
        if (posts.length > 0) this.loadInteractions(posts);
      },
      error: () => {
        this.error.set('Failed to load posts.');
        this.isLoading.set(false);
      }
    });
  }

  loadInteractions(posts: Post[]) {
    const postIds = posts.map(p => p.id);
    this.interactionService.getBatchInteractions(postIds).subscribe({
      next: (interactions) => {
        const map = new Map<string, PostInteraction>();
        interactions.forEach(i => map.set(i.postId, i));
        this.interactions.set(map);
      }
    });
  }

  getInteraction(postId: string): PostInteraction | null {
    return this.interactions().get(postId) ?? null;
  }

  onDeletePost(id: string) {
    if (!confirm('Are you sure you want to delete this post?')) return;

    this.postService.deletePost(id).subscribe({
      next: () => {
        this.posts.update(posts => posts.filter(p => p.id !== id));
        this.toast.success('Post deleted.');
      },
      error: () => this.toast.error('Failed to delete post.')
    });
  }
}
