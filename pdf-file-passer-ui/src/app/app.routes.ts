import { Routes } from '@angular/router';
import { MainLayoutComponent } from './shared/layouts/main-layout/main-layout.component';
import { HomeComponent } from './features/home/home.component';
import { UploadComponent } from './features/upload/upload.component';
import { ListComponent } from './features/list/list.component';
import { ViewerComponent } from './features/viewer/viewer.component';


export const routes: Routes = [
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      { path: '', component: HomeComponent },
      { path: 'upload', component: UploadComponent },
      { path: 'list', component: ListComponent },
      { path: 'viewer/:id', component: ViewerComponent },
      { path: '**', redirectTo: '' }
    ]
  }
];
