import { HttpClient } from '@angular/common/http';
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "src/environments/environment";
import { LeaseModel } from "./lease.model";

@Injectable()
export class LeaseApiService {

    constructor(private http: HttpClient) { }

    public getLeases(): Observable<LeaseModel[]> {
        const url = environment.Urls.LeaseUrl;
        return this.http.get<LeaseModel[]>(url);
    }
}