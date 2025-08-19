import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { CreateProjectModal } from '../create-project-modal/create-project-modal';

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

  constructor(
    private dialog: MatDialog
  ) { }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(CreateProjectModal, {minWidth: '750px'});

    dialogRef.afterClosed().subscribe(a => console.log(a));
  }
}
