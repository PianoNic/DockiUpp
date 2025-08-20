import { CommonModule } from '@angular/common';
import { Component, DestroyRef, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { CreateProjectModal } from '../create-project-modal/create-project-modal';
import { ProjectService, SetupProjectDto } from '../../../../api';
import { firstValueFrom } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ProjectStore } from '../../../stores/project.store';
@Component({
  selector: 'app-create-project-button',
  imports: [
    CommonModule,
    MatButtonModule,
    MatMenuModule,
    MatIconModule
  ],
  templateUrl: './create-project-button.html',
  styleUrl: './create-project-button.scss'
})
export class CreateProjectButton {
  projectService = inject(ProjectService)
  projectStore = inject(ProjectStore);
  destroyRef = inject(DestroyRef);

  constructor(
    private dialog: MatDialog
  ) { }

  async openCreateDialog() {
    const dialogRef = this.dialog.open(CreateProjectModal, { minWidth: '750px' });

    dialogRef.afterClosed().pipe(takeUntilDestroyed(this.destroyRef)).subscribe(async (result: SetupProjectDto | undefined) => {
      console.log(result);
      await this.projectStore.deployProject(result!);
    });
  }
}
