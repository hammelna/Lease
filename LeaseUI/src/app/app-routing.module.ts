import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DownloadComponent } from './download/download.component';
import { LeaseListComponent } from './lease/lease-list.component';
import { UploadComponent } from './upload/upload.component';

const routes: Routes = [
  { path: 'lease', component: LeaseListComponent },
  { path: 'upload', component: UploadComponent },
  { path: 'download', component: DownloadComponent },
  { path: '', redirectTo: 'lease', pathMatch: 'full' },
  { path: '**', redirectTo: 'lease', pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { relativeLinkResolution: 'legacy' })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
