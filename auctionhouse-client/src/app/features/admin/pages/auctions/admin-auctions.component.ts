import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';

interface Auction {
  id: number;
  title: string;
  startDate: string;
  endDate: string;
  lots?: any[];
}

@Component({
  selector: 'app-admin-auctions',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterModule, FormsModule],
  templateUrl: './admin-auctions.component.html',
  styleUrls: ['./admin-auctions.component.css']
})
export class AdminAuctionsComponent implements OnInit {

  auctions: Auction[] = [];
  loading = false;
  error: string | null = null;

  editingAuction: Auction | null = null;
  editForm: { title?: string; startDate?: string; endDate?: string } = {};

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.loadAuctions();
  }

  openEditModal(auction: Auction): void {
    this.editingAuction = auction;
    this.editForm = {
      title: auction.title,
      startDate: auction.startDate ? auction.startDate.substring(0, 16) : '',
      endDate: auction.endDate ? auction.endDate.substring(0, 16) : ''
    };
  }

  closeEditModal(): void {
    this.editingAuction = null;
    this.editForm = {};
  }

  saveEdit(): void {
    if (!this.editingAuction) return;

    this.http.put(`http://localhost:5098/api/Admin/EditAuction/${this.editingAuction.id}`, this.editForm)
      .subscribe({
        next: (res: any) => {
          const index = this.auctions.findIndex(a => a.id === this.editingAuction!.id);
          if (index > -1) this.auctions[index] = res;
          alert('Auktionen blev opdateret!');
          this.closeEditModal();
        },
        error: () => alert('Kunne ikke opdatere auktionen')
      });
  }

  loadAuctions(): void {
    this.loading = true;
    this.error = null;

    this.http.get<Auction[]>('http://localhost:5098/api/Admin/GetAllAuctions').subscribe({
      next: (res) => {
        console.log('Modtaget data:', res);
        this.auctions = res ?? [];
        this.loading = false;
      },
      error: (err) => {
        console.error('Fejl ved hentning:', err);
        this.error = 'Kunne ikke hente auktioner';
        this.loading = false;
      }
    });
  }

  deleteAuction(id: number): void {
    if (!confirm('Er du sikker på du vil slette denne auktion?')) return;

    this.http.delete(`http://localhost:5098/api/Admin/DeleteAuction/${id}`).subscribe({
      next: () => {
        this.auctions = this.auctions.filter(a => a.id !== id);
      },
      error: () => {
        alert('Kunne ikke slette auktionen');
      }
    });
  }

  editAuction(auction: Auction, updatedData: any): void {
    this.http.put(`http://localhost:5098/api/Admin/EditAuction/${auction.id}`, updatedData).subscribe({
      next: (res: any) => {
        const index = this.auctions.findIndex(a => a.id === auction.id);
        if (index > -1) this.auctions[index] = res;
        alert('Auktionen blev opdateret!');
      },
      error: () => {
        alert('Kunne ikke opdatere auktionen');
      }
    });
  }

  closeAuction(auction: Auction): void {
    if (!confirm('Er du sikker på du vil lukke denne auktion nu?')) return;

    this.http.post(`http://localhost:5098/api/Admin/CloseAuction/${auction.id}`, {}).subscribe({
      next: () => {
        const index = this.auctions.findIndex(a => a.id === auction.id);
        if (index > -1) this.auctions[index].endDate = new Date().toISOString();
        alert('Auktionen er lukket!');
      },
      error: () => {
        alert('Kunne ikke lukke auktionen');
      }
    });
  }



  isActive(auction: Auction): boolean {
    if (!auction?.endDate) return false;
    return new Date(auction.endDate).getTime() > Date.now();
  }
}
