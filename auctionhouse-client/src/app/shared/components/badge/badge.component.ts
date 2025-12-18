import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-badge',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="badge">
      <img [src]="badge.icon" [alt]="badge.name" />
      <p>{{ badge.name }}</p>
    </div>
  `,
  styles: [`
    .badge {
      text-align: center;
      font-size: 0.9rem;
    }
    .badge img {
      width: 60px;
      height: 60px;
    }
    .badge p {
      margin-top: 5px;
      font-weight: bold;
    }
  `]
})
export class BadgeComponent {
  @Input() badge: { name: string; icon: string } = {
    name: '',
    icon: ''
  };
}
