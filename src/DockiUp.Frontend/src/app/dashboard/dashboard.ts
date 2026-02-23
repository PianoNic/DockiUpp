import { Component, OnInit, inject, computed } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressBar } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ProjectDto } from '../api';
import { UpdateMethodType, normalizeContainerState } from '../shared/models/api-enums';
import { CreateProjectButton } from '../shared/components/create-project-button/button/create-project-button';
import { ProjectStore } from '../shared/stores/project.store';

@Component({
  selector: 'app-dashboard',
  imports: [
    CommonModule,
    MatCardModule,
    MatIconModule,
    RouterLink,
    MatButtonModule,
    MatMenuModule,
    CreateProjectButton,
    MatProgressBar,
    MatProgressSpinnerModule,
  ],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class Dashboard implements OnInit {
  projectStore = inject(ProjectStore);
  projects = this.projectStore.projectDtos;

  async onStopProject(project: ProjectDto) {
    await this.projectStore.stopProject(project.dockerProjectName);
  }

  async onRestartProject(project: ProjectDto) {
    await this.projectStore.restartProject(project.dockerProjectName);
  }

  async onUpdateProject(project: ProjectDto) {
    const id = (project as ProjectDto & { id?: number })?.id;
    if (id != null) await this.projectStore.updateProject(id);
  }

  canUpdateProject(project: ProjectDto): boolean {
    return project.managedByDockiUp && (project as ProjectDto & { id?: number }).id != null;
  }

  getRunningCount(project: ProjectDto): number {
    return (project.containers || []).filter(c => normalizeContainerState(c.state) === UpdateMethodType.Running).length;
  }

  getTotalCount(project: ProjectDto): number {
    return (project.containers || []).length;
  }

  async ngOnInit() {
    await this.projectStore.loadContainers();
  }

  /** Accepts state as number or string (API may send enum as string). */
  getStatusIcon(status?: number | string): string {
    const n = normalizeContainerState(status);
    if (n === UpdateMethodType.Unknown) return 'help';

    switch (n) {
      case UpdateMethodType.Running:
        return 'check_circle';
      case UpdateMethodType.Stopped:
        return 'stop';
      case UpdateMethodType.Updating:
        return 'loop';
      case UpdateMethodType.Crashed:
        return 'error';
      case UpdateMethodType.Created:
        return 'pending';
      default:
        return 'help';
    }
  }

  /** Project-level status: prefer first container, else derive from all containers. */
  getProjectStatusIcon(project: ProjectDto): string {
    const containers = project?.containers ?? [];
    if (containers.length === 0) return 'help';
    const states = containers.map(c => normalizeContainerState(c.state));
    if (states.some(s => s === UpdateMethodType.Crashed)) return 'error';
    if (states.some(s => s === UpdateMethodType.Updating)) return 'loop';
    if (states.every(s => s === UpdateMethodType.Stopped)) return 'stop';
    if (states.some(s => s === UpdateMethodType.Running)) return 'check_circle';
    if (states.some(s => s === UpdateMethodType.Created)) return 'pending';
    return 'help';
  }

  readonly containerStats = computed(() => {
    const allContainers = this.projects().flatMap(p => p.containers || []);

    return {
      stopped: allContainers.filter(c => normalizeContainerState(c.state) === UpdateMethodType.Stopped).length,
      running: allContainers.filter(c => normalizeContainerState(c.state) === UpdateMethodType.Running).length,
      needsUpdate: allContainers.filter(c => normalizeContainerState(c.state) === UpdateMethodType.Updating).length,
      updating: allContainers.filter(c => normalizeContainerState(c.state) === UpdateMethodType.Updating).length,
      crashed: allContainers.filter(c => normalizeContainerState(c.state) === UpdateMethodType.Crashed).length,
    };
  });
}
