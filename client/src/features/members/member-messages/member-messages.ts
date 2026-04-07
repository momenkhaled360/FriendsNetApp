import { Component, ElementRef, inject, OnInit, signal, ViewChild} from '@angular/core';
import { MessageService } from '../../../core/services/message-service';
import { Message } from '../../../types/message';
import { ActivatedRoute } from '@angular/router';
import { DatePipe } from '@angular/common';
import { TimeAgoPipe } from '../../../core/pipes/time-ago-pipe';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-member-messages',
  imports: [DatePipe,TimeAgoPipe,FormsModule],
  templateUrl: './member-messages.html',
  styleUrl: './member-messages.css',
})
export class MemberMessages implements OnInit {
  @ViewChild('messageEndRef') messageEndRef!:ElementRef
  private messageService = inject(MessageService);
  private route = inject(ActivatedRoute);
  protected messages = signal<Message[]>([]);
  protected messageContent ='';

  ngOnInit(): void {
    this.loadMessages();
  }

  loadMessages(){
    const memberId = this.route.parent?.snapshot.paramMap.get('id');
    
    console.log(memberId)
    if(memberId){
      this.messageService.getMessageThread(memberId).subscribe({
        next: messages => {
            this.messages.set(messages.map(message=>({
              ...message,
              currentUserSender: message.senderId !== memberId
            })))

        setTimeout(() => {
          this.scrollToBottom();
        });
        }
      })
    }
  }

  sendMessage(){
    const recipientId = this.route.parent?.snapshot.paramMap.get('id');

    if(!recipientId || !this.messageContent.trim()) return;

    this.messageService.sendMessage(recipientId, this.messageContent).subscribe({
      next: message =>{
        this.messages.update(messages=>{
          message.currentUserSender = true;
          return [...messages , message];
        });

        this.messageContent = '';

        setTimeout(() => {
          this.scrollToBottom();
        });
      }
    });
  }

  scrollToBottom(){
    if(this.messageEndRef){
      this.messageEndRef.nativeElement.scrollIntoView({behavior:'smooth'})
    }
  }

}
