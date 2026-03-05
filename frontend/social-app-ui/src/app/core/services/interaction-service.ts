import { Injectable, signal, computed } from '@angular/core';
import { environment } from '../../../environments/environment';
import { FollowUser, LoginRequest, RegisterRequest, User, UserProfile } from '../models/user.model';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { LikeResult, PostInteraction, SavedPost, SavePostRequest } from '../models/interaction.model';

@Injectable({
  providedIn: 'root',
})
export class InteractionService {
  private readonly baseURL = environment.apiURL + '/interactions';
  private readonly storageKey = environment.storageKey;

  constructor(private http: HttpClient, private router: Router) {}

  // ── Likes ──────────────────────────────────────
  toggleLike(postId: string) {
    return this.http.post<LikeResult>(
      `${this.baseURL}/posts/${postId}/like`, {});
  }

  getLikesCount(postId: string) {
    return this.http.get<{ likesCount: number }>(
      `${this.baseURL}/posts/${postId}/likes`);
  }

  // ── Saves ──────────────────────────────────────
  savePost(request: SavePostRequest) {
    return this.http.post(`${this.baseURL}/posts/save`, request);
  }

  unsavePost(postId: string) {
    return this.http.delete(`${this.baseURL}/posts/${postId}/save`);
  }

  getSavedPosts() {
    return this.http.get<SavedPost[]>(`${this.baseURL}/posts/saved`);
  }

  // ── Combined ───────────────────────────────────
  getPostInteractions(postId: string) {
    return this.http.get<PostInteraction>(
      `${this.baseURL}/posts/${postId}`);
  }

  getBatchInteractions(postIds: string[]) {
    return this.http.post<PostInteraction[]>(
      `${this.baseURL}/posts/batch`, postIds);
  }
  
}
