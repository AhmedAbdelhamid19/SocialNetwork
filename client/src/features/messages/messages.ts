import { Component, inject, OnInit, signal } from '@angular/core';
import { MessageService } from '../../core/services/message-service';
import { Message } from '../../types/message';
import { PaginatedResult } from '../../types/pagination';
import { Paginator } from "../../shared/paginator/paginator";
import { TimeAgoPipe } from "../../core/pipes/time-ago-pipe";
import { RouterLink } from "@angular/router";

@Component({
  selector: 'app-messages',
  imports: [Paginator, TimeAgoPipe, RouterLink],
  templateUrl: './messages.html',
  styleUrl: './messages.css'
})
export class Messages implements OnInit {
  private messageService = inject(MessageService);
  protected container = 'Inbox';
  protected fetchedContainer = 'Inbox';
  protected pageNumber = 1
  protected pageSize = 10;
  protected paginatedMessages = signal<PaginatedResult<Message> | null>(null);

  tabs = [
    { label: 'Inbox', container: 'Inbox' },
    { label: 'Outbox', container: 'Outbox' },
    { label: 'Unread', container: 'Unread' }
  ]

  ngOnInit(): void {
    this.loadMessages();
  }
  loadMessages() {
    this.messageService.getMessages(this.container, this.pageNumber, this.pageSize).subscribe({
      next: messages => {
        this.paginatedMessages.set(messages);
        this.fetchedContainer = this.container;
      }
    });
  }
  get isInbox() {
    return this.fetchedContainer === 'Inbox';
  }
  setContainer(container: string) {
    this.container = container;
    this.pageNumber = 1;
    this.loadMessages();
  }
  onPageChanged(event: {pageNumber: number, pageSize: number}) {
    this.pageNumber = event.pageNumber;
    this.pageSize = event.pageSize;
    this.loadMessages();
  }
}
