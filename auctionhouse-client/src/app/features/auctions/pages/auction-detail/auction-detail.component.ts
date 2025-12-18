import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CurrencyDkkPipe } from '../../../../shared/pipes/currency-dkk.pipe';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-auction-detail',
  standalone: true,
  imports: [CommonModule, FormsModule, CurrencyDkkPipe, RouterLink],
  templateUrl: './auction-detail.component.html',
  styleUrls: ['./auction-detail.component.css']
})
export class AuctionDetailComponent implements OnInit {
  lot: any = null;
  bidHistory: any[] = [];
  newBid: number | null = null;
  loading = true;
  errorMsg = '';
  countdown: string = '';

  constructor(private http: HttpClient, private route: ActivatedRoute) {}

  ngOnInit() {
    const lotId = this.route.snapshot.paramMap.get('id');
    if (lotId) {
      this.loadLot(+lotId);
    } else {
      this.errorMsg = 'Lot ikke fundet';
      this.loading = false;
    }
  }

  loadLot(id: number) {
    this.http.get<any>(`http://localhost:5098/api/Admin/GetAuction/${id}`).subscribe({
      next: (res) => {
        const lotData = res.lots?.[0] || res.Lots?.[0];

        if (lotData) {
          if (lotData.images || lotData.Images) {
            const imgs = lotData.images || lotData.Images;
            imgs.forEach((img: any) => {
              if (img.url && !img.url.startsWith('http')) {
                img.url = `http://localhost:5098${img.url}`;
              } else if (img.Url && !img.Url.startsWith('http')) {
                img.Url = `http://localhost:5098${img.Url}`;
              }
            });
          }

          this.lot = lotData;
          this.bidHistory = lotData.bids || lotData.Bids || [];

          this.loading = false;
          this.updateCountdown();
          setInterval(() => this.updateCountdown(), 1000);
        } else {
          this.errorMsg = 'Ingen vare fundet på denne auktion';
          this.loading = false;
        }
      },
      error: (err) => {
        console.error('Kunne ikke hente data', err);
        this.errorMsg = 'Kunne ikke hente auktion';
        this.loading = false;
      }
    });
  }

  updateCountdown() {
    if (!this.lot || !this.lot.endTime) return;

    const endTime = new Date(this.lot.endTime).getTime();
    const now = new Date().getTime();
    const diff = endTime - now;

    if (diff <= 0) {
      this.countdown = 'Auktionen er slut';
      return;
    }

    const hours = Math.floor(diff / (1000 * 60 * 60));
    const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));
    const seconds = Math.floor((diff % (1000 * 60)) / 1000);

    this.countdown = `${hours}t ${minutes}m ${seconds}s`;
  }

  placeBid() {
    if (!this.newBid || this.newBid <= (this.lot.currentPrice || 0)) {
      alert('Dit bud skal være højere end nuværende bud!');
      return;
    }

    this.http.post(`http://localhost:5098/api/Bids/${this.lot.id}`, { amount: this.newBid, maxBid: 0 }).subscribe({
      next: (res: any) => {
        this.lot.currentPrice = res.currentPrice;
        this.bidHistory = res.bidHistory || [];
        this.newBid = null;
      },
      error: (err) => {
        alert(err.error || 'Noget gik galt med buddet');
      }
    });
  }

  get sortedBidHistory() {
    return [...this.bidHistory].sort((a, b) => new Date(b.timeStamp).getTime() - new Date(a.timeStamp).getTime());
  }
}
