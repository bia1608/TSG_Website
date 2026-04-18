import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../core/auth';

@Component({
  selector: 'app-navbar',
  imports: [RouterLink, RouterLinkActive, CommonModule],
  templateUrl: './navbar.html',
  template: `<p>NAVBAR TEST</p>`,
  styleUrl: './navbar.css',
  standalone: true // componenta poate fi folosită fără a fi declarată într-un modul
})
export class NavbarComponent {
  menuOpen = false; // controleaza meniul mobil

  constructor(public auth: AuthService) { }

  toggleMenu() {
    this.menuOpen = !this.menuOpen;
  }

  logout() {
    this.auth.logout();
  } 
}
