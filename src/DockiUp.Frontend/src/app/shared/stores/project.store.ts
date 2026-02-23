import { signalStore, withHooks, withMethods, withState, withComputed, patchState } from '@ngrx/signals';
import { ProjectDto, ProjectService, SetupProjectDto } from '../../api';
import { inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { NotificationService } from '../services/notification.service';

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
    const notifications = inject(NotificationService);

    const loadContainers = async () => {
      patchState(store, { loading: true });
      try {
        const projects = await firstValueFrom(projectService.getProjects()) ?? initialProjectStore.projectDtos;
        patchState(store, { projectDtos: projects });
      } catch (err) {
        notifications.showError('Failed to load projects', err);
      } finally {
        patchState(store, { loading: false });
      }
    };

    /** Update project list from SignalR payload (no HTTP call). */
    const setProjectDtos = (projects: ProjectDto[]) => {
      patchState(store, { projectDtos: projects ?? [] });
    };

    const deployProject = async (setupProjectDto: SetupProjectDto) => {
      patchState(store, { loading: true });
      try {
        await firstValueFrom(projectService.deployProject(setupProjectDto));
        await loadContainers();
      } catch (err) {
        notifications.showError('Failed to deploy project', err);
      } finally {
        patchState(store, { loading: false });
      }
    };

    const stopProject = async (dockerProjectName: string) => {
      try {
        await firstValueFrom(projectService.stopProject(undefined, dockerProjectName));
        await loadContainers();
      } catch (err) {
        notifications.showError('Failed to stop project', err);
      }
    };

    const restartProject = async (dockerProjectName: string) => {
      try {
        await firstValueFrom(projectService.restartProject(undefined, dockerProjectName));
        await loadContainers();
      } catch (err) {
        notifications.showError('Failed to restart project', err);
      }
    };

    const updateProject = async (projectId: number) => {
      patchState(store, { loading: true });
      try {
        await firstValueFrom(projectService.updateProject(projectId));
        await loadContainers();
      } catch (err) {
        notifications.showError('Failed to update project', err);
      } finally {
        patchState(store, { loading: false });
      }
    };

    return {
      loadContainers,
      setProjectDtos,
      deployProject,
      stopProject,
      restartProject,
      updateProject,
    };
  }),
  withHooks((store) => ({}))
);
