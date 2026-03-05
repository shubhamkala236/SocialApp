// src/app/features/users/profile/profile.component.ts
import { Component, inject, signal, OnInit, computed } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTabsModule } from '@angular/material/tabs';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { UserService } from '../../core/services/user-service';
import { PostService } from '../../core/services/post-service';
import { InteractionService } from '../../core/services/interaction-service';
import { AuthService } from '../../core/services/auth-service';
import { ThemeService } from '../../core/services/theme-service';
import { ToastService } from '../../core/services/toast.service';
import { UserProfile, FollowUser } from '../../core/models/user.model';
import { Post } from '../../core/models/post.model';
import { PostInteraction } from '../../core/models/interaction.model';
import { PostCard } from '../post-card/post-card';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [
    CommonModule, RouterLink,
    MatIconModule, MatButtonModule,
    MatTabsModule, MatProgressSpinnerModule,
    PostCard
  ],
  templateUrl: './profile.html',
  styleUrl: './profile.scss'
})
export class ProfileComponent implements OnInit {
  private route       = inject(ActivatedRoute);
  private userService = inject(UserService);
  private postService = inject(PostService);
  private intService  = inject(InteractionService);
  private toast       = inject(ToastService);
  authService         = inject(AuthService);
  themeService        = inject(ThemeService);

  profile      = signal<UserProfile | null>(null);
  posts        = signal<Post[]>([]);
  interactions = signal<Map<string, PostInteraction>>(new Map());
  followers    = signal<FollowUser[]>([]);
  following    = signal<FollowUser[]>([]);

  isLoading        = signal(true);
  isFollowing      = signal(false);
  isFollowLoading  = signal(false);

  userId!: string;

  get isOwnProfile(): boolean {
    return this.authService.currentUser()?.userId === this.userId;
  }

  ngOnInit() {
    this.route.params.subscribe(params => {
      this.userId = params['userId'];
      this.loadProfile();
    });
  }

  loadProfile() {
    this.isLoading.set(true);

    this.userService.getProfile(this.userId).subscribe({
      next: (profile) => {
        this.profile.set(profile);
        this.isFollowing.set(profile.isFollowing);
        this.isLoading.set(false);
        this.loadUserPosts();
        this.loadFollowers();
        this.loadFollowing();
      },
      error: () => this.isLoading.set(false)
    });
  }

  loadUserPosts() {
    this.postService.getPostsByUser(this.userId).subscribe({
      next: (posts) => {
        this.posts.set(posts);
        if (posts.length > 0) {
          this.intService.getBatchInteractions(posts.map(p => p.id)).subscribe({
            next: (interactions) => {
              const map = new Map<string, PostInteraction>();
              interactions.forEach(i => map.set(i.postId, i));
              this.interactions.set(map);
            }
          });
        }
      }
    });
  }

  loadFollowers() {
    this.userService.getFollowers(this.userId).subscribe({
      next: (f) => this.followers.set(f)
    });
  }

  loadFollowing() {
    this.userService.getFollowing(this.userId).subscribe({
      next: (f) => this.following.set(f)
    });
  }

  onToggleFollow() {
    if (!this.authService.isLoggedIn()) {
      this.toast.error('Please login to follow users.');
      return;
    }

    this.isFollowLoading.set(true);
    const wasFollowing = this.isFollowing();

    // Optimistic update
    this.isFollowing.set(!wasFollowing);
    this.profile.update(p => p ? {
      ...p,
      followersCount: wasFollowing
        ? p.followersCount - 1
        : p.followersCount + 1
    } : p);

    const action = wasFollowing
      ? this.userService.unfollowUser(this.userId)
      : this.userService.followUser(this.userId);

    action.subscribe({
      next: () => {
        this.isFollowLoading.set(false);
        this.toast.success(wasFollowing ? 'Unfollowed.' : 'Following! 🎉');
        this.loadFollowers();
      },
      error: () => {
        // Revert
        this.isFollowing.set(wasFollowing);
        this.profile.update(p => p ? {
          ...p,
          followersCount: wasFollowing
            ? p.followersCount + 1
            : p.followersCount - 1
        } : p);
        this.isFollowLoading.set(false);
        this.toast.error('Failed. Please try again.');
      }
    });
  }

  onDeletePost(id: string) {
    if (!confirm('Delete this post?')) return;
    this.postService.deletePost(id).subscribe({
      next: () => {
        this.posts.update(posts => posts.filter(p => p.id !== id));
        this.toast.success('Post deleted.');
      }
    });
  }

  getInteraction(postId: string): PostInteraction | null {
    return this.interactions().get(postId) ?? null;
  }
}