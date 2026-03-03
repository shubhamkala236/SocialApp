import { Injectable, signal, effect, untracked } from '@angular/core';
import { environment } from '../../../environments/environment';

export type Theme = 'dark' | 'light';

@Injectable({
  providedIn: 'root',
})
export class ThemeService {
  private readonly themeKey = environment.themeKey;

  theme = signal<Theme>(this.loadTheme());

  constructor() {
    // Apply theme to DOM whenever signal changes
    effect(() => {
      this.applyTheme(this.theme());
    });
  }

  toggleTheme() {
    this.theme.set(this.theme() === 'dark' ? 'light' : 'dark');
    localStorage.setItem(this.themeKey, this.theme());
  }

  isDark() {
    return this.theme() === 'dark';
  }

  applyTheme(theme: Theme) {
    const html = document.documentElement;
    if (theme === 'dark') {
      html.classList.add('dark');
      html.classList.remove('light');
    } else {
      html.classList.add('light');
      html.classList.remove('dark');
    }
  }

  loadTheme() : Theme  {
    const saved = localStorage.getItem(this.themeKey) as Theme;
    if (saved) return saved;
    // Default to system preference
    return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
  }
}
