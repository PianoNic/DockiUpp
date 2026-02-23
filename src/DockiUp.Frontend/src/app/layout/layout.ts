import { Component, ChangeDetectionStrategy, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { DockiUpHubService } from '../shared/services/dockiup-hub.service';
import { Layout as LayoutService } from '../shared/services/layout';
import { Header } from './header/header';
import { Sidenav } from './sidenav/sidenav';

@Component({
  selector: 'app-layout',
  imports: [RouterOutlet, Header, Sidenav],
  templateUrl: './layout.html',
  styleUrl: './layout.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LayoutComponent {
  readonly layoutService = inject(LayoutService);

  constructor() {
    inject(DockiUpHubService); // start SignalR connection for live updates
  }
}
