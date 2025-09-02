using API.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using API.Interfaces;

namespace API.Controllers
{        
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MembersController(IMemberRepository memberRepository) : ControllerBase
    {
        private readonly IMemberRepository _memberRepository = memberRepository;
        
        [HttpGet("GetUsers")]
        public async Task<ActionResult<IReadOnlyList<Member>>> GetMembers()
        {
            var users = await _memberRepository.GetMembersAsync();
            return Ok(users);
        }


        [HttpGet("GetUser/{id}")]
        public async Task<ActionResult<Member>> GetMember(int id)
        {
            var user = await _memberRepository.GetMemberByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }

        [HttpGet("{id}/photos")]
        public async Task<ActionResult<IReadOnlyList<Photo>>> GetMemberPhotos(int id)
        {
            var photos = await _memberRepository.GetPhotosForMemberAsync(id);
            return Ok(photos);
        }
    }
}
