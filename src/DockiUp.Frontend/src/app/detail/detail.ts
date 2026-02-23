import { Component, computed, inject, OnInit, signal, ViewChild, ElementRef, afterNextRender, effect } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { ProjectStore } from '../shared/stores/project.store';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { firstValueFrom } from 'rxjs';
import { ContainerDto, ContainerService, Configuration, ProjectDto } from '../api';
import { UpdateMethodType, normalizeContainerState } from '../shared/models/api-enums';
import { NotificationService } from '../shared/services/notification.service';

@Component({
  selector: 'app-detail',
  imports: [
    MatCardModule,
    MatInputModule,
    MatSelectModule,
    MatFormFieldModule,
    FormsModule,
    MatButtonModule,
    MatMenuModule,
    MatIconModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './detail.html',
  styleUrl: './detail.scss',
})
export class Detail implements OnInit {
  private route = inject(ActivatedRoute);
  private http = inject(HttpClient);
  private configuration = inject(Configuration);
  projectStore = inject(ProjectStore);
  private containerService = inject(ContainerService);
  private notifications = inject(NotificationService);
  readonly dockerId = this.route.snapshot.paramMap.get('id');

  /** Empty string = all containers, otherwise single container id. */
  readonly selectedContainerId = signal<string>('');
  readonly consoleLogs = signal<string>('');
  readonly logsLoading = signal<boolean>(false);

  @ViewChild('logContainer') logContainerRef?: ElementRef<HTMLDivElement>;

  UpdateMethodType = UpdateMethodType;

  constructor() {
    afterNextRender(() => this.scrollLogsToBottom());
    effect(() => {
      this.consoleLogs();
      setTimeout(() => this.scrollLogsToBottom(), 0);
    });
  }

  async ngOnInit() {
    await this.projectStore.loadContainers();
    await this.loadConsoleLogs();
  }

  project = computed(() =>
    this.projectStore.projectDtos().find((a) => a.dockerProjectName === this.dockerId)
  );

  async onStopProject() {
    const p = this.project();
    if (p?.dockerProjectName) await this.projectStore.stopProject(p.dockerProjectName);
  }

  async onRestartProject() {
    const p = this.project();
    if (p?.dockerProjectName) await this.projectStore.restartProject(p.dockerProjectName);
  }

  async onUpdateProject() {
    const p = this.project();
    const id = (p as ProjectDto & { id?: number })?.id;
    if (id != null) await this.projectStore.updateProject(id);
  }

  async onStartContainer(container: ContainerDto) {
    try {
      await firstValueFrom(this.containerService.startContainer(container.id));
      await this.projectStore.loadContainers();
    } catch (err) {
      this.notifications.showError('Failed to start container', err);
    }
  }

  async onStopContainer(container: ContainerDto) {
    try {
      await firstValueFrom(this.containerService.stopContainer(container.id));
      await this.projectStore.loadContainers();
    } catch (err) {
      this.notifications.showError('Failed to stop container', err);
    }
  }

  async onRestartContainer(container: ContainerDto) {
    try {
      await firstValueFrom(this.containerService.restartContainer(container.id));
      await this.projectStore.loadContainers();
    } catch (err) {
      this.notifications.showError('Failed to restart container', err);
    }
  }

  getRunningCount(): number {
    return (
      (this.project()?.containers ?? []).filter(
        (c) => normalizeContainerState(c.state) === UpdateMethodType.Running
      ).length
    );
  }

  /** Normalized state (API may send enum as string). */
  getContainerState(container: ContainerDto): number {
    return normalizeContainerState(container.state);
  }

  getStatusIcon(state: number | string | undefined): string {
    const n = normalizeContainerState(state);
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

  canUpdateProject(): boolean {
    const p = this.project();
    return !!(p?.managedByDockiUp && (p as ProjectDto & { id?: number }).id != null);
  }

  async loadConsoleLogs(): Promise<void> {
    const containerId = this.selectedContainerId();
    const p = this.project();
    const containers = p?.containers ?? [];

    this.logsLoading.set(true);
    try {
      const basePath = this.configuration.basePath ?? '';
      const url = `${basePath}/api/Container/GetContainerLogs`;
      const tail = 200;

      if (!containerId && containers.length === 0) {
        this.consoleLogs.set('No containers in this project.');
        return;
      }

      if (!containerId && containers.length > 0) {
        // All containers: fetch in parallel and merge with container name prefix
        const results = await Promise.all(
          containers.map(async (c) => {
            try {
              const logs = await firstValueFrom(
                this.http.get(url, { params: { containerId: c.id, tail }, responseType: 'text' })
              );
              return { name: c.name, logs: logs ?? '' };
            } catch (err) {
              this.notifications.showError(`Failed to load logs for ${c.name}`, err);
              return { name: c.name, logs: `(failed: ${err instanceof Error ? err.message : String(err)})` };
            }
          })
        );
        const merged = results
          .map((r) => {
            const lines = (r.logs || '').split(/\r?\n/).filter((line) => line.length > 0);
            return lines.map((line) => `[${r.name}] ${line}`).join('\n');
          })
          .filter((block) => block.length > 0)
          .join('\n\n');
        this.consoleLogs.set(merged || '(no output from any container)');
      } else {
        const logs = await firstValueFrom(
          this.http.get(url, { params: { containerId, tail }, responseType: 'text' })
        );
        this.consoleLogs.set(logs || '(no output)');
      }
    } catch (err) {
      const msg = err instanceof Error ? err.message : String(err);
      this.consoleLogs.set('Failed to load logs. ' + msg);
      this.notifications.showError('Failed to load logs', err);
    } finally {
      this.logsLoading.set(false);
    }
  }

  scrollLogsToBottom(): void {
    const el = this.logContainerRef?.nativeElement;
    if (el) el.scrollTop = el.scrollHeight;
  }
}
