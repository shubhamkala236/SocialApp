import { Injectable, signal, computed } from '@angular/core';
import { environment } from '../../../environments/environment';
import { LoginRequest, RegisterRequest, User } from '../models/user.model';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { CreatePostRequest, Post, UpdatePostRequest } from '../models/post.model';

@Injectable({
  providedIn: 'root',
})
export class PostService {
  private readonly baseURL = environment.apiURL + '/posts';
  private readonly storageKey = environment.storageKey;

  constructor(private http: HttpClient, private router: Router) {}
  
  getAllPosts() {
    return this.http.get<Post[]>(this.baseURL);
  }

  getPostById(id: string) {
    return this.http.get<Post>(`${this.baseURL}/${id}`);
  }

  getPostsByUser(userId: string) {
    return this.http.get<Post[]>(`${this.baseURL}/user/${userId}`);
  }

  createPost(request: CreatePostRequest) {
    const formData = this.buildFormData(request);
    return this.http.post<Post>(this.baseURL, formData);
  }

  updatePost(id: string, request: UpdatePostRequest) {
    const formData = this.buildFormData(request);
    return this.http.put<Post>(`${this.baseURL}/${id}`, formData);
  }

  deletePost(id: string) {
    return this.http.delete(`${this.baseURL}/${id}`);
  }

  private buildFormData(request: CreatePostRequest | UpdatePostRequest): FormData {
    const formData = new FormData();
    formData.append('title', request.title);
    formData.append('content', request.content);
    if (request.image) formData.append('image', request.image);
    return formData;
  }
}
