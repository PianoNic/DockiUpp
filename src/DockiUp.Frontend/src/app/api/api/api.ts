export * from './app.service';
import { AppService } from './app.service';
export * from './container.service';
import { ContainerService } from './container.service';
export * from './project.service';
import { ProjectService } from './project.service';
export * from './webhook.service';
import { WebhookService } from './webhook.service';
export const APIS = [AppService, ContainerService, ProjectService, WebhookService];
