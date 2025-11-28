import { Component, computed, effect, ElementRef, inject, OnDestroy, OnInit, signal, ViewChild } from '@angular/core';
import { MessageService } from '../../../core/services/message-service';
import { MemberService } from '../../../core/services/member-service';
import { Message } from '../../../types/message';
import { DatePipe } from '@angular/common';
import { TimeAgoPipe } from '../../../core/pipes/time-ago-pipe';
import { FormsModule } from '@angular/forms';
import { PresenceService } from '../../../core/services/presence-service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-member-messages',
  imports: [DatePipe, TimeAgoPipe, FormsModule],
  templateUrl: './member-messages.html',
  styleUrl: './member-messages.css'
})
export class MemberMessages implements OnInit, OnDestroy{
  @ViewChild('endOfMessages') endOfMessages!: ElementRef;
  protected presenceService = inject(PresenceService);
  private memberService = inject(MemberService);
  protected messageService = inject(MessageService);
  private router = inject(ActivatedRoute);
  protected MessageContent = '';
  protected isOnline = computed(() => {
    return this.presenceService.onlineUsers().map(id => Number(id))
      .includes(this.memberService.member()?.id??-1);
  });
  constructor() {
    effect(() => {
      const messages = this.messageService.messageThread();
      setTimeout(() => {
        this.scrollToBottom()
      })
    })
  }

  ngOnInit(): void {
    this.router.parent?.paramMap.subscribe({
      next: params => {
        const otherUserId = params.get('id');
        if(!otherUserId) throw new Error('Cannot connect to hub');

        this.messageService.createHubConnection(Number(otherUserId));
      }
    });
  }
  sendMessage() {
    const memberId = this.memberService.member()?.id;
    if(memberId && this.MessageContent.trim()) {
      this.messageService.sendMessage(this.MessageContent, memberId)?.then(() => {
        this.MessageContent = '';
      })
    }
  }
  scrollToBottom() {
    // Using setTimeout to ensure scrolling happens after the view updates
    if(this.endOfMessages) {
      this.endOfMessages.nativeElement.scrollIntoView({ behavior: 'smooth' });
    }

  }    
  ngOnDestroy(): void {
    console.log("destroyed");
    this.messageService.stopHubConnection();
  }
} 