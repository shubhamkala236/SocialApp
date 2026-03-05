import { Component, Input, Output, EventEmitter, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { Post } from '../../core/models/post.model';
import { AuthService } from '../../core/services/auth-service';
import { ThemeService } from '../../core/services/theme-service';
import { PostInteraction } from '../../core/models/interaction.model';
import { InteractionService } from '../../core/services/interaction-service';
import { ToastService } from '../../core/services/toast.service';
import { MatTooltipModule } from '@angular/material/tooltip';

@Component({
  selector: 'app-post-card',
  imports: [CommonModule, RouterLink, MatIconModule, MatButtonModule, MatMenuModule, MatTooltipModule],
  templateUrl: './post-card.html',
  styleUrl: './post-card.scss',
})
export class PostCard {
  @Input() post!: Post;
  @Input() interaction: PostInteraction | null = null;
  @Output() deletePost = new EventEmitter<string>();

  constructor(private authService: AuthService, public themeService: ThemeService, public intService: InteractionService, public toast: ToastService) {}
  
  // ── Signals ──────────────────────────────────
  isLiked    = signal(false);
  isSaved    = signal(false);
  likesCount = signal(0);
  isLiking   = signal(false);
  isSaving   = signal(false);

  get isOwner(): boolean {
    return this.authService.currentUser()?.userId === this.post.userId;
  }

  get timeAgo(): string {
    const now  = new Date();
    const date = new Date(this.post.createdAt);
    const diff = Math.floor((now.getTime() - date.getTime()) / 1000);
    if (diff < 60)    return `${diff}s ago`;
    if (diff < 3600)  return `${Math.floor(diff / 60)}m ago`;
    if (diff < 86400) return `${Math.floor(diff / 3600)}h ago`;
    return `${Math.floor(diff / 86400)}d ago`;
  }

  ngOnInit() {
    if (this.interaction) {
      this.isLiked.set(this.interaction.isLiked);
      this.isSaved.set(this.interaction.isSaved);
      this.likesCount.set(this.interaction.likesCount);
    }
  }

  onToggleLike() {
    if (!this.authService.isLoggedIn()) {
      this.toast.error('Please login to like posts.');
      return;
    }
    if (this.isLiking()) return;

    // Optimistic update
    const wasLiked = this.isLiked();
    this.isLiked.set(!wasLiked);
    this.likesCount.update(c => wasLiked ? c - 1 : c + 1);
    this.isLiking.set(true);

    this.intService.toggleLike(this.post.id).subscribe({
      next: (result) => {
        this.isLiked.set(result.isLiked);
        this.likesCount.set(result.likesCount);
        this.isLiking.set(false);
      },
      error: () => {
        // Revert optimistic update
        this.isLiked.set(wasLiked);
        this.likesCount.update(c => wasLiked ? c + 1 : c - 1);
        this.isLiking.set(false);
        this.toast.error('Failed to like post.');
      }
    });
  }

  onToggleSave() {
    if (!this.authService.isLoggedIn()) {
      this.toast.error('Please login to save posts.');
      return;
    }
    if (this.isSaving()) return;

    const wasSaved = this.isSaved();
    this.isSaved.set(!wasSaved);
    this.isSaving.set(true);

    const action = wasSaved
      ? this.intService.unsavePost(this.post.id)
      : this.intService.savePost({
          postId:       this.post.id,
          postTitle:    this.post.title,
          postContent:  this.post.content,
          postUsername: this.post.username,
          postImageUrl: this.post.imageUrl,
          postCreatedAt: this.post.createdAt
        });

    action.subscribe({
      next: () => {
        this.isSaving.set(false);
        this.toast.success(wasSaved ? 'Post unsaved.' : 'Post saved! 🔖');
      },
      error: () => {
        this.isSaved.set(wasSaved);
        this.isSaving.set(false);
        this.toast.error('Failed to save post.');
      }
    });
  }
}