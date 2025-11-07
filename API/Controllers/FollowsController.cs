using System.Security.Claims;
using API.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowController(IFollowRepository followRepository) : BaseApiController
    {
        [HttpPost("toggleFollow/{targetMemberId}")]
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
                    return Ok("Followed successfully");
                return BadRequest("Failed to follow member");
            }
            else
            {
                followRepository.RemoveFollow(existingFollow);
                if (await followRepository.SaveAllAsync())
                    return Ok("Unfollowed successfully");
                return BadRequest("Failed to unfollow member");
            }
        }

        [HttpGet("Follows")]
        public async Task<ActionResult> GetFollows([FromQuery] string predicate)
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id == null) return BadRequest("no id found in token");
            int memberId = int.Parse(id);

            try
            {
                var follows = await followRepository.GetAllFollowsIdsAsync(memberId, predicate);
                return Ok(follows);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
