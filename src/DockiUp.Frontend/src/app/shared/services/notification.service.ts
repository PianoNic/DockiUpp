import { Injectable, inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

export type NotificationType = 'error' | 'warning' | 'success' | 'info';

const DEFAULT_DURATION_MS = 5000;

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  private readonly snackBar = inject(MatSnackBar);

  /**
   * Show an error toast. Use for failed API calls, validation, etc.
   */
  error(message: string, detail?: string): void {
    const display = detail ? `${message}: ${detail}` : message;
    this.snackBar.open(display, 'Close', {
      duration: DEFAULT_DURATION_MS,
      panelClass: ['notification-error'],
      horizontalPosition: 'end',
      verticalPosition: 'bottom',
    });
  }

  /**
   * Show a success toast.
   */
  success(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: DEFAULT_DURATION_MS,
      panelClass: ['notification-success'],
      horizontalPosition: 'end',
      verticalPosition: 'bottom',
    });
  }

  /**
   * Show a warning toast.
   */
  warning(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: DEFAULT_DURATION_MS,
      panelClass: ['notification-warning'],
      horizontalPosition: 'end',
      verticalPosition: 'bottom',
    });
  }

  /**
   * Show an info toast.
   */
  info(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: DEFAULT_DURATION_MS,
      panelClass: ['notification-info'],
      horizontalPosition: 'end',
      verticalPosition: 'bottom',
    });
  }

  /**
   * Show a toast from a caught error (extracts message from Error or string).
   */
  showError(contextMessage: string, err: unknown): void {
    const detail = err instanceof Error ? err.message : String(err);
    this.error(contextMessage, detail);
  }
}
