using System.Security.Claims;
using API.Extensions;
using API.Helpers;
using API.DTOs;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController(IMessageRepository messageRepository, IMemberRepository memberRepository) : BaseApiController
    {
        [HttpPost("sendMessage")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDTO sendMessageDTO)
        {
            var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (senderId == null) return BadRequest("no id found in token");
            var sender = await memberRepository.GetMemberByIdAsync(int.Parse(senderId));
            if (sender == null) return NotFound("Sender not found");
            var recipient = await memberRepository.GetMemberByIdAsync(sendMessageDTO.RecipientId);
            if (recipient == null) return NotFound("Recipient not found");

            var message = new Entities.Message
            {
                SenderId = sender.Id,
                RecipientId = recipient.Id,
                Content = sendMessageDTO.Content,
                Sender = sender,
                Recipient = recipient
            };

            messageRepository.AddMessage(message);

            if (await messageRepository.SaveAllAsync())
            {
                var messageDto = message.ToMessageDTO();
                return Ok(messageDto);
            }

            return BadRequest("Failed to send message");
        }

        [HttpGet("getMessages")]
        public async Task<ActionResult<PaginatedResult<MessageDTO>>> GetMessagesForMember([FromQuery] MessageParams messageParams)
        {
            var memberIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (memberIdStr == null) return BadRequest("no id found in token");
            var memberId = int.Parse(memberIdStr);

            var paginatedMessages = await messageRepository.GetMessagesForMember(messageParams, memberId);

            return Ok(paginatedMessages);
        }
        
        [HttpGet("thread/{recipientId}")]
        public async Task<ActionResult<IReadOnlyList<MessageDTO>>> GetMessageThread(int recipientId)
        {
            var memberIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (memberIdStr == null) return BadRequest("no id found in token");
            var memberId = int.Parse(memberIdStr);

            var messageThread = await messageRepository.GetMessageThread(memberId, recipientId);

            return Ok(messageThread);
        }
    
        [HttpDelete("deleteMessage/{messageId}")]
        public async Task<IActionResult> DeleteMessage(int messageId)
        {
            var memberIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (memberIdStr == null) return BadRequest("no id found in token");
            var memberId = int.Parse(memberIdStr);

            var message = await messageRepository.GetMessage(messageId);
            if (message == null) return NotFound();

            if (message.SenderId != memberId && message.RecipientId != memberId)
                return Unauthorized();

            if (message.SenderId == memberId) message.SenderDeleted = true;
            if (message.RecipientId == memberId) message.RecipientDeleted = true;

            if (message.SenderDeleted && message.RecipientDeleted)
                messageRepository.DeleteMessage(message);

            if (await messageRepository.SaveAllAsync()) return Ok();

            return BadRequest("Problem deleting the message");    
        }
    }
}