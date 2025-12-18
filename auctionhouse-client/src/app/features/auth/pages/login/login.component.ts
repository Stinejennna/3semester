import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import {AuthService} from '../../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  identifier: string = '';
  password: string = '';
  error: string | null = null;

  constructor(private authService: AuthService, private router: Router) {}

  login() {
    this.authService.login(this.identifier, this.password)
      .subscribe({
        next: (res) => {
          console.log('Login response:', res);
          this.error = null;
          this.router.navigate(['/profile']);
        },
        error: (err) => {
          console.error('Login error', err);
          this.error = 'Forkert email eller password';
        }
      });
  }
}
