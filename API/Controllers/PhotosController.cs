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
    public class PhotosController(IMemberRepository memberRepository, IPhotoService photoService) : BaseApiController
    {   
        [HttpGet("{id}")]
        public async Task<ActionResult<IReadOnlyList<Photo>>> GetMemberPhotos(int id)
        {
            var photos = await memberRepository.GetPhotosForMemberAsync(id);
            return Ok(photos);
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<Photo>> AddPhoto([FromForm] IFormFile file)
        {
            var memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (memberId == null) return BadRequest("no id found in token");

            var member = await memberRepository
            .GetMemberByIdAsync(int.Parse(memberId), includeUser: false, includePhotos: false);
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

            member.Photos.Add(photo);

            if (await memberRepository.SaveAllAsync())
            {
                // CreatedAtAction to point to MembersController.GetMember
                return CreatedAtAction("GetMember", "Members", new { id = member.Id }, photo);
            }

            return BadRequest("Problem happend at adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (memberId == null) return BadRequest("no id found in token");

            var member = await memberRepository
                .GetMemberByIdAsync(int.Parse(memberId), includeUser: true, includePhotos: true);
            if (member == null) return BadRequest("Member doesn't exist");

            var photo = member.Photos.SingleOrDefault(x => x.Id == photoId);
            if (photo == null) return NotFound("Photo not found");

            if (photo.Url == member.ImageUrl) return BadRequest("This is already your main photo");

            member.ImageUrl = photo.Url;
            member.User.ImageUrl = photo.Url;

            if (await memberRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to set main photo, try again");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (memberId == null) return BadRequest("no id found in token");

            var member = await memberRepository
                .GetMemberByIdAsync(int.Parse(memberId), includeUser: false, includePhotos: true);
            if (member == null) return BadRequest("Member doesn't exist");

            var photo = member.Photos.SingleOrDefault(x => x.Id == photoId);
            if (photo == null) return NotFound("Photo not found");

            if (photo.Url == member.ImageUrl) return BadRequest("You cannot delete your main photo");

            if (photo.PublicId != null)
            {
                var result = await photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            member.Photos.Remove(photo);

            if (await memberRepository.SaveAllAsync()) return Ok();

            return BadRequest("Failed to delete the photo");
        }
    }
}
