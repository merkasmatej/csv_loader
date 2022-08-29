import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-footer',
  templateUrl: './app-footer.html',
})

export class AppFooterComponent {
  @Input() title!: string;
}
