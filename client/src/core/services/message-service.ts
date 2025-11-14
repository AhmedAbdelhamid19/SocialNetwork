import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Message } from '../../types/message';
import { PaginatedResult } from '../../types/pagination';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  private baseUrl = environment.apiUrl; 
  private http = inject(HttpClient);

  getMessages(container: string, pageNumber: number, pageSize: number) {
    let params = new HttpParams();
    params = params.append('container', container);
    params = params.append('pageNumber', pageNumber.toString());
    params = params.append('pageSize', pageSize.toString());
    return this.http.get<PaginatedResult<Message>>(`${this.baseUrl}message/getmessages`, { params });
  }
  getMessageThread(memberId: number) {
    return this.http.get<Message[]>(`${this.baseUrl}message/thread/${memberId}`);
  }
  sendMessage(content: string, recipientId: number) {
    return this.http.post<Message>(`${this.baseUrl}message/sendMessage`, { content: content, recipientId: recipientId });
  }
  deleteMessage(id: number) {
    return this.http.delete(`${this.baseUrl}message/deleteMessage/${id}`);
  }
}