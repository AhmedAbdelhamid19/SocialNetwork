using System.Security.Claims;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController(IMessageRepository messageRepository, IMemberRepository memberRepository) : BaseApiController
    {
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] MessageParams messageParams)
        {
            var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (senderId == null) return BadRequest("no id found in token");
            var sender = await memberRepository.GetMemberByIdAsync(int.Parse(senderId));
            if (sender == null) return NotFound("Sender not found");
            var recipient = await memberRepository.GetMemberByIdAsync(messageParams.RecipientId);
            if (recipient == null) return NotFound("Recipient not found");

            var message = new Entities.Message
            {
                SenderId = sender.Id,
                RecipientId = recipient.Id,
                Content = messageParams.Content,
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
    }
}
