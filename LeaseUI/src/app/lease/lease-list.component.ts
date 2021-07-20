import { Component, OnInit } from "@angular/core";
import { Observable, Subject } from "rxjs";
import { LeaseApiService } from "./lease-api.service";
import { LeaseModel } from "./lease.model";

@Component({
    selector: 'lsc-lease-list',
    templateUrl: './lease-list.component.html',
    styleUrls: ['./lease-list.component.scss']
})
export class LeaseListComponent implements OnInit{
    leases$: Observable<LeaseModel[]>;
    unsubscribe$: Subject<void> = new Subject();
    
    displayedColumns: string[] = ['name', 'numberOfPayments', 'paymentAmount', 'interestRate', 'startDate', 'endDate'];

    constructor(private leaseApiService: LeaseApiService) { }

    ngOnInit(): void {
        this.leases$ = this.leaseApiService.getLeases();
    }
}