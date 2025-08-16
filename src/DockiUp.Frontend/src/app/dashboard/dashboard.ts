import { Component, signal, OnInit } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { CreateProjectButton } from '../shared/components/create-project-button/create-project-button';

interface Container {
  dbContainerId: string;
  name: string;
  description: string;
  status: string;
  lastUpdated?: Date;
}

@Component({
  selector: 'app-dashboard',
  imports: [
    CommonModule,
    MatCardModule,
    MatIconModule,
    RouterLink,
    MatButtonModule,
    CreateProjectButton
  ],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class Dashboard implements OnInit {
  // Add the missing properties as signals
  loading = signal(true);
  containers = signal<Container[]>([]);
  error = signal<string | null>(null);

  ngOnInit() {
    this.loadContainers();
  }

  loadContainers() {
    this.loading.set(true);
    this.error.set(null);

    // Simulate API call - replace with your actual service call
    setTimeout(() => {
      // Mock data - replace with actual API call
      const mockContainers: Container[] = [
        {
          dbContainerId: '1',
          name: 'Web Server',
          description: 'Nginx web server container',
          status: 'running',
          lastUpdated: new Date()
        },
        {
          dbContainerId: '2',
          name: 'Database',
          description: 'PostgreSQL database container',
          status: 'stopped',
          lastUpdated: new Date()
        }
      ];

      this.containers.set(mockContainers);
      this.loading.set(false);
    }, 1000);
  }

  getStatusIcon(status: string): string {
    switch (status) {
      case 'running':
        return 'check_circle';
      case 'stopped':
        return 'stop';
      case 'updating':
        return 'loop';
      case 'failed':
        return 'error';
      case 'needs-update':
        return 'new_releases';
      default:
        return 'help';
    }
  }

  // Getter methods for template filtering
  get stoppedCount(): number {
    return this.containers().filter(c => c.status === 'stopped').length;
  }

  get runningCount(): number {
    return this.containers().filter(c => c.status === 'running').length;
  }

  get needsUpdateCount(): number {
    return this.containers().filter(c => c.status === 'needs-update').length;
  }

  get updatingCount(): number {
    return this.containers().filter(c => c.status === 'updating').length;
  }

  get failedCount(): number {
    return this.containers().filter(c => c.status === 'failed').length;
  }
}
