import { Component, inject  } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { Nav } from '../layout/nav/nav';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-root',
  imports: [Nav, RouterOutlet, NgClass],
  templateUrl: './app.html',
  styleUrl: './app.css'  
})
export class App {
  protected router = inject(Router);


}
