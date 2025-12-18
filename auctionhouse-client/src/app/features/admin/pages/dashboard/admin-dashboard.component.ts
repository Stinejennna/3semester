import { Component, OnInit } from '@angular/core';
import { AdminControllerService } from '../../../../core/services/admin-controller.service';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-admin-dashboard',
  imports: [RouterLink],
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.css']
})
export class AdminDashboardComponent implements OnInit {
  stats: any = null;

  constructor(private adminService: AdminControllerService) {}

  ngOnInit(): void {
    this.loadStats();
  }

  loadStats() {
    this.adminService.getDashboardStats().subscribe({
      next: (res) => this.stats = res,
      error: (err) => console.error('Fejl ved hentning af dashboard stats', err)
    });
  }
}
