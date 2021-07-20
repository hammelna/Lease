import { HttpClient } from '@angular/common/http';
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "src/environments/environment";
import { UploadStatus } from "./upload-status.model";

@Injectable()
export class UploadService {

    constructor(private http: HttpClient) { }

    public uploadLeases(file: File): Observable<UploadStatus> {
        
        const url = environment.Urls.UploadUrl;
        const formData: FormData = new FormData();

        formData.append('upload', file);

        return this.http.post<UploadStatus>(url, formData);
    }
}
