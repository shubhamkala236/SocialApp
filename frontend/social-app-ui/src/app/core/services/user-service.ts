import { Injectable, signal, computed } from '@angular/core';
import { environment } from '../../../environments/environment';
import { FollowUser, LoginRequest, RegisterRequest, User, UserProfile } from '../models/user.model';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private readonly baseURL = environment.apiURL + '/users';
  private readonly storageKey = environment.storageKey;

  constructor(private http: HttpClient, private router: Router) {}

  getProfile(userId: string) {
    return this.http.get<UserProfile>(`${this.baseURL}/${userId}`);
  }

  syncProfile() {
    return this.http.post<UserProfile>(`${this.baseURL}/sync`, {});
  }

  updateProfile(data: FormData) {
    return this.http.put<UserProfile>(`${this.baseURL}/profile`, data);
  }

  getFollowers(userId: string) {
    return this.http.get<FollowUser[]>(`${this.baseURL}/${userId}/followers`);
  }

  getFollowing(userId: string) {
    return this.http.get<FollowUser[]>(`${this.baseURL}/${userId}/following`);
  }

  followUser(userId: string) {
    return this.http.post(`${this.baseURL}/${userId}/follow`, {});
  }

  unfollowUser(userId: string) {
    return this.http.delete(`${this.baseURL}/${userId}/follow`);
  }

  isFollowing(userId: string) {
    return this.http.get<{ isFollowing: boolean }>(
      `${this.baseURL}/${userId}/is-following`);
  }
  
}
