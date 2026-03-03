import { Component, Input, Output, EventEmitter, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { Post } from '../../core/models/post.model';
import { AuthService } from '../../core/services/auth-service';
import { ThemeService } from '../../core/services/theme-service';

@Component({
  selector: 'app-post-card',
  imports: [CommonModule, RouterLink, MatIconModule, MatButtonModule, MatMenuModule],
  templateUrl: './post-card.html',
  styleUrl: './post-card.scss',
})
export class PostCard {
  @Input() post!: Post;
  @Output() deletePost = new EventEmitter<string>();

  constructor(private authService: AuthService, public themeService: ThemeService) {}

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

}
