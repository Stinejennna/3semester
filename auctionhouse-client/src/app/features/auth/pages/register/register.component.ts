import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {Router, RouterLink} from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  fullname: string = '';
  username: string = '';
  email: string = '';
  password: string = '';
  confirmPassword: string = '';

  submitted: boolean = false;
  errorMessage: string = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  register() {
    this.submitted = true;
    this.errorMessage = '';

    if (!this.fullname || !this.username || !this.email || !this.password || !this.confirmPassword) {
      this.errorMessage = 'Alle felter skal udfyldes.';
      return;
    }

    if (!this.validateEmail(this.email)) {
      this.errorMessage = 'Email er ikke gyldig.';
      return;
    }

    if (this.password !== this.confirmPassword) {
      this.errorMessage = 'Passwords matcher ikke.';
      return;
    }

    this.authService
      .register(this.username, this.fullname, this.email, this.password)
      .subscribe({
        next: () => {
          alert('Konto oprettet. Du kan nu logge ind');
          this.router.navigate(['/login']);
        },
        error: (err: HttpErrorResponse) => {
          if (err.error && typeof err.error === 'string') {
            // Hvis backend sender string
            this.errorMessage = err.error;
          } else if (err.error && err.error.message) {
            // Hvis backend sender { message: '...' }
            this.errorMessage = err.error.message;
          } else {
            this.errorMessage = `Noget gik galt (${err.status})`;
          }
        }
      });
  }

  validateEmail(email: string): boolean {
    return /\S+@\S+\.\S+/.test(email);
  }
}
