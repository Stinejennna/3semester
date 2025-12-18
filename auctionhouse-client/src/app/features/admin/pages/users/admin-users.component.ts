import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-admin-users',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-users.component.html',
  styleUrls: ['./admin-users.component.css']
})
export class AdminUsersComponent implements OnInit {
  users: any[] = [];
  search = '';

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.loadUsers();
  }

  loadUsers() {
    this.http
      .get<any[]>('http://localhost:5098/api/Admin/GetAllUsers')
      .subscribe(res => {
        console.log(res);
        this.users = res;
      });
  }

  deleteUser(id: number) {
    if (!confirm('Er du sikker på du vil slette denne bruger?')) return;
    this.http.delete(`http://localhost:5098/api/Admin/DeleteUser/${id}`).subscribe(() => {
      this.users = this.users.filter(u => u.id !== id);
    });
  }

  get filteredUsers() {
    return this.users.filter(u =>
      u.userName?.toLowerCase().includes(this.search.toLowerCase())
    );
  }
}
