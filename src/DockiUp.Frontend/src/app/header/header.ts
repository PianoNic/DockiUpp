import { Component } from '@angular/core';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { Router } from '@angular/router';
import { CreateProjectButton } from '../shared/components/create-project-button/button/create-project-button';

@Component({
  selector: 'app-header',
  imports: [
    MatToolbarModule,
    MatIconModule,
    MatButtonModule,
    CreateProjectButton
  ],
  templateUrl: './header.html',
  styleUrl: './header.scss'
})
export class Header {
  constructor(private router: Router) { }

  ngOnInit(): void {
  }

  navigateTo(route: string): void {
    this.router.navigate([route]);
  }
}
