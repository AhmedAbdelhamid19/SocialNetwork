using API.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using API.Interfaces;
using API.DTOs;
using System.Security.Claims;
using System.Globalization;

namespace API.Controllers
{        
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MembersController(IMemberRepository memberRepository) : ControllerBase
    {   
        [HttpGet("GetUsers")]
        public async Task<ActionResult<IReadOnlyList<Member>>> GetMembers()
        {
            var users = await memberRepository.GetMembersAsync();
            return Ok(users);
        }


        [HttpGet("GetUser/{id}")]
        public async Task<ActionResult<Member>> GetMember(int id)
        {
            var user = await memberRepository.GetMemberByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }

        [HttpGet("{id}/photos")]
        public async Task<ActionResult<IReadOnlyList<Photo>>> GetMemberPhotos(int id)
        {
            var photos = await memberRepository.GetPhotosForMemberAsync(id);
            return Ok(photos);
        }

        [HttpPut("update")]
        public async Task<ActionResult> UpdateMember(MemberUpdateDto memberUpdateDto) 
        {
            var memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(memberId == null) return BadRequest("no id found in token");

            var member = await memberRepository.GetMemberForUpdate(int.Parse(memberId));

            if(member == null) return BadRequest("Member doesn't exist");
            member.DisplayName = memberUpdateDto.DisplayName ?? member.DisplayName;
            member.Description = memberUpdateDto.Description ?? member.Description;
            member.City = memberUpdateDto.City ?? member.City;
            member.Country = memberUpdateDto.Country ?? member.Country;
            member.User.DisplayName = memberUpdateDto.DisplayName ?? member.DisplayName;
            
            // Update here just to make an entity updated so save change return number greater than 1.
            // it may be optional
            memberRepository.Update(member);
            if(await memberRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Faild to update member");
        }
    }
}
