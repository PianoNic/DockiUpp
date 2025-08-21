import { Routes } from '@angular/router';
import { Dashboard } from './dashboard/dashboard';
import { Containers } from './containers/containers';
import { Detail } from './detail/detail';
import { User } from './user/user';

export const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: 'dashboard', component: Dashboard },
  { path: 'containers', component: Containers },
  { path: 'project/:id', component: Detail },
  { path: 'me', component: User },
  { path: '**', redirectTo: '/dashboard' }
];
