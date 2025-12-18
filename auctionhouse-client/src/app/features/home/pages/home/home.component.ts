import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AuctionCardComponent } from '../../../auctions/components/auction-card/auction-card.component';
import { BadgeComponent } from '../../../../shared/components/badge/badge.component';
import {RouterLink} from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    AuctionCardComponent,
    BadgeComponent
  ],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {
  featuredAuctions = [
    { title: 'Sjælden antik vase', startingBid: 5000, imageUrl: '/assets/images/vase.jpg' },
    { title: 'Maleri af ukendt kunstner', startingBid: 12000, imageUrl: '/assets/images/painting.jpg' },
    { title: 'Samlerobjekt – mønt', startingBid: 800, imageUrl: '/assets/images/coin.jpg' }
  ];

  previewBadges = [
    { name: 'Første Bud', icon: '/assets/icons/badge1.png' },
    { name: 'Regulær Byder', icon: '/assets/icons/badge2.png' },
    { name: 'Byder Maniac', icon: '/assets/icons/badge3.png' }
  ];
}
