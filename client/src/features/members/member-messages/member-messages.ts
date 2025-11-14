import { Component, effect, ElementRef, inject, OnInit, signal, ViewChild } from '@angular/core';
import { MessageService } from '../../../core/services/message-service';
import { MemberService } from '../../../core/services/member-service';
import { Message } from '../../../types/message';
import { DatePipe } from '@angular/common';
import { TimeAgoPipe } from '../../../core/pipes/time-ago-pipe';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-member-messages',
  imports: [DatePipe, TimeAgoPipe, FormsModule],
  templateUrl: './member-messages.html',
  styleUrl: './member-messages.css'
})
export class MemberMessages implements OnInit{
  @ViewChild('endOfMessages') endOfMessages!: ElementRef;
  private messageService = inject(MessageService);
  private memberService = inject(MemberService);
  protected messages = signal<Message[]>([]);
  protected MessageContent = '';


  // changing the signal that affect the dom elements will put in microtask queue that run after the current call stack is cleared but before the macrotask queue and before callbacks like setTimeout
  // so we can use setTimeout here to wait for the dom to update
  // angular dom update happen in microtask queue after the current call stack is cleared
  // if currently in call stack exist callback, then microtask will wait them, but if the call stack empty and there're callback and microtasks, then microtasks will take the priorty.
  ngOnInit(): void {
    this.loadMessages();
  }
  loadMessages() {
    console.log('Loading message thread');
    const memberId = this.memberService.member()?.id;
    if(memberId) {
      this.messageService.getMessageThread(memberId).subscribe({
        next: messages => {
          this.messages.set(messages.map(msg => ({
            ...msg,
            currentUserSender: msg.senderId !== memberId
          })));
        },
        complete: () => {
          setTimeout(() => {
            this.scrollToBottom();
            console.log('Scrolled to bottom after loading messages');
          });
        }
      });
    }
  }
  sendMessage() {
    const memberId = this.memberService.member()?.id;
    if(memberId && this.MessageContent.trim()) {
      this.messageService.sendMessage(this.MessageContent, memberId).subscribe({
        next: message => {
          this.messages.update(msgs => {
            message.currentUserSender = true;
            return [...msgs, message];
          });
          this.MessageContent = '';
        },
        complete: () => {
          setTimeout(() => {
            this.scrollToBottom();
          });
        }
      });
    }
  }
  scrollToBottom() {
    // Using setTimeout to ensure scrolling happens after the view updates
    // setTimeout is necessary here to wait for the DOM to update with the new message
    // settimeout make it asynchronous and it will happen after the current call stack is cleared (remember javascript call stack and event loop and microtasks),
    // so the DOM will be updated with the new message before we try to scroll to the bottom
    if(this.endOfMessages) {
      this.endOfMessages.nativeElement.scrollIntoView({ behavior: 'smooth' });
    }
  }
} 