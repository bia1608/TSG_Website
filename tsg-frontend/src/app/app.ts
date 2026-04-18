import { Component, signal } from '@angular/core';
import { NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { NavbarComponent } from './shared/navbar/navbar';
import { FooterComponent } from './shared/footer/footer';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, NavbarComponent, FooterComponent],
  styleUrl: './app.css',
  standalone: true,
  template: `
  <app-navbar />
  <main>
    <router-outlet />
  </main>
  <app-footer />
  `
})
export class App {
  protected readonly title = signal('tsg-frontend');

  
}
