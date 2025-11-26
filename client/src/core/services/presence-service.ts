import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { ToastService } from './toast-service';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { User } from '../../types/user';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  private hubUrl = environment.hubUrl;
  private toastService = inject(ToastService);
  private hubConnection?: HubConnection; 

  createHubConnection(user: User) {
    // create the connection
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();
    
    // starts the connection
    this.hubConnection?.start().catch(error => console.log(error));

    // listen for the "UserOnline" event from the server
    this.hubConnection.on('UserOnline', email => {
      this.toastService.info(email + ' has connected');
    });
    // listen for the "UserOffline" event from the server
    this.hubConnection.on('UserOffline', email => {
      this.toastService.info(email + ' has disconnected');
    });
  }

  stopHubConnection() { 
    if(this.hubConnection?.state === HubConnectionState.Connected) {
      this.hubConnection?.stop().catch(error => console.log(error));
    }
  }

  isConnected(): boolean {
    return this.hubConnection?.state === HubConnectionState.Connected;
  }
} 