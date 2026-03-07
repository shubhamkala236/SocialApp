// src/app/features/posts/saved-posts/saved-posts.component.ts
import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatButtonModule } from '@angular/material/button';
import { InteractionService } from '../../core/services/interaction-service';
import { ThemeService } from '../../core/services/theme-service';
import { ToastService } from '../../core/services/toast.service';
import { SavedPost } from '../../core/models/interaction.model';

@Component({
  selector: 'app-saved-posts',
  standalone: true,
  imports: [
    CommonModule, RouterLink,
    MatIconModule, MatProgressSpinnerModule,
    MatButtonModule
  ],
  templateUrl: './saved-posts.html',
  styleUrl: './saved-posts.scss'
})
export class SavedPostsComponent implements OnInit {
  private intService = inject(InteractionService);
  private toast      = inject(ToastService);
  themeService       = inject(ThemeService);

  savedPosts = signal<SavedPost[]>([]);
  isLoading  = signal(true);

  ngOnInit() {
    this.intService.getSavedPosts().subscribe({
      next: (posts) => {
        this.savedPosts.set(posts);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  onUnsave(postId: string) {
    this.intService.unsavePost(postId).subscribe({
      next: () => {
        this.savedPosts.update(posts => posts.filter(p => p.postId !== postId));
        this.toast.success('Post removed from saved.');
      },
      error: () => this.toast.error('Failed to unsave post.')
    });
  }

  get timeAgo() {
    return (dateStr: string) => {
      const diff = Math.floor(
        (new Date().getTime() - new Date(dateStr).getTime()) / 1000);
      if (diff < 60)    return `${diff}s ago`;
      if (diff < 3600)  return `${Math.floor(diff / 60)}m ago`;
      if (diff < 86400) return `${Math.floor(diff / 3600)}h ago`;
      return `${Math.floor(diff / 86400)}d ago`;
    };
  }
}