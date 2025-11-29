import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { ToastService } from './toast-service';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { User } from '../../types/user';
import { Message } from '../../types/message';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  private hubUrl = environment.hubUrl;
  private toastService = inject(ToastService);
  hubConnection?: HubConnection;
  onlineUsers = signal<number[]>([]);

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
    this.hubConnection.on('UserOnline', (id: number) => {
      this.onlineUsers.update(users => [...users, id]);
    });
    // listen for the "UserOffline" event from the server
    this.hubConnection.on('UserOffline', (id: number) => {
      this.onlineUsers.update(users => users.filter(x => x !== id));
    });
    // listen for the "GetOnlineUsers" event from the server
    this.hubConnection.on('GetOnlineUsers', (ids: number[]) => {
      this.onlineUsers.set(ids);
    });
    this.hubConnection.on("newMessageReceived", (message: Message) => {
      this.toastService.info( `new message recieved from: ${message.senderDisplayName}.`, 
        5000,message.senderImageUrl, `/members/${message.senderId}/messages`);
    }) 
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