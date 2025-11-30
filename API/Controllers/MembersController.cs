using API.Data;
using Microsoft.AspNetCore.Mvc;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using API.Interfaces;
using API.DTOs;
using System.Security.Claims;
using API.Helpers;

namespace API.Controllers
{        
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MembersController(IUnitOfWork unitOfWork) : BaseApiController
    {   
        [HttpGet("GetUsers")]
        public async Task<ActionResult<IReadOnlyList<Member>>> GetMembers([FromQuery]MemberParams memberParams)
        {
            if(memberParams.CurrentMemberId == null)
            {
                var memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (memberId == null) return BadRequest("no id found in token");
                memberParams.CurrentMemberId = int.Parse(memberId);
            }
            var users = await unitOfWork.MemberRepository.GetMembersAsync(memberParams);
            return Ok(users);
        }
        [HttpGet("GetUser/{id}")]
        public async Task<ActionResult<Member>> GetMember(int id)
        {
            var user = await unitOfWork.MemberRepository
                .GetMemberByIdAsync(id, includeUser: false, includePhotos: false);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }

        [HttpPut("update")]
        public async Task<ActionResult> UpdateMember(MemberUpdateDto memberUpdateDto)
        {
            var memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (memberId == null) return BadRequest("no id found in token");

            var member = await unitOfWork.MemberRepository
                .GetMemberByIdAsync(int.Parse(memberId), includeUser: true, includePhotos: false);

            if (member == null) return BadRequest("Member doesn't exist");
            member.DisplayName = memberUpdateDto.DisplayName ?? member.DisplayName;
            member.Description = memberUpdateDto.Description ?? member.Description;
            member.City = memberUpdateDto.City ?? member.City;
            member.Country = memberUpdateDto.Country ?? member.Country;
            member.User.DisplayName = memberUpdateDto.DisplayName ?? member.DisplayName;

            // Update here just to make an entity updated so save change return number greater than 1.
            // it may be optional
            unitOfWork.MemberRepository.Update(member);
            if (await unitOfWork.Complete()) return NoContent();
            return BadRequest("Faild to update member");
        }
    }
}