import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../../../core/services/auth.service';
import { CommonModule } from '@angular/common';
import { CurrencyDkkPipe } from '../../../../shared/pipes/currency-dkk.pipe';

@Component({
  selector: 'app-my-bids',
  standalone: true,
  imports: [CommonModule, CurrencyDkkPipe],
  templateUrl: './my-bids.component.html',
  styleUrls: ['./my-bids.component.css']
})
export class MyBidsComponent implements OnInit {
  activeLots: any[] = [];
  wonLots: any[] = [];
  loadingActive = true;
  loadingWon = true;
  error: string | null = null;
  readonly baseUrl = 'http://localhost:5098';

  constructor(private authService: AuthService) {}

  ngOnInit() {
    this.loadActiveBids();
    this.loadWonAuctions();
  }

  private mapLotData(lots: any[]): any[] {
    if (!lots) return [];

    return lots.map(lot => {
      const images = lot.images || lot.Images || lot.imageUrls || lot.ImageUrls;
      const firstImage = images?.[0];

      let imageUrl = '';

      if (firstImage && (firstImage.url || firstImage.Url)) {
        const path = firstImage.url || firstImage.Url;
        imageUrl = path.startsWith('http') ? path : `${this.baseUrl}${path}`;
      }
      else if (typeof firstImage === 'string') {
        imageUrl = firstImage.startsWith('http') ? firstImage : `${this.baseUrl}${firstImage}`;
      }
      else {
        imageUrl = 'assets/images/placeholder.png';
      }

      return {
        ...lot,
        displayImage: imageUrl,
        displayTitle: lot.title || lot.Title || lot.lotTitle || lot.LotTitle,
        displayAuction: lot.auctionTitle || lot.AuctionTitle,
        displayPrice: lot.currentPrice || lot.CurrentPrice || lot.finalPrice || lot.FinalPrice
      };
    });
  }

  loadActiveBids() {
    this.authService.getUserInfoActiveAuctions().subscribe({
      next: (data) => {
        this.activeLots = this.mapLotData(data);
        this.loadingActive = false;
      },
      error: (err) => {
        console.error(err);
        this.error = 'Kunne ikke hente aktive bud.';
        this.loadingActive = false;
      }
    });
  }

  loadWonAuctions() {
    this.authService.getUserInfoWonAuctions().subscribe({
      next: (data) => {
        this.wonLots = this.mapLotData(data);
        this.loadingWon = false;
      },
      error: (err) => {
        console.error(err);
        this.error = 'Kunne ikke hente vundne auktioner.';
        this.loadingWon = false;
      }
    });
  }
}
