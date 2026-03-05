export interface User {
  userId: string;
  username: string;
  token: string;
}

export interface UserProfile {
  userId: string;
  username: string;
  email: string;
  bio?: string;
  avatarUrl?: string;
  followersCount: number;
  followingCount: number;
  isFollowing: boolean;
  createdAt: string;
}

export interface FollowUser {
  userId: string;
  username: string;
  avatarUrl?: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
}