import { Routes } from '@angular/router';
import { PatternDashboardComponent } from './components/pattern-dashboard/pattern-dashboard.component';

export const routes: Routes = [
  { path: '', redirectTo: '/patterns', pathMatch: 'full' },
  { path: 'patterns', component: PatternDashboardComponent },
  { path: '**', redirectTo: '/patterns' }
];