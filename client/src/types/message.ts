export type Message = {
  id: number
  content: string
  dateRead?: string
  messageSent: string
  senderId: number
  senderDisplayName: string
  senderImageUrl: string
  recipientId: number
  recipientDisplayName: string
  recipientImageUrl: string
  currentUserSender?: boolean 
}
