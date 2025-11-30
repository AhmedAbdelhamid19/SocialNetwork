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
    public class MessageController(IUnitOfWork unitOfWork) : BaseApiController
    {
        [HttpPost("sendMessage")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDTO sendMessageDTO)
        {
            int senderId = User.GetMemberId();
            var sender = await unitOfWork.MemberRepository.GetMemberByIdAsync(senderId);
            if (sender == null) return NotFound("Sender not found");
            var recipient = await unitOfWork.MemberRepository.GetMemberByIdAsync(sendMessageDTO.RecipientId);
            if (recipient == null) return NotFound("Recipient not found");

            var message = new Entities.Message
            {
                SenderId = sender.Id,
                RecipientId = recipient.Id,
                Content = sendMessageDTO.Content,
                Sender = sender,
                Recipient = recipient
            };

            unitOfWork.MessageRepository.AddMessage(message);

            if (await unitOfWork.Complete())
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

            var paginatedMessages = await unitOfWork.MessageRepository
                .GetMessagesForMember(messageParams, memberId);

            return Ok(paginatedMessages);
        }
        
        [HttpGet("thread/{recipientId}")]
        public async Task<ActionResult<IReadOnlyList<MessageDTO>>> GetMessageThread(int recipientId)
        {
            var memberIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (memberIdStr == null) return BadRequest("no id found in token");
            var memberId = int.Parse(memberIdStr);

            var messageThread = await unitOfWork.MessageRepository
                .GetMessageThread(memberId, recipientId);

            return Ok(messageThread);
        }
    
        [HttpDelete("deleteMessage/{messageId}")]
        public async Task<IActionResult> DeleteMessage(int messageId)
        {
            var memberIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (memberIdStr == null) return BadRequest("no id found in token");
            var memberId = int.Parse(memberIdStr);

            var message = await unitOfWork.MessageRepository.GetMessage(messageId);
            if (message == null) return NotFound();

            if (message.SenderId != memberId && message.RecipientId != memberId)
                return Unauthorized();

            if (message.SenderId == memberId) message.SenderDeleted = true;
            if (message.RecipientId == memberId) message.RecipientDeleted = true;

            if (message.SenderDeleted && message.RecipientDeleted)
                unitOfWork.MessageRepository.DeleteMessage(message);

            if (await unitOfWork.Complete()) return Ok();

            return BadRequest("Problem deleting the message");    
        }
    }
} 