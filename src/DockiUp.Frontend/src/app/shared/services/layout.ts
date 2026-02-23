import { Injectable, signal, computed, inject } from '@angular/core';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';

@Injectable({
  providedIn: 'root',
})
export class Layout {
  private readonly breakpointObserver = inject(BreakpointObserver);

  private readonly sidenavCollapsedSignal = signal(false);
  private readonly sidenavOpenedSignal = signal(true);
  private readonly isMobileSignal = signal(false);

  readonly sidenavCollapsed = this.sidenavCollapsedSignal.asReadonly();
  readonly sidenavOpened = this.sidenavOpenedSignal.asReadonly();
  readonly isMobile = this.isMobileSignal.asReadonly();

  readonly sidenavMode = computed(() => (this.isMobileSignal() ? 'over' : 'side'));

  constructor() {
    this.breakpointObserver
      .observe([Breakpoints.Medium, Breakpoints.Large, Breakpoints.XLarge])
      .subscribe((result) => {
        const isDesktop = result.matches;
        this.isMobileSignal.set(!isDesktop);
        this.sidenavOpenedSignal.set(isDesktop);
      });
  }

  toggleSidenav(): void {
    this.sidenavOpenedSignal.update((value) => !value);
  }

  toggleSidenavCollapsed(): void {
    this.sidenavCollapsedSignal.update((value) => !value);
  }

  closeSidenav(): void {
    this.sidenavOpenedSignal.set(false);
  }
}
