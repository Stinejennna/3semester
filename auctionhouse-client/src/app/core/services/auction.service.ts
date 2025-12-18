import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';

export interface Auction {
  id?: number;
  title: string;
  description: string;
  imageUrl: string;
  startingBid: number;
  currentBid?: number;
}

export interface Bid {
  user: string;
  amount: number;
}

@Injectable({
  providedIn: 'root'
})
export class AuctionService {
  private apiUrl = 'http://localhost:5098/api/Lots';

  constructor(private http: HttpClient, private auth: AuthService) {}

  // Auktioner
  getActiveAuctions(): Observable<Auction[]> {
    return this.http.get<Auction[]>(`${this.apiUrl}/active`);
  }

  getUpcomingAuctions(): Observable<Auction[]> {
    return this.http.get<Auction[]>(`${this.apiUrl}/upcoming`);
  }

  getAuction(id: number): Observable<Auction> {
    return this.http.get<Auction>(`${this.apiUrl}/${id}`);
  }

  // Bud
  getBidHistory(lotId: number): Observable<Bid[]> {
    return this.http.get<Bid[]>(`http://localhost:5098/api/Bids/${lotId}/history`);
  }

  placeBid(lotId: number, amount: number) {
    return this.http.post(`http://localhost:5098/api/Bids/${lotId}`, { amount }, { headers: this.auth.getAuthHeaders() });
  }
}
