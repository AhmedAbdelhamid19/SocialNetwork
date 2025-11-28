import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Message } from '../../types/message';
import { PaginatedResult } from '../../types/pagination';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { AccountService } from './account-service';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  private baseUrl = environment.apiUrl; 
  private http = inject(HttpClient);
  private accountService = inject(AccountService);
  private hubUrl = environment.hubUrl + "messages";
  private hubConnection?: HubConnection;
  messageThread = signal<Message[]>([]);

  createHubConnection(otherUserId: number) {
    const currentUser = this.accountService.currentUser();
    if(!currentUser) return;

    this.hubConnection = new HubConnectionBuilder()
      .withUrl(`${this.hubUrl}?otherUserId=${otherUserId}`, {
        accessTokenFactory: () => currentUser.token
      }).withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch(error => console.log(error));

    this.hubConnection.on("ReceiveMessageThread", (messages: Message[]) => {
      this.messageThread.set(messages.map(msg => ({
            ...msg,
            currentUserSender: msg.senderId !== otherUserId
      })))
    });

    this.hubConnection.on("NewMessage", (message: Message) => {
      message.currentUserSender = message.senderId === currentUser.id;
      this.messageThread.update(messages => [...messages, message]);
    })
  }
  stopHubConnection() {
    if(this.hubConnection?.state === HubConnectionState.Connected) {
      this.hubConnection.stop().catch(error => console.log(error)); 
    }
  }
  getMessages(container: string, pageNumber: number, pageSize: number) {
    let params = new HttpParams();
    params = params.append('container', container);
    params = params.append('pageNumber', pageNumber.toString());
    params = params.append('pageSize', pageSize.toString());
    return this.http.get<PaginatedResult<Message>>(`${this.baseUrl}message/getmessages`, { params });
  }
  sendMessage(content: string, recipientId: number) {
    return this.hubConnection?.invoke("SendMessage", {recipientId, content})
  }
  deleteMessage(id: number) {
    return this.http.delete(`${this.baseUrl}message/deleteMessage/${id}`);
  }
}