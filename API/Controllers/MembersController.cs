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
    public class MembersController(IMemberRepository memberRepository, IPhotoService photoService) : ControllerBase
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
            if (memberId == null) return BadRequest("no id found in token");

            var member = await memberRepository.GetMemberForUpdate(int.Parse(memberId));

            if (member == null) return BadRequest("Member doesn't exist");
            member.DisplayName = memberUpdateDto.DisplayName ?? member.DisplayName;
            member.Description = memberUpdateDto.Description ?? member.Description;
            member.City = memberUpdateDto.City ?? member.City;
            member.Country = memberUpdateDto.Country ?? member.Country;
            member.User.DisplayName = memberUpdateDto.DisplayName ?? member.DisplayName;

            // Update here just to make an entity updated so save change return number greater than 1.
            // it may be optional
            memberRepository.Update(member);
            if (await memberRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Faild to update member");
        }
    
        [HttpPost("add-photo")]
        public async Task<ActionResult<Photo>> AddPhoto([FromForm] IFormFile file)
        {
            var memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (memberId == null) return BadRequest("no id found in token");

            var member = await memberRepository.GetMemberByIdAsync(int.Parse(memberId));
            if (member == null) return BadRequest("Member doesn't exist");

            var result = await photoService.UploadPhotoAsync(file);

            // the error from cloudinary is well reported.
            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
                MemberId = member.Id
            };

            if (member.ImageUrl == null)
            {
                member.ImageUrl = photo.Url;
                member.User.ImageUrl = photo.Url;
            }

            member.Photos.Add(photo);

            if (await memberRepository.SaveAllAsync())
            {
                /*
                    CreatedAtAction is used to return a 201 Created response, and include the following:
                        Location: /api/Members/GetUser/{memberId}
                        Content-Type: application/json
                    and also response body like:
                    {
                        "url": "https://res.cloudinary.com/.../{publicId}", // The SecureUrl from Cloudinary
                        "publicId": "{publicId}",                          // The PublicId from Cloudinary
                        "memberId": 123                                    // The member's ID
                    }
                    new { id = member.Id } is an anonymous object that provides the route values needed to generate the URL for the GetMember action
                    photo is the body of the response, which contains the details of the newly added photo
                */
                return CreatedAtAction(nameof(GetMember), new { id = member.Id }, photo);
            }

            return BadRequest("Problem adding photo");
        }
    }
}