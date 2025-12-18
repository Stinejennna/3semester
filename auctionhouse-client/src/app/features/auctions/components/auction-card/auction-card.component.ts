import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CurrencyDkkPipe } from '../../../../shared/pipes/currency-dkk.pipe';

@Component({
  selector: 'app-auction-card',
  standalone: true,
  imports: [CommonModule, CurrencyDkkPipe],
  template: `
    <div class="auction-card">
      <div class="image-container">
        <img [src]="auction.imageUrl" [alt]="auction.title" />
      </div>
      <div class="auction-content">
        <h3>{{ auction.title }}</h3>
        <p>Startbud: {{ auction.startingBid | currencyDkk }}</p>
        <p *ngIf="auction.currentBid">Nuværende bud: {{ auction.currentBid | currencyDkk }}</p>
      </div>
    </div>
  `,
  styles: [`
    .auction-card {
      width: 220px;
      height: 350px;
      display: flex;
      flex-direction: column;
      justify-content: space-between;
      border: 1px solid #ddd;
      border-radius: 10px;
      overflow: hidden;
      background-color: #fff;
      transition: transform 0.3s ease, box-shadow 0.3s ease;
    }

    .auction-card:hover {
      transform: translateY(-8px);
      box-shadow: 0 12px 24px rgba(0,0,0,0.15);
    }

    .image-container {
      height: 180px;
      overflow: hidden;
    }

    .image-container img {
      width: 100%;
      height: 100%;
      object-fit: cover; /* sikrer samme billedhøjde uden at forvride */
      transition: transform 0.3s ease;
    }

    .auction-card:hover img {
      transform: scale(1.05);
    }

    .auction-content {
      padding: 10px 15px;
      text-align: center;
      flex: 1;
      display: flex;
      flex-direction: column;
      justify-content: center;
    }

    .auction-content h3 {
      font-family: 'Merriweather', serif;
      font-size: 1.1rem;
      margin: 8px 0;
      color: #1a1a1a;
    }

    .auction-content p {
      font-family: 'Open Sans', sans-serif;
      font-size: 0.95rem;
      color: #555;
      margin: 0;
    }
  `]
})
export class AuctionCardComponent {
  @Input() auction: {
    title: string;
    startingBid: number;
    currentBid?: number;
    imageUrl: string
  } = {
    title: '',
    startingBid: 0,
    currentBid: 0,
    imageUrl: ''
  };
}
