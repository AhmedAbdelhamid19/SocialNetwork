import { HttpClient } from '@angular/common/http';
import { Component, inject, signal, OnInit  } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { lastValueFrom } from 'rxjs';
import { Nav } from '../layout/nav/nav';

@Component({
  selector: 'app-root',
  imports: [Nav],
  templateUrl: './app.html',
  styleUrl: './app.css' 
})
export class App implements OnInit{
  private http = inject(HttpClient);
  protected readonly title  = signal('Social Network');
  protected members = signal<any>([]);

  async ngOnInit() {
    this.members.set(await this.getMembers())
  }

  async getMembers() {
    try {
      return lastValueFrom(this.http.get('https://localhost:5001/api/members/getusers'));
    } catch (error) {
      console.log(error);
      throw error;
    }
  }
}
