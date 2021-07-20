import { HttpClient, HttpResponse } from '@angular/common/http';
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { map } from 'rxjs/operators';
import { environment } from "src/environments/environment";
import { FileInformation } from "./file-info.model";

@Injectable()
export class DownloadService {

    constructor(private http: HttpClient) { }

    public downloadLeases(startDate: Date, endDate: Date): Observable<FileInformation> {
        
        const url = environment.Urls.DownloadUrl;
        const response = this.http.get(url, { 
            params: { 
                startDate: startDate.toLocaleDateString('en-US', {year: 'numeric', month: '2-digit', day: '2-digit'}), 
                endDate: endDate.toLocaleDateString('en-US', {year: 'numeric', month: '2-digit', day: '2-digit'})
            },
            responseType: 'blob',
            observe: 'response'
        }).pipe(
            map((resp: HttpResponse<Blob>) => {
                const contentDisposition = resp.headers.get('content-disposition');
                const fileName = contentDisposition.split(';')[1].trim().replace('filename=', '');
                return { file: resp.body, fileName }
            })
        );
        return response;
    }
}