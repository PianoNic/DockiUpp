import { signalStore, withHooks, withMethods, withState, withComputed, patchState } from '@ngrx/signals';
import { ProjectDto, ProjectService, SetupProjectDto } from '../../api';
import { inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';

type ProjectState = {
  projectDtos: ProjectDto[],
  loading: boolean,
};

export const initialProjectStore: ProjectState = {
  projectDtos: [],
  loading: false
};

export const ProjectStore = signalStore(
  { providedIn: 'root' },
  withState(initialProjectStore),
  withComputed((store) => ({})),
  withMethods((store) => {
    const projectService = inject(ProjectService);

    const loadContainers = async () => {
      patchState(store, { loading: true });
      try {
        const projects = await firstValueFrom(projectService.getProjects()) ?? initialProjectStore.projectDtos;
        patchState(store, { projectDtos: projects });
      } catch {
        console.error("An Error occurred");
      } finally {
        patchState(store, { loading: false });
      }
    };

    const deployProject = async (setupProjectDto: SetupProjectDto) => {
      patchState(store, { loading: true });
      try {
        await firstValueFrom(projectService.deployProject(setupProjectDto));
        await loadContainers();
      } catch {
        console.error("An Error occurred");
      } finally {
        patchState(store, { loading: false });
      }
    };

    return {
      loadContainers,
      deployProject
    };
  }),
  withHooks((store) => ({}))
);
