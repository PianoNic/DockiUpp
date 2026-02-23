import { Injectable, effect, signal } from '@angular/core';

export type Theme = 'light' | 'dark' | 'auto';

@Injectable({
  providedIn: 'root',
})
export class ThemeService {
  private currentTheme = signal<Theme>(this.getStoredTheme());

  constructor() {
    effect(() => {
      const theme = this.currentTheme();
      if (theme === 'auto') {
        document.documentElement.style.colorScheme = 'light dark';
      } else {
        document.documentElement.style.colorScheme = theme;
      }
      localStorage.setItem('theme', theme);
    });
  }

  getTheme() {
    return this.currentTheme.asReadonly();
  }

  setTheme(theme: Theme) {
    this.currentTheme.set(theme);
  }

  toggleTheme() {
    const order: Theme[] = ['auto', 'light', 'dark'];
    const current = this.currentTheme();
    const index = order.indexOf(current);
    const next = order[(index + 1) % order.length];
    this.setTheme(next);
  }

  getEffectiveTheme(): 'light' | 'dark' {
    const theme = this.currentTheme();
    if (theme === 'auto') {
      return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
    }
    return theme;
  }

  isDarkMode() {
    return this.getEffectiveTheme() === 'dark';
  }

  private getStoredTheme(): Theme {
    const stored = localStorage.getItem('theme');
    if (stored === 'light' || stored === 'dark' || stored === 'auto') {
      return stored;
    }
    return 'auto';
  }
}
