import { Component, inject, OnInit, signal } from '@angular/core';
import { MessageService } from '../../../core/services/message-service';
import { MemberService } from '../../../core/services/member-service';
import { Message } from '../../../types/message';
import { DatePipe } from '@angular/common';
import { TimeAgoPipe } from '../../../core/pipes/time-ago-pipe';

@Component({
  selector: 'app-member-messages',
  imports: [DatePipe, TimeAgoPipe],
  templateUrl: './member-messages.html',
  styleUrl: './member-messages.css'
})
export class MemberMessages implements OnInit{
  private messageService = inject(MessageService);
  private memberService = inject(MemberService);
  protected messages = signal<Message[]>([]);
  
  ngOnInit(): void {
    this.loadMessages();
  }
  loadMessages() {
    const memberId = this.memberService.member()?.id;
    if(memberId) {
      this.messageService.getMessageThread(memberId).subscribe({
        next: messages => {
          this.messages.set(messages.map(msg => ({
            ...msg,
            currentUserSender: msg.senderId !== memberId
          })));
        }
      });
    }
  }
}