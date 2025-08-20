import { Component, OnInit, inject, computed } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { CreateProjectButton } from '../../shared/components/create-project-button/button/create-project-button';
import { ProjectDto, UpdateMethodType } from '../../api';
import { MatProgressBar } from "@angular/material/progress-bar";
import { ProjectStore } from '../../shared/stores/project.store';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-dashboard',
  imports: [
    CommonModule,
    MatCardModule,
    MatIconModule,
    RouterLink,
    MatButtonModule,
    CreateProjectButton,
    MatProgressBar,
    MatProgressSpinnerModule
],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class Dashboard implements OnInit {
  projectStore = inject(ProjectStore);
  projects = this.projectStore.projectDtos;

  getRunningCount(project: ProjectDto): number {
    return (project.containers || []).filter(c => c.state === UpdateMethodType.Running).length;
  }

  getTotalCount(project: ProjectDto): number {
    return (project.containers || []).length;
  }

  async ngOnInit() {
    await this.projectStore.loadContainers();
  }

  getStatusIcon(status?: UpdateMethodType): string {
    if (!status) return 'help';

    switch (status) {
      case UpdateMethodType.Running:
        return 'check_circle';
      case UpdateMethodType.Stopped:
        return 'stop';
      case UpdateMethodType.Updating:
        return 'loop';
      case UpdateMethodType.Crashed:
        return 'error';
      default:
        return 'help';
    }
  }

  readonly containerStats = computed(() => {
    const allContainers = this.projects().flatMap(p => p.containers || []);

    return {
      stopped: allContainers.filter(c => c.state === UpdateMethodType.Stopped).length,
      running: allContainers.filter(c => c.state === UpdateMethodType.Running).length,
      needsUpdate: allContainers.filter(c => c.state === UpdateMethodType.Updating).length,
      updating: allContainers.filter(c => c.state === UpdateMethodType.Updating).length,
      crashed: allContainers.filter(c => c.state === UpdateMethodType.Crashed).length,
    };
  });
}
