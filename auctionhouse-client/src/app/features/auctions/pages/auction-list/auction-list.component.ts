import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-auction-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './auction-list.component.html',
  styleUrls: ['./auction-list.component.css']
})
export class AuctionListComponent implements OnInit {
  auctions: any[] = [];
  search: string = '';
  sortBy: string = '';
  loading = true;

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.loadAuctions();
    setInterval(() => {
      this.auctions.forEach(a => {
        const end = new Date(a.endDate).getTime();
        const now = new Date().getTime();
        const diff = end - now;
        if (diff > 0) {
          const minutes = Math.floor(diff / 1000 / 60);
          const seconds = Math.floor((diff / 1000) % 60);
          a.countdown = `${minutes}m ${seconds}s`;
        } else {
          a.countdown = "Auktion afsluttet";
        }
      });
    }, 1000);
  }

  loadAuctions() {
    const baseUrl = 'http://localhost:5098';

    this.http.get<any[]>('http://localhost:5098/api/Admin/GetAllAuctions').subscribe({
      next: (data) => {
        this.auctions = data.map(a => {
          const lot = a.lots?.[0] || a.Lots?.[0];

          const images = lot?.images || lot?.Images;
          const imageUrl = images?.[0]?.url || images?.[0]?.Url;

          const fullImageUrl = imageUrl
            ? `${baseUrl}${imageUrl}`
            : 'assets/images/placeholder.png';

          return {
            id: a.id || a.Id,
            title: a.title || a.Title,
            endDate: lot?.endTime || lot?.EndTime || a.endDate || a.EndDate,
            currentPrice: lot?.currentPrice || lot?.CurrentPrice || 0,
            startPrice: lot?.startPrice || lot?.StartPrice || 0, // Tilføj denne for at undgå 'undefined' i HTML
            mainImage: fullImageUrl,
            countdown: ''
          };
        });
        this.updateCountdowns();
        this.loading = false;
      },
      error: (err) => {
        console.error('Kunne ikke hente auktioner', err);
        this.loading = false;
      }
    });
  }

  updateCountdowns() {
    const now = new Date().getTime();
    this.auctions.forEach(a => {
      const end = new Date(a.endDate).getTime();
      const diff = end - now;
      if (diff <= 0) {
        a.countdown = 'Sluttet';
        return;
      }
      const hours = Math.floor(diff / (1000 * 60 * 60));
      const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));
      const seconds = Math.floor((diff % (1000 * 60)) / 1000);
      a.countdown = `${hours}t ${minutes}m ${seconds}s`;
    });
  }

  get filteredAuctions() {
    let list = this.auctions;
    if (this.search) {
      list = list.filter(a => a.title.toLowerCase().includes(this.search.toLowerCase()));
    }
    if (this.sortBy === 'title') {
      list = [...list].sort((a, b) => a.title.localeCompare(b.title));
    } else if (this.sortBy === 'price') {
      list = [...list].sort((a, b) => a.currentPrice - b.currentPrice);
    }
    return list;
  }
}
