import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RegistrationService, Registration, UserAccount } from '../core/register';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin.html',
  styleUrl: './admin.css',
})
export class AdminComponent implements OnInit {
  private registrationService = inject(RegistrationService);

  registrations: Registration[] = [];
  users: UserAccount[] = [];
  
  activeTab: 'registrations' | 'members' | 'analytics' = 'registrations';
  loading = false;
  processingId: string | null = null;
  
  // Stări pentru modalul de respingere
  showRejectModal = false;
  rejectId: string | null = null;
  rejectionReason = '';

  // Metricile Analytics
  totalRegistrations = 0;
  acceptedCount = 0;
  rejectedCount = 0;
  pendingCount = 0;
  acceptanceRate = 0;

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.loading = true;
    
    // Obține înscrierile
    this.registrationService.getRegistrations().subscribe({
      next: (regs) => {
        this.registrations = regs;
        this.calculateAnalytics();
        
        // Obține membrii
        this.registrationService.getUsers().subscribe({
          next: (users) => {
            this.users = users;
            this.loading = false;
          },
          error: (err) => {
            console.error('Eroare la încărcarea membrilor:', err);
            this.loading = false;
          }
        });
      },
      error: (err) => {
        console.error('Eroare la încărcarea înscrierilor:', err);
        this.loading = false;
      }
    });
  }

  acceptRegistration(id: string) {
    this.processingId = id;
    this.registrationService.processRegistration(id, 'accept').subscribe({
      next: (res) => {
        this.processingId = null;
        alert('Înscrierea a fost acceptată! Contul a fost creat și trimis prin e-mail.');
        this.loadData();
      },
      error: (err) => {
        this.processingId = null;
        alert('Eroare la acceptarea înscrierii: ' + (err.error?.message || err.message));
      }
    });
  }

  openRejectModal(id: string) {
    this.rejectId = id;
    this.rejectionReason = '';
    this.showRejectModal = true;
  }

  closeRejectModal() {
    this.showRejectModal = false;
    this.rejectId = null;
  }

  submitRejection() {
    if (!this.rejectId) return;
    
    this.processingId = this.rejectId;
    const reason = this.rejectionReason.trim();
    this.closeRejectModal();

    this.registrationService.processRegistration(this.rejectId, 'reject', reason).subscribe({
      next: (res) => {
        this.processingId = null;
        alert('Înscrierea a fost respinsă și e-mailul de notificare a fost trimis.');
        this.loadData();
      },
      error: (err) => {
        this.processingId = null;
        alert('Eroare la respingerea înscrierii: ' + (err.error?.message || err.message));
      }
    });
  }

  deleteUser(id: string) {
    if (!confirm('Ești sigur că vrei să ștergi acest cont de utilizator? Această acțiune este ireversibilă.')) {
      return;
    }

    this.loading = true;
    this.registrationService.deleteUser(id).subscribe({
      next: (res) => {
        alert('Utilizatorul a fost șters din sistem.');
        this.loadData();
      },
      error: (err) => {
        this.loading = false;
        alert('Eroare la ștergerea utilizatorului: ' + (err.error?.message || err.message));
      }
    });
  }

  calculateAnalytics() {
    this.totalRegistrations = this.registrations.length;
    this.acceptedCount = this.registrations.filter(r => r.status === 'Accepted').length;
    this.rejectedCount = this.registrations.filter(r => r.status === 'Rejected').length;
    this.pendingCount = this.registrations.filter(r => r.status === 'Pending').length;
    
    if (this.totalRegistrations > 0) {
      this.acceptanceRate = Math.round((this.acceptedCount / this.totalRegistrations) * 100);
    } else {
      this.acceptanceRate = 0;
    }
  }

  getPendingRegistrations(): Registration[] {
    return this.registrations.filter(r => r.status === 'Pending');
  }

  getProcessedRegistrations(): Registration[] {
    return this.registrations.filter(r => r.status !== 'Pending');
  }
}
