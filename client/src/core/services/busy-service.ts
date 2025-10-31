import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class BusyService {
  busyRequestCount = signal(0)

  busy() {
    this.busyRequestCount.update(count => count + 1)
  }
  idle() {
    this.busyRequestCount.update(count => Math.max(0, count - 1))
  }
}
