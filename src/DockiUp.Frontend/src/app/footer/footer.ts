import { Component, signal } from '@angular/core';
import { faGithub } from '@fortawesome/free-brands-svg-icons';
import { AppService } from '../api';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';

@Component({
  selector: 'app-footer',
  imports: [
    FontAwesomeModule
  ],
  templateUrl: './footer.html',
  styleUrl: './footer.scss'
})
export class Footer {
  currentYear: number = new Date().getFullYear();
  faGithub = faGithub;

  version = signal('unknown');
  environment = signal('unknown');

  constructor(private appService: AppService) {
    this.appService.getAppInfo()
      .pipe(takeUntilDestroyed())
      .subscribe(appInfo => {
        this.version.set(appInfo.version!);
        this.environment.set(appInfo.environment!);
      });
  }
}
