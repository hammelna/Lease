import { Component, OnInit } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { first } from "rxjs/operators";
import { DownloadService } from "./download.service";
import { FileInformation } from "./file-info.model";

@Component({
    selector: 'lsc-download-leases',
    templateUrl: './download.component.html',
    styleUrls: ['./download.component.scss']
})
export class DownloadComponent implements OnInit {

    paymentsDateRange: FormGroup;
    
    constructor(private downloadService: DownloadService) { }

    ngOnInit() {
        this.paymentsDateRange = new FormGroup({
            start: new FormControl(new Date(2021, 0, 1)),
            end: new FormControl(new Date(2022, 0, 1))
        });
    }


    downloadLeases(): void {
        this.downloadService.downloadLeases(this.startDate, this.endDate)
            .pipe(first())
            .subscribe((fileInfo: FileInformation) => this.invokeDownload(fileInfo));
    }

    private get startDate(): Date {
        return this.paymentsDateRange.get('start')?.value ?? new Date();
    }

    private get endDate(): Date {
        return this.paymentsDateRange.get('end')?.value ?? new Date();
    }

    private invokeDownload(fileInfo: FileInformation): void {
        const a = document.createElement('a')
        const objectUrl = URL.createObjectURL(fileInfo.file)
        a.href = objectUrl
        a.download = fileInfo.fileName;
        a.click();
        URL.revokeObjectURL(objectUrl);
    }
}