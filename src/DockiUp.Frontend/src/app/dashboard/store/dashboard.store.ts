import {
  signalStore,
  withHooks,
  withMethods,
  withState,
  withComputed,
  patchState
} from '@ngrx/signals';
import { ProjectDto, ProjectService } from '../../api';
import { inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';

type DashboardState = {
  projectDtos: ProjectDto[],
  loading: boolean,
};

export const initialDashboardStore: DashboardState = {
  projectDtos: [],
  loading: false
};

export const DashboardStore = signalStore(
  { providedIn: 'root' },
  withState(initialDashboardStore),
  withComputed((store) => ({})),
  withMethods((store) => {
    const projectService = inject(ProjectService);

    const loadContainers = async () => {
      patchState(store, { loading: true });
      try {
        const projects = await firstValueFrom(projectService.getProjects()) ?? initialDashboardStore.projectDtos;
        patchState(store, { projectDtos: projects });
      } catch {
        console.error("An Error occurred");
      } finally {
        patchState(store, { loading: false });
      }
    };

    return {
      loadContainers
    };
  }),
  withHooks((store) => ({}))
);
