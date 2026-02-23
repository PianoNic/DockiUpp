import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { Theme, ThemeService } from '../../services/theme.service';

@Component({
  selector: 'app-theme-toggle',
  imports: [CommonModule, MatIconModule],
  templateUrl: './theme-toggle.component.html',
  styleUrls: ['./theme-toggle.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ThemeToggleComponent {
  private themeService = inject(ThemeService);

  readonly theme = this.themeService.getTheme();

  toggleTheme(): void {
    this.themeService.toggleTheme();
  }

  icon(): string {
    const current = this.theme();
    if (current === 'auto') {
      return 'computer';
    }
    return current === 'dark' ? 'light_mode' : 'dark_mode';
  }

  label(): string {
    const current = this.theme();
    if (current === 'auto') {
      return 'Auto';
    }
    return current === 'dark' ? 'Dark' : 'Light';
  }

  ariaLabel(): string {
    return `Switch to ${this.nextTheme()} mode`;
  }

  private nextTheme(): string {
    const order: Theme[] = ['auto', 'light', 'dark'];
    const current = this.theme();
    const idx = order.indexOf(current);
    return order[(idx + 1) % order.length];
  }
}
