import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Registration {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  cvUrl: string;
  status: 'Pending' | 'Accepted' | 'Rejected';
  submittedAt: string;
  userId?: string;
}

export interface UserAccount {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  bio: string;
  linkedInUrl?: string;
  phoneNumber?: string;
  isAdmin: boolean;
  isActive: boolean;
}

@Injectable({
  providedIn: 'root',
})
export class RegistrationService {
  private readonly http = inject(HttpClient);
  private readonly API_REGISTER = 'http://localhost:5119/api/register';
  private readonly API_AUTH = 'http://localhost:5119/api/auth';
  private readonly API_USER = 'http://localhost:5119/api/user';

  submitRegistration(data: any): Observable<any> {
    return this.http.post(this.API_REGISTER, data);
  }

  getRegistrations(): Observable<Registration[]> {
    return this.http.get<Registration[]>(this.API_REGISTER);
  }

  processRegistration(id: string, action: 'accept' | 'reject', rejectionReason?: string): Observable<any> {
    return this.http.post(`${this.API_AUTH}/${id}/process`, { action, rejectionReason });
  }

  getUsers(): Observable<UserAccount[]> {
    return this.http.get<UserAccount[]>(this.API_USER);
  }

  deleteUser(id: string): Observable<any> {
    return this.http.delete(`${this.API_USER}/${id}/delete`);
  }
}
