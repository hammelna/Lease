import { HttpClient } from "@angular/common/http";
import { Component } from "@angular/core";
import { Subscription } from "rxjs";
import { first } from "rxjs/operators";
import { UploadService } from "./upload.service";

@Component({
    selector: 'lsc-upload',
    templateUrl: './upload.component.html',
    styleUrls: ['./upload.component.scss']
})
export class UploadComponent {
    fileName: string = '';

    constructor(private uploadService: UploadService, private http: HttpClient) { }
    
    onFileSelected($event) {
        const file: File = $event.target.files[0];

        if (file) {
            this.fileName = file.name;
            this.uploadService.uploadLeases(file).pipe(
                first(),
            ).subscribe();
        }
    }
}