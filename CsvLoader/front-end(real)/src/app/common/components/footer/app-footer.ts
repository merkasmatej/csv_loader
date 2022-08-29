import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-footer',
  templateUrl: './app-footer.html',
  styleUrls: ['./app-footer.css']
})

export class AppFooterComponent {
  @Input() title!: string;
}
