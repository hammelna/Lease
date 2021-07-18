import { Component } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { first, tap } from "rxjs/operators";
import { DownloadService, FileBlob } from "./download.service";

@Component({
    selector: 'lsc-download-leases',
    templateUrl: './download.component.html',
    styleUrls: ['./download.component.scss']
})
export class DownloadComponent {

    datesForm: FormGroup = new FormGroup ({
        startDate: new FormControl(), 
        endDate: new FormControl()
    });
    
    constructor(private downloadService: DownloadService) { }

    downloadLeases(): void {
        console.log('Hit download leases');
        const startDate = new Date(this.datesForm.get('startDate').value);
        const endDate = new Date(this.datesForm.get('endDate').value);
        console.log(`StartDate: ${startDate}, EndDate: ${endDate}`);
        console.log(`Form StartDate: ${this.datesForm.get('startDate').value}, Form EndDate: ${this.datesForm.get('endDate').value}`);
        this.downloadService.downloadLeases(startDate, endDate).pipe(
            first(),
            tap((resp) => console.log('Response in component: ', resp))
        ).subscribe((fileInfo: FileBlob) => {
              const a = document.createElement('a')
              const objectUrl = URL.createObjectURL(fileInfo.file)
              a.href = objectUrl
              a.download = fileInfo.fileName;
              a.click();
              URL.revokeObjectURL(objectUrl);
        });
    }
}