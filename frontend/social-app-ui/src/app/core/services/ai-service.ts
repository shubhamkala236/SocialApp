// src/app/core/services/ai.service.ts
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export type PostTone = 'Professional' | 'Casual' | 'Funny' | 'Inspirational' | 'Storytelling';

export interface AIPostResponse {
  title: string;
  content: string;
  actionTaken: string;
}

export interface GeneratePostRequest {
  idea: string;
  tone: PostTone;
}

export interface ImprovePostRequest {
  title: string;
  content: string;
  tone: PostTone;
}

export interface RephrasePostRequest {
  content: string;
  tone: PostTone;
}

@Injectable({ providedIn: 'root' })
export class AiService {
  private readonly baseURL = environment.apiURL + '/ai/posts';
  private readonly storageKey = environment.storageKey;
  private http   = inject(HttpClient);

  generatePost(req: GeneratePostRequest): Observable<AIPostResponse> {
    return this.http.post<AIPostResponse>(`${this.baseURL}/generate`, req);
  }

  improvePost(req: ImprovePostRequest): Observable<AIPostResponse> {
    return this.http.post<AIPostResponse>(`${this.baseURL}/improve`, req);
  }

  rephrasePost(req: RephrasePostRequest): Observable<AIPostResponse> {
    return this.http.post<AIPostResponse>(`${this.baseURL}/rephrase`, req);
  }

  summarizePost(content: string): Observable<AIPostResponse> {
    return this.http.post<AIPostResponse>(`${this.baseURL}/summarize`, JSON.stringify(content), {
      headers: { 'Content-Type': 'application/json' }
    });
  }

  makeHook(content: string): Observable<AIPostResponse> {
    return this.http.post<AIPostResponse>(`${this.baseURL}/hook`, JSON.stringify(content), {
      headers: { 'Content-Type': 'application/json' }
    });
  }
}