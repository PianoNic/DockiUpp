import { Component, computed, inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProjectStore } from '../shared/stores/project.store';
import { MatCardModule } from "@angular/material/card";
import { MatInputModule } from "@angular/material/input";
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { ContainerDto, ProjectDto, UpdateMethodType } from '../api';

@Component({
  selector: 'app-detail',
  imports: [
    MatCardModule,
    MatInputModule,
    FormsModule,
    MatButtonModule,
    MatMenuModule,
    MatIconModule
],
  templateUrl: './detail.html',
  styleUrl: './detail.scss'
})
export class Detail implements OnInit {
  private route = inject(ActivatedRoute);
  public projectStore = inject(ProjectStore);
  readonly dockerId = this.route.snapshot.paramMap.get('id');

  UpdateMethodType = UpdateMethodType;

  async ngOnInit() {
    await this.projectStore.loadContainers();
  }

  public project = computed(() => {
    return this.projectStore.projectDtos().find(a => a.dockerProjectName === this.dockerId);
  });

  consoleText = 'Compose healthy\n$';
  consoleInput = '';

  onStart() {
    this.consoleText += `\n$ start command executed`;
  }

  onRestart() {
    this.consoleText += `\n$ restart command executed`;
  }

  onStop() {
    this.consoleText += `\n$ stop command executed`;
  }

  getRunningCount(): number {
    return ((this.project()?.containers) || []).filter(c => c.state === UpdateMethodType.Running).length;
  }
}
