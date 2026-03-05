import { Routes } from '@angular/router';
import { authGuard, guestGuard } from './core/guards/auth-guard';

export const routes: Routes = [
  { path: '', redirectTo: 'posts', pathMatch: 'full' },
  {
    path: 'login',
    canActivate: [guestGuard],
    loadComponent: () => import('./features/login/login').then(m => m.Login)
  },
  {
    path: 'register',
    canActivate: [guestGuard],
    loadComponent: () =>
      import('./features/register/register').then(m => m.Register)
  },
  {
    path: 'posts',
    loadComponent: () =>
      import('./features/post-list/post-list').then(m => m.PostList)
  },
  {
    path: 'posts/create',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/post-create/post-create').then(m => m.PostCreate)
  },
  {
    path: 'posts/edit/:id',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/post-edit/post-edit').then(m => m.PostEdit)
  },
  {
    path: 'saved',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/saved-posts/saved-posts').then(m => m.SavedPostsComponent)
  },
  {
    path: 'profile/:userId',
    loadComponent: () =>
      import('./features/profile/profile').then(m => m.ProfileComponent)
  },
  { path: '**', redirectTo: 'posts' }
];

