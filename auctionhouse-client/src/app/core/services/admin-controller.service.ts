import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AdminControllerService {
  private apiUrl = 'http://localhost:5098/api/Admin';

  constructor(private http: HttpClient) {}

  getDashboardStats(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/DashboardStats`);
  }

  createAuction(formData: FormData) { return this.http.post(`${this.apiUrl}/CreateAuction`, formData); }
  editAuction(id: number, formData: FormData) { return this.http.put(`${this.apiUrl}/EditAuction/${id}`, formData); }
  closeAuction(id: number) { return this.http.post(`${this.apiUrl}/CloseAuction/${id}`, {}); }
  getAllUsers() { return this.http.get(`${this.apiUrl}/Users`); }
  deleteUser(id: number) { return this.http.delete(`${this.apiUrl}/DeleteUser/${id}`); }
}
