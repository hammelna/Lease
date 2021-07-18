import { Injectable } from "@angular/core";
import { HttpClient, HttpResponse } from '@angular/common/http';
import { Observable } from "rxjs";
import { environment } from "src/environments/environment";
import { map, tap } from 'rxjs/operators';

export interface FileBlob {
    fileName: string;
    file: Blob;
}

@Injectable()
export class DownloadService {

    constructor(private http: HttpClient) { }

    public downloadLeases(startDate: Date, endDate: Date): Observable<FileBlob> {
        
        const url = environment.Urls.DownloadUrl;
        const utcStartDate = startDate.toUTCString();
        const utcEndDate = endDate.toUTCString();
        console.log(`In Service, UTC Dates, Start: ${utcStartDate}, End: ${utcEndDate}`);
        const response = this.http.get(url, { 
            params: { 
                startDate: utcStartDate, //.toLocaleDateString('en-US'), 
                endDate: utcEndDate//.toLocaleDateString('en-US')
            },
            responseType: 'blob',
            observe: 'response'
        }).pipe(
            tap((resp: HttpResponse<Blob>) => {
                console.log(`Header for Content Disposition: ${resp.headers.get('content-disposition')}. `, resp.headers);
            }),
            map((resp: HttpResponse<Blob>) => {
                const contentDisposition = resp.headers.get('content-disposition');
                const fileName = contentDisposition.split(';')[1].trim().replace('filename=', '');
                return {
                    file: resp.body,
                    fileName
                }
            })
        );
        return response;
    }
}