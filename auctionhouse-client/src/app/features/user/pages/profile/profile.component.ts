import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../../../core/services/auth.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import {CurrencyDkkPipe} from '../../../../shared/pipes/currency-dkk.pipe';

@Component({
  selector: 'app-profile',
  imports: [CommonModule, CurrencyDkkPipe],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
  user: any = null;
  stats: any = null;

  constructor(private authService: AuthService, private router: Router) {}

  ngOnInit() {
    this.loadUser();
    this.loadStats();
  }

  loadUser() {
    this.authService.getUserInfo().subscribe({
      next: (data) => {
        this.user = data;
      },
      error: () => {
        this.user = null;
        alert('Kunne ikke hente brugerdata, log venligst ind igen.');
      }
    });
  }

  loadStats() {
    this.authService.getUserStats().subscribe({
      next: (data) => {
        this.stats = {
          totalBids: data.totalBids || data.TotalBids,
          wonAuctions: data.wonAuctions || data.WonAuctions,
          totalWonValue: data.totalWonValue || data.TotalWonValue,
          badges: data.badges || data.Badges || []
        };
      },
      error: () => {
        this.stats = null;
      }
    });
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/']);
  }

  // Returner ikon-URL baseret på badge navn
  getBadgeIcon(badge: string): string {
    const lower = badge.toLowerCase();
    if (lower.includes('første bud')) return '/assets/icons/badge1.png';
    if (lower.includes('regulær byder')) return '/assets/icons/badge2.png';
    if (lower.includes('power byder')) return '/assets/icons/badge2.png';
    if (lower.includes('byder maniac')) return '/assets/icons/badge3.png';
    if (lower.includes('første vind')) return '/assets/icons/badge1.png';
    if (lower.includes('double vinder')) return '/assets/icons/badge2.png';
    if (lower.includes('auktions pro')) return '/assets/icons/badge2.png';
    if (lower.includes('auktions konge')) return '/assets/icons/badge3.png';
    if (lower.includes('automation rookie')) return '/assets/icons/badge1.png';
    if (lower.includes('strategisk byder')) return '/assets/icons/badge2.png';
    if (lower.includes('sniper bot')) return '/assets/icons/badge3.png';
    if (lower.includes('high roller')) return '/assets/icons/badge1.png';
    if (lower.includes('big spender')) return '/assets/icons/badge2.png';
    if (lower.includes('elite køber')) return '/assets/icons/badge3.png';
    if (lower.includes('sidste sekunds sniper')) return '/assets/icons/badge3.png';
    if (lower.includes('ugentlig aktiv')) return '/assets/icons/badge2.png';
    if (lower.includes('fast deltager')) return '/assets/icons/badge3.png';
    if (lower.includes('top 10 vinder')) return '/assets/icons/badge3.png';

    return '/assets/icons/default-badge.png'; // fallback
  }

  getBadgeClass(badge: string): string {
    const lower = badge.toLowerCase();

    if (lower.includes('første bud') || lower.includes('første vind') || lower.includes('automation rookie') || lower.includes('high roller')) {
      return 'badge-gold';
    }

    if (lower.includes('regulær byder') || lower.includes('power byder') || lower.includes('double vinder') || lower.includes('strategisk byder') || lower.includes('ugentlig aktiv')) {
      return 'badge-silver';
    }

    if (lower.includes('byder maniac') || lower.includes('auktions konge') || lower.includes('sniper bot') || lower.includes('elite køber') || lower.includes('sidste sekunds sniper') || lower.includes('fast deltager') || lower.includes('top 10 vinder')) {
      return 'badge-bronze';
    }

    return 'badge-default';
  }

}
