import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  public data: string[] = [];
  public people?: Person[];

  constructor(
    private httpCLient: HttpClient,
    private toastr: ToastrService
  ) { }

  onFileSelected() {
    this.httpCLient.get<Person[]>('/data').subscribe(result => {
      this.people = result;
    }, error => this.toastr.error(error.error));
  }

  saveFile() {
    this.httpCLient.post<Person[]>('/data', this.people).subscribe(result => {
      this.toastr.success('Data successfully saved to the database.')
    }, error => this.toastr.error(error.error));
  }

}

interface Person {
  lastName: string;
  firstName: string;
  postalCode?: string;
  city: string;
  phoneNumber: string;
  isPostalCodeValid: boolean;
}
