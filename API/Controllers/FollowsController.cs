using System.Security.Claims;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowController(IFollowRepository followRepository) : BaseApiController
    {
        [HttpPost("toggle-Follow/{targetMemberId}")]
        public async Task<ActionResult> ToggleFollow(int targetMemberId)
        {
            var memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (memberId == null) return BadRequest("no id found in token");
            int sourceUserId = int.Parse(memberId);

            var existingFollow = await followRepository.GetFollowAsync(sourceUserId, targetMemberId);
            if (existingFollow == null)
            {
                var newFollow = new Entities.MemberFollow
                {
                    SourceMemberId = sourceUserId,
                    TargetMemberId = targetMemberId
                };
                followRepository.AddFollow(newFollow);
                if (await followRepository.SaveAllAsync())
                    return Ok(new { message = "Followed successfully" });
                return BadRequest("Failed to follow member");
            }
            else
            {
                followRepository.RemoveFollow(existingFollow);
                if (await followRepository.SaveAllAsync())
                    return Ok(new { message = "Unfollowed successfully" });
                return BadRequest("Failed to unfollow member");
            }
        }

        [HttpGet("Follows-Ids")]
        public async Task<ActionResult<PaginatedResult<Member>>> GetFollowsIds([FromQuery] FollowParams followParams)
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id == null) return BadRequest("no id found in token");
            int memberId = int.Parse(id);

            try
            {
                var follows = await followRepository.GetAllFollowsIdsAsync(memberId, followParams);
                return Ok(follows);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Follows-Members")]
        public async Task<ActionResult<PaginatedResult<Member>>> GetFollowsMembers([FromQuery] FollowParams followParams)
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id == null) return BadRequest("no id found in token");
            int memberId = int.Parse(id);

            try
            {
                var follows = await followRepository.GetAllFollowsAsync(memberId, followParams);
                return Ok(follows);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}