import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  constructor(private http: HttpClient) {}

  getAllAuctions(): Observable<any> {
    return this.http.get('/api/Admin/GetAllAuctions');
  }

  deleteAuction(id: number): Observable<any> {
    return this.http.delete(`/api/Admin/DeleteAuction/${id}`);
  }

  getAllUsers(): Observable<any> {
    return this.http.get('/api/Admin/GetAllUsers');
  }

  deleteUser(id: number): Observable<any> {
    return this.http.delete(`/api/Admin/DeleteUser/${id}`);
  }
}
