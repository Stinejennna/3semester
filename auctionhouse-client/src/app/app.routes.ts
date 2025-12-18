import { Routes } from '@angular/router';

//Forside
import { HomeComponent} from './features/home/pages/home/home.component'

// Auctions
import { AuctionListComponent } from './features/auctions/pages/auction-list/auction-list.component';
import { AuctionDetailComponent } from './features/auctions/pages/auction-detail/auction-detail.component';

// Auth
import { LoginComponent } from './features/auth/pages/login/login.component';
import { RegisterComponent } from './features/auth/pages/register/register.component';

// User
import { ProfileComponent } from './features/user/pages/profile/profile.component';
import { MyBidsComponent } from './features/user/pages/my-bids/my-bids.component';

//admin
import { AdminDashboardComponent } from './features/admin/pages/dashboard/admin-dashboard.component';
import { AdminAuctionsComponent } from './features/admin/pages/auctions/admin-auctions.component';
import { AdminUsersComponent } from './features/admin/pages/users/admin-users.component';
import { AdminAuctionCreateComponent } from './features/admin/pages/auctions/create/admin-auction-create.component';

// Guards
import { AuthGuard } from './core/guards/auth.guard';
import { AdminGuard } from './core/guards/admin.guard';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'auctions', component: AuctionListComponent },
  { path: 'auction/:id', component: AuctionDetailComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'profile', component: ProfileComponent, canActivate: [AuthGuard] },
  { path: 'my-bids', component: MyBidsComponent, canActivate: [AuthGuard] },
  {
    path: 'admin',
    children: [
      { path: '', component: AdminDashboardComponent,  },
      { path: 'auctions', component: AdminAuctionsComponent, },
      { path: 'auctions/create', component: AdminAuctionCreateComponent },
      { path: 'users', component: AdminUsersComponent, }
    ]
  }
];
