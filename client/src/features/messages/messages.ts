import { Component, inject, OnInit, signal } from '@angular/core';
import { MessageService } from '../../core/services/message-service';
import { Message } from '../../types/message';
import { PaginatedResult } from '../../types/pagination';
import { Paginator } from "../../shared/paginator/paginator";
import { TimeAgoPipe } from "../../core/pipes/time-ago-pipe";
import { RouterLink } from "@angular/router";
import { ConfirmDialogService } from '../../core/services/confirm-dialog-service';

@Component({
  selector: 'app-messages',
  imports: [Paginator, TimeAgoPipe, RouterLink],
  templateUrl: './messages.html',
  styleUrl: './messages.css'
})
export class Messages implements OnInit {
  private messageService = inject(MessageService);
  private confirmDialogService = inject(ConfirmDialogService);
  protected container = 'Inbox';
  protected fetchedContainer = 'Inbox';
  protected pageNumber = 1
  protected pageSize = 5;
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
  async confirmDeleteMessage(event: Event, id: number) {
    event.stopPropagation();
    this.confirmDialogService
      .confirm('Are you sure you want to delete this message?')
      .then(result => {
        if(result) this.deleteMessage(id);
      });
  }
  deleteMessage(id: number) {
    this.messageService.deleteMessage(id).subscribe({
      next: () => {
        const currentMessages = this.paginatedMessages();
        if (currentMessages) {
          const updatedMessages = currentMessages.items.filter(m => m.id !== id);
          this.paginatedMessages.set({
            ...currentMessages,
            items: updatedMessages
          });
        }
      }
    });
  }
}
