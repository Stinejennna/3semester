import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
  selector: 'app-admin-auction-create',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-auction-create.component.html',
  styleUrls: ['./admin-auction-create.component.css']
})
export class AdminAuctionCreateComponent {
  title = '';
  startDate = '';
  endDate = '';
  lotTitle = '';
  lotDescription = '';
  lotEstimatedValue: number | null = null;
  lotStartPrice: number | null = null;
  images: File[] = [];

  submitting = false;
  errorMsg = '';

  constructor(private http: HttpClient, private router: Router) {}

  onFileChange(event: any) {
    this.images = Array.from(event.target.files);
  }

  createAuction() {
    if (!this.title || !this.startDate || !this.endDate || !this.lotTitle) {
      this.errorMsg = 'Udfyld venligst alle obligatoriske felter.';
      return;
    }

    this.submitting = true;

    const formData = new FormData();
    formData.append('Title', this.title);
    formData.append('StartDate', this.startDate);
    formData.append('EndDate', this.endDate);
    formData.append('LotTitle', this.lotTitle);
    formData.append('LotDescription', this.lotDescription);
    if (this.lotEstimatedValue) formData.append('EstimatedValue', this.lotEstimatedValue.toString());
    if (this.lotStartPrice) formData.append('StartPrice', this.lotStartPrice.toString());
    this.images.forEach(img => formData.append('images', img));

    this.http.post('http://localhost:5098/api/Admin/CreateAuction', formData).subscribe({
      next: (res: any) => {
        this.submitting = false;
        this.router.navigate(['/admin/auctions']);
      },
      error: (err) => {
        console.error(err);
        this.errorMsg = 'Noget gik galt under oprettelse af auktion.';
        this.submitting = false;
      }
    });
  }
}
