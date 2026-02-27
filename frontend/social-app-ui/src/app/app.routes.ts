import { Routes } from '@angular/router';
import { authGuard, guestGuard } from './core/guards/auth-guard';

export const routes: Routes = [
//   { path: '', redirectTo: 'posts', pathMatch: 'full' },
//   {
//     path: 'login',
//     canActivate: [guestGuard],
//     loadComponent: () =>
//       import('./features/auth/login/login.component').then(m => m.LoginComponent)
//   },
//   {
//     path: 'register',
//     canActivate: [guestGuard],
//     loadComponent: () =>
//       import('./features/auth/register/register.component').then(m => m.RegisterComponent)
//   },
//   {
//     path: 'posts',
//     canActivate: [authGuard],
//     loadComponent: () =>
//       import('./features/posts/post-list/post-list.component').then(m => m.PostListComponent)
//   },
//   { path: '**', redirectTo: 'posts' }
];

